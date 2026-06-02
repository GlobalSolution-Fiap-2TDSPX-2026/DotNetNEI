using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;
using NEI.Services;

namespace NEI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly NasaIntegrationService _nasaService;
        private readonly AppDbContext _context;

        public AdminController(NasaIntegrationService nasaService, AppDbContext context)
        {
            _nasaService = nasaService;
            _context = context;
        }

        [HttpGet("sync-nasa")]
        public async Task<IActionResult> ForceSync()
        {
            try
            {
                // Busca os asteroides de hoje até os próximos 7 dias
                var startDate = DateTime.Today;
                var endDate = DateTime.Today.AddDays(7);

                await _nasaService.SyncAsteroidsAsync(startDate, endDate);

                return Ok("Sincronização com a NASA concluída com sucesso!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao sincronizar: {ex.Message}");
            }
        }

        [HttpDelete("reset")]
        public async Task<IActionResult> DeleteAll()
        {
            // ZERA OS ALERTAS DAS ZONAS (Sem deletar as cidades cadastradas)
            await _context.RiskZones
                .ExecuteUpdateAsync(s => s
                    .SetProperty(z => z.RiskAssessmentId, (int?)null)
                    .SetProperty(z => z.AlertLevel, (AlertLevel?)null)
                    .SetProperty(z => z.RadiusKm, (decimal?)null));

            await _context.RiskAssessments.ExecuteDeleteAsync();
            await _context.CloseApproaches.ExecuteDeleteAsync();
            await _context.Asteroids.ExecuteDeleteAsync();

            return NoContent();
        }

    }
}