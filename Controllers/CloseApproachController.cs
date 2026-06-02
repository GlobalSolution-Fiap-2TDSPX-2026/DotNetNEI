using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;

namespace NEI
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloseApproachController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CloseApproachController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var approaches = await _context.CloseApproaches.ToListAsync();
            return Ok(approaches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var approach = await _context.CloseApproaches.FindAsync(id);
            if (approach == null) return NotFound();
            return Ok(approach);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CloseApproachRequest request)
        {
            var closeApproach = request.ToEntity();
            _context.Add(closeApproach);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = closeApproach.Id }, closeApproach);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CloseApproachRequest updatedApproach)
        {
            var approach = await _context.CloseApproaches.FindAsync(id);

            if (approach == null) return NotFound();

            approach.Update(
                updatedApproach.AsteroidId,
                updatedApproach.ApproachDate,
                updatedApproach.MissDistanceKm,
                updatedApproach.RelativeVelocityKm,
                updatedApproach.OrbitingBody);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var approach = await _context.CloseApproaches.FindAsync(id);

            if (approach == null) return NotFound($"Aproximação de id {id} não encontrada");

            _context.CloseApproaches.Remove(approach);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
