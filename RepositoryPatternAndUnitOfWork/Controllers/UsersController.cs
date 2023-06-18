using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RepositoryPatternAndUnitOfWork.Core.IConfiguration;
using RepositoryPatternAndUnitOfWork.Filters;
using RepositoryPatternAndUnitOfWork.Models;

namespace RepositoryPatternAndUnitOfWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(ILogger<UsersController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();

                await _unitOfWork.Users.Add(user);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetItem), new { user.Id }, user);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };

        }

        [HttpGet]
        [MyAsyncActionFilter("GetAllUsersAsync")]
        [Route("GetAll")]
        [AllowAnonymous]
        [EnableRateLimiting("FiexedWindowPolicy")]
        //[Authorize(
        //    AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        //    Roles = "AppUser",
        //    Policy = "AccessDbPolicy"
        //    )]
        public async Task<IActionResult> GetALL()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            //var jobId = BackgroundJob.Enqueue<IServiceManagement>(x => x., "0 * * ? * *");
            //RecurringJob.AddOrUpdate<IServiceManagement>(x => x.UpdateDatabase(), Cron.Minutely);

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("{Id}")]
        public async Task<IActionResult> Update(Guid id, User user)
        {
            if(user.Id != id)
                return BadRequest();

            await _unitOfWork.Users.Upsert(user);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            await _unitOfWork.Users.Delete(id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
