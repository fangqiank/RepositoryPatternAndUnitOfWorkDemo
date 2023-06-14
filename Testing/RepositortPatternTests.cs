using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryPatternAndUnitOfWork.Controllers;
using RepositoryPatternAndUnitOfWork.Core.IConfiguration;
using RepositoryPatternAndUnitOfWork.Data;
using RepositoryPatternAndUnitOfWork.Models;

namespace Testing
{

    public class RepositortPatternTests
    {
        
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