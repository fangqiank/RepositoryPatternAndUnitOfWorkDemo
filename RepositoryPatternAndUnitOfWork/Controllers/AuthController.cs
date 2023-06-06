using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RepositoryPatternAndUnitOfWork.Data;
using RepositoryPatternAndUnitOfWork.Dtos;
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


        public AuthController(
            UserManager<IdentityUser> userManager,
            IConfiguration config
            ) 
        {
            _userManager = userManager;
            _config = config;
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
                    var token = GenerateToken(newUser);
                    
                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token
                    });
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

                var jwtToken = GenerateToken(existedUser);

                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = jwtToken
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

        private string GenerateToken(IdentityUser user)
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

                Expires = DateTime.Now.AddHours(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
