using BlazorCRUDWebApi.Server.Data;
using BlazorCRUDWebApi.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorCRUDWebApi.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DriversController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Driver>>> GetDrivers()
        {
            var drivers = await _db.Drivers.ToListAsync();

            return Ok(drivers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            var driver = await _db.Drivers.FindAsync(id);

            if (driver == null)
                return NotFound("The driver not found");

            return Ok(driver);
        }

        [HttpPost]
        public async Task<IActionResult> AddDriver([FromBody]Driver newDriver)
        {
            _db.Drivers.Add(newDriver); 
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDriver), new { newDriver.Id }, newDriver);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver([FromBody]Driver updDriver, int id)
        {
            var driver = await _db.Drivers.FindAsync(id);

            if(driver == null)
                return NotFound();

            driver.Name = updDriver.Name;
            driver.RacingNb = updDriver.RacingNb;
            driver.Team = updDriver.Team;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveDriver(int id)
        {
            var driver = await _db.Drivers.FindAsync(id);

            if(driver == null)
                return NotFound();

            _db.Drivers.Remove(driver);
            await _db.SaveChangesAsync();

            return NoContent() ;
        }
    }           
}
