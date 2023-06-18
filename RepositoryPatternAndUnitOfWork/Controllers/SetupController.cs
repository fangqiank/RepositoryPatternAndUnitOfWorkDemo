using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryPatternAndUnitOfWork.Data;
using System.Xml.Linq;

namespace RepositoryPatternAndUnitOfWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<SetupController> _logger;

        public SetupController(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, 
            ILogger<SetupController> logger
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllRoles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        $"The User wth Email {email} does not exist"
                    }
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            
            return Ok(roles);
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole(string name)
        {
            var roleExisted = await _roleManager.RoleExistsAsync(name);
            if (!roleExisted) 
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(name));
                if(result.Succeeded)
                {
                    _logger.LogInformation($"The Role {name} created successfully");
                    return Ok(new
                    {
                        result = $"The Role {name} created successfully"

                    });
                }

                _logger.LogError($"The Role {name} created unsuccessfully");
                return BadRequest(new
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        $"The Role {name} created unsuccessfully"
                    }

                });
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    "Role already existed"
                }
            });
        }

        [HttpPost]
        [Route("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUser(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                _logger.LogError($"The User with the email: {email} does not exist");
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        $"The User with the email: {email} does not exist"
                    }
                });
            }

            var role = await _roleManager.RoleExistsAsync(roleName);
            if (!role)
            {
                _logger.LogError($"The Role {roleName} does not exist");
                return BadRequest(new
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        $"The Role {roleName} is not found"
                    }
                });
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                _logger.LogInformation($"The Role {roleName} assigned to User: {user} successfully");
                return Ok(new
                {
                    result = $"The Role {roleName} assigned to User: {user} successfully"
                }); 
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    $"The Role {roleName} assigned to User: {user} unsuccessfully"
                }
            });
        }

        [HttpPost]
        [Route("RemoveUserFromRole")]
        public async Task<IActionResult> RemoveUserFromRole(string email ,string roleName) 
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogError($"The User with the email: {email} does not exist");
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        $"The User with the email: {email} does not exist"
                    }
                });
            }

            var role = await _roleManager.RoleExistsAsync(roleName);
            if (!role)
            {
                _logger.LogError($"The Role {roleName} does not exist");
                return BadRequest(new
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        $"The Role {roleName} is not found"
                    }
                });
            }

            var result = await _userManager.RemoveFromRolesAsync(user, new List<string> { roleName});

            if(result.Succeeded) 
            {
                _logger.LogInformation($"The Role {roleName} removed from User: {user} successfully");
                return Ok(new
                {
                    result = $"The Role {roleName} removed from User: {user} successfully"
                });
            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    $"The Role {roleName} removed from User: {user} unsuccessfully"
                }
            });
        }
    }
}
