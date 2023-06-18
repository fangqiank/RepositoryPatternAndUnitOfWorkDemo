using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryPatternAndUnitOfWork.Controllers;
using RepositoryPatternAndUnitOfWork.Core.IConfiguration;
using RepositoryPatternAndUnitOfWork.Data;
using RepositoryPatternAndUnitOfWork.Dtos;
using RepositoryPatternAndUnitOfWork.Models;

namespace Testing
{

    public class RepositortPatternTests
    {

        private readonly AuthController _authController;
        private readonly Mock<Microsoft.AspNetCore.Identity.UserManager<IdentityUser>> _userManagerMock;

        public RepositortPatternTests()
        {
            _userManagerMock = new Mock<Microsoft.AspNetCore.Identity.UserManager<IdentityUser>>();
            _authController = new AuthController(
                _userManagerMock.Object, 
                null, 
                null, 
                null, 
                null, 
                null);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResultWithToken()
        {
            // Arrange
            var userLoginDto = new UserLoginDto
            {
                Email = "test@example.com",
                Password = "password"
            };

            var existedUser = new IdentityUser { Email = userLoginDto.Email, EmailConfirmed = true };

            _userManagerMock.Setup(x => x.FindByEmailAsync(userLoginDto.Email))
                .ReturnsAsync(existedUser);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(existedUser, userLoginDto.Password))
                .ReturnsAsync(true);

            var jwtToken = "your-generated-jwt-token";
            //_authController.GenerateToken = (user) => Task.FromResult(jwtToken);

            // Act
            var result = await _authController.Login(userLoginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(jwtToken, okResult.Value);
        }

        [Fact]
            public async Task GetALL_Returns_Users()
            {
                // Arrange
                var expectedUsers = new List<User>
                {
                    new User { 
                        Id = Guid.NewGuid(), 
                        FirstName = "zhang", 
                        LastName="si", 
                        Email="zhangsi@mail.com" 
                    },
                    new User {
                        Id = Guid.NewGuid(),
                        FirstName = "li",
                        LastName="san",
                        Email="lisan@mail.com"
                    },
                };

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(repo => repo.Users.GetAllAsync())
                .ReturnsAsync(expectedUsers);

            var loggerMock = new Mock<ILogger<UsersController>>();
            var userController = new UsersController(loggerMock.Object, unitOfWorkMock.Object);

            // Act
            var result = await userController.GetALL();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(expectedUsers.Count, actualUsers.Count());
            Assert.Equal("zhangsi@mail.com", actualUsers.FirstOrDefault().Email);
            // Add more assertions if needed
        }

        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.MyUsers.CountAsync() <= 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    databaseContext.MyUsers.Add(new User()
                    {
                        Id = Guid.NewGuid(),
                        FirstName = $"san {i}",
                        LastName = "zhang",
                        Email = $"testuser{i}@mail.com",
                    });
                    await databaseContext.SaveChangesAsync();
                }
            }

            return databaseContext;
        }
    }
}