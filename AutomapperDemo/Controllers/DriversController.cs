using AutoMapper;
using AutomapperDemo.Models;
using AutomapperDemo.Models.Dtos.Incoming;
using AutomapperDemo.Models.Dtos.Outgoing;
using Microsoft.AspNetCore.Mvc;

namespace AutomapperDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ILogger<DriversController> _logger;
        private readonly IMapper _mapper;
        private static List<Driver> _drivers = new List<Driver>();

        public DriversController(ILogger<DriversController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetDrivers()
        {
            var results = _drivers.Where(x => x.Status == 1).ToList();

            var driversDto = _mapper.Map<IEnumerable<DriverDto>>(results); 

            return Ok(driversDto);
        }

        [HttpPost]
        public IActionResult AddDriver(CreateDto newDriver)
        {
            if (ModelState.IsValid)
            {
                //var driver = new Driver
                //{
                //    Id = Guid.NewGuid(),
                //    Status = 1,
                //    DateAdded = DateTime.Now,
                //    DateUpdated = DateTime.Now,
                //    DriverNumber = newDriver.DriverNumber,
                //    FirstName = newDriver.FirstName,
                //    LastName = newDriver.LastName,
                //    WorldChampionship = newDriver.WorldChampionship,
                //};

                var driver = _mapper.Map<Driver>(newDriver);
                
                _drivers.Add(driver);

                return CreatedAtAction(nameof(GetDriver), new { driver.Id }, driver);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }

        [HttpGet("{id}")]
        public IActionResult GetDriver(Guid id)
        {
            var driver = _drivers.FirstOrDefault(x => x.Id == id);

            if (driver == null)
                return NotFound();

            return Ok(driver);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDriver(Guid id, Driver updDriver)
        {
            if (id == updDriver.Id)
                return BadRequest();

            var existingDriver = _drivers.FirstOrDefault(x => x.Id == updDriver.Id);

            if (existingDriver == null)
                return NotFound();

            existingDriver.DriverNumber = updDriver.DriverNumber;
            existingDriver.FirstName = updDriver.FirstName;
            existingDriver.LastName = updDriver.LastName;
            existingDriver.WorldChampionship = updDriver.WorldChampionship;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDriver(Guid id)
        {
            var driver = _drivers.FirstOrDefault(y => y.Id == id);

            if(driver == null)
                return NotFound();

            _drivers.Remove(driver);

            return NoContent();
        }

    }
}
