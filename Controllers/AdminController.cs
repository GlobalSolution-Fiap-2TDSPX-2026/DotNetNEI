using Microsoft.AspNetCore.Mvc;
using NEI.Services;

namespace NEI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly NasaIntegrationService _nasaService;

        public AdminController(NasaIntegrationService nasaService)
        {
            _nasaService = nasaService;
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
    }
}