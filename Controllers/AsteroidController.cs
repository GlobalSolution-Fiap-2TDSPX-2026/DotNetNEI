using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;
using NEI.Models;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var asteroid = await _context.Asteroids.FindAsync(id);
            if (asteroid == null) return NotFound();
            return Ok(asteroid);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Asteroid>>> GetByName(string name)
        {
            var asteroids = await _context.Asteroids
                .Where(a => a.Name.Contains(name))
                .ToListAsync();

            return Ok(asteroids);
        }

    }
}
