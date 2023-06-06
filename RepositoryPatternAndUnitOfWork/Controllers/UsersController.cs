﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryPatternAndUnitOfWork.Core.IConfiguration;
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

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetALL()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            return Ok(users);
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
