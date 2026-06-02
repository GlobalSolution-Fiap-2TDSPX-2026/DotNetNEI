using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;

namespace NEI
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskZoneController : ControllerBase
    {
        
        private readonly AppDbContext _context;

        public RiskZoneController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var zones = await _context.RiskZones.ToListAsync();
            return Ok(zones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _context.RiskZones.FindAsync(id);
            return Ok(zone);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RiskZoneRequest request)
        {
            var riskZone = request.ToEntity();
            _context.Add(riskZone);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = riskZone.Id }, riskZone);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var zone = await _context.RiskZones.FindAsync(id);

            if (zone == null) return NotFound($"Pet de id {id} não encontrado");

            _context.RiskZones.Remove(zone);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
