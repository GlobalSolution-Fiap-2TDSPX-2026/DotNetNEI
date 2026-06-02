using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;

namespace NEI
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskAssessmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RiskAssessmentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assessments = await _context.RiskAssessments.ToListAsync();
            return Ok(assessments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var assessment = await _context.RiskAssessments.FindAsync(id);
            if (assessment == null) return NotFound();
            return Ok(assessment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RiskAssessmentRequest request)
        {
            var riskAssessment = request.ToEntity();
            _context.Add(riskAssessment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = riskAssessment.Id }, riskAssessment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RiskAssessmentRequest updatedAssessment)
        {
            var assessment = await _context.RiskAssessments.FindAsync(id);

            if (assessment == null) return NotFound();

            assessment.Update(
                updatedAssessment.AsteroidId,
                updatedAssessment.RiskLevel,
                updatedAssessment.MissDistanceKm,
                updatedAssessment.SafeDistanceThresholdKm,
                updatedAssessment.AssessedAt);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var assessment = await _context.RiskAssessments.FindAsync(id);

            if (assessment == null) return NotFound($"Avaliação de risco de id {id} não encontrada");

            _context.RiskAssessments.Remove(assessment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
