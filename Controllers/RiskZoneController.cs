using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _context.RiskZones.FindAsync(id);
            return Ok(zone);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RiskZone riskZone)
        {
            _context.Add(riskZone);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = riskZone.Id }, riskZone);
        }

    }
}
