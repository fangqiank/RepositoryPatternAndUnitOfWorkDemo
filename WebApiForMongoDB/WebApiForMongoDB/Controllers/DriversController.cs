using Microsoft.AspNetCore.Mvc;
using WebApiForMongoDB.Models;
using WebApiForMongoDB.Services;

namespace WebApiForMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ILogger<DriversController> _logger;
        private readonly IDriverService _service;

        public DriversController(ILogger<DriversController> logger, IDriverService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<Driver>> GetDrivers()
        {
            var drivers = await _service.GetAsync();
            
            return Ok(drivers);
        }

    }
}
