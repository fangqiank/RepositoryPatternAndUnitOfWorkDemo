using Dotnet7AngualrAuthWithJwt.Configurations;
using Dotnet7AngualrAuthWithJwt.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dotnet7AngualrAuthWithJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthManagementController : ControllerBase
    {
        private readonly ILogger<AuthManagementController> _logger;
        private readonly UserManager<IdentityUser> _manager;
        private readonly IOptionsMonitor<JwtConfig> _optionsMonitor;

        public AuthManagementController(
            ILogger<AuthManagementController> logger, 
            UserManager<IdentityUser> manager,
            IOptionsMonitor<JwtConfig> optionsMonitor
            )
        {
            _logger = logger;
            _manager = manager;
            _optionsMonitor = optionsMonitor;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterationRequestDto requestDto)
        {
            if(ModelState.IsValid)
            {
                var existedEmail = await _manager.FindByEmailAsync(requestDto.Email);
                if (existedEmail != null)
                    return BadRequest("Email already existed");

                var newUser = new IdentityUser
                {
                    UserName = requestDto.Name,
                    Email = requestDto.Email,
                };

                var isCreted = await _manager.CreateAsync(newUser, requestDto.Password);
                if (isCreted.Succeeded)
                {
                    return Ok(new RegisterationRequestResponse
                    {
                        Result = true,
                        Token = GenerateToken(newUser)
                    }); 
                }

                return BadRequest("Problems with creating user");
            }

            return BadRequest("Invalid rwequest payload");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var existedEmail = await _manager.FindByEmailAsync(requestDto.Email);
                if (existedEmail == null)
                    return BadRequest("Invalid user");

                var isPwdCorrect = await _manager.CheckPasswordAsync(existedEmail, requestDto.Password);

                if (isPwdCorrect)
                {
                    return Ok(new LoginRequestResponse
                    {
                        Result = true,
                        Token = GenerateToken(existedEmail)
                    });
                }
                return BadRequest("Incorrect username or password");
            }
            return BadRequest("Invalid rwequest payload");
        }

        private string GenerateToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_optionsMonitor.CurrentValue.Secret!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, 
                        user.Email ?? "zhangsan@mail.com"),
                    new Claim(JwtRegisteredClaimNames.Email, 
                        user.Email ?? "zhangsan@mail.com"),
                    new Claim(JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                }),
                Expires= DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                algorithm: SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
