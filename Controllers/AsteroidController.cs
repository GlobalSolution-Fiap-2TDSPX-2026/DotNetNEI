using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;

namespace NEI
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsteroidController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsteroidController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var asteroids = await _context.Asteroids.ToListAsync();
            return Ok(asteroids);
        }


    }
}
