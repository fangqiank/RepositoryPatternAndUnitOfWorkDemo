using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryPatternAndUnitOfWork.Data;
using RepositoryPatternAndUnitOfWork.Dtos;
using RepositoryPatternAndUnitOfWork.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryPatternAndUnitOfWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;
        private readonly TokenValidationParameters _validationParameters;

        public AuthController(
            UserManager<IdentityUser> userManager,
            IConfiguration config,
            ApplicationDbContext db,
            TokenValidationParameters validationParameters
            ) 
        {
            _userManager = userManager;
            _config = config;
            _db = db;
            _validationParameters = validationParameters;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(
            [FromBody] UserRegistrationDto userRegistrationDto)
        {
            if(ModelState.IsValid)
            {
                var existedUser = await _userManager.FindByEmailAsync(userRegistrationDto.Email!);

                if(existedUser != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email already exists"
                        }
                    });
                }

                var newUser = new IdentityUser()
                {
                    Email = userRegistrationDto.Email,
                    UserName = userRegistrationDto.Name
                };

                var isCreated = await _userManager.CreateAsync(newUser, 
                    userRegistrationDto.Password!);

                if(isCreated.Succeeded)
                {
                    var token = await GenerateToken(newUser);
                    
                    return Ok(token);
                }

                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Server Error"
                    }
                });
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    "Invalid payload"
                }
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(
            [FromBody] UserLoginDto userLoginDto)
        {
            if (ModelState.IsValid)
            {
                var existedUser = await _userManager.FindByEmailAsync(userLoginDto.Email!);

                if (existedUser == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email does not  exists"
                        }
                    });
                }

                var isCorrectPassword = await _userManager.CheckPasswordAsync(
                    existedUser, 
                    userLoginDto.Password!);

                if (!isCorrectPassword)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Server Error"
                        }
                    });
                }

                var jwtToken = await GenerateToken(existedUser);

                return Ok(jwtToken);
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    "Invalid payload"
                }
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            if(ModelState.IsValid)
            {
                var result = await VerifyAndGenerateToken(request);

                if(result == null)
                {
                    return BadRequest(new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid tokens"
                        }
                    });    
                }

                return Ok(result);
            }

            return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string>
                {
                    "Invalid parameters"
                }
            });
        }

        private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _validationParameters.ValidateLifetime = false; //for test

                var tokenInVerification = jwtTokenHandler.ValidateToken(
                    tokenRequest.Token,
                    _validationParameters,
                    out SecurityToken validatedToken
                    );

                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256, 
                        StringComparison.InvariantCultureIgnoreCase);

                    if (!result) 
                        return null;
                }

                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(
                    x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                if (expiryDate > DateTime.Now)
                    return new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                           "Expired token"      
                        }
                    };

                var storedToken = await _db.RefreshTokens.FirstOrDefaultAsync(
                    x => x.Token == tokenRequest.RefreshToken);
                if(storedToken is null)
                {
                    return new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid tokens"
                        }
                    };
                }

                if(storedToken.IsUsed || storedToken.IsRevoked)
                {
                    return new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid tokens"
                        }
                    };
                }

                var jti = tokenInVerification.Claims.FirstOrDefault(
                    x => x.Type == JwtRegisteredClaimNames.Jti)!.Value;
                if(storedToken.JwtId != jti) 
                {
                    return new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Invalid tokens"
                        }
                    };
                }

                if(storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Expired tokens"
                        }
                    };
                }

                storedToken.IsUsed = true;
                _db.RefreshTokens.Update(storedToken);
                await _db.SaveChangesAsync();

                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId!);
                return await GenerateToken(dbUser!);
            }
            catch(Exception ex)
            {
                return new AuthResult
                {
                    Result = false,
                    Errors = new List<string>
                        {
                            "Server Error"
                        }
                };
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal =new DateTime(
                1970,
                1,
                1,
                0,
                0,
                0,
                0,
                DateTimeKind.Utc);

            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp)
                .ToUniversalTime();

            return dateTimeVal;
        }

        private bool SendEmail(string body, string email) 
        {
            var option = new RestClientOptions("https//api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", 
                _config.GetSection("JwtConfig:ApiKey").Value!)
            };

            var client = new RestClient(option);

            var request = new RestRequest("", Method.Post);

            request.AddParameter("domail", "");
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "");
            request.AddParameter("to", "zhangsan@mail.com");
            request.AddParameter("subject", "Email verification");
            request.AddParameter("text", body);
            request.Method = Method.Post;

            var response = client.Execute(request);

            return response.IsSuccessful;
        }

        private async Task<AuthResult> GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secret = _config.GetSection("JwtConfig:Secret").Value;
            var key = Encoding.UTF8.GetBytes(secret!);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now
                    .ToUniversalTime().ToString())
                }),

                Expires = DateTime.UtcNow.Add(
                    TimeSpan.Parse(_config.GetSection("JwtConfig:ExpiryTime").Value!)),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                Token = RandomStringGeneration(23),
                AddDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsRevoked = false,
                IsUsed = false, 
                UserId = user.Id,

            };

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();

            return new AuthResult() 
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                Result = true
            };
        }

        private string RandomStringGeneration(int length)
        {
            var rnd = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz_";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}
