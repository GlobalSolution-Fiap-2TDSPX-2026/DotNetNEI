using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;
using NEI.Services;

namespace NEI.Controllers
{
    /// <summary>
    /// Controller responsável por operações administrativas do sistema NEI,
    /// como sincronização manual com a API da NASA e reset de dados.
    /// </summary>
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

        /// <summary>
        /// Força a sincronização dos asteroides com a API da NASA.
        /// </summary>
        /// <remarks>
        /// Busca todos os objetos próximos da Terra (NEOs) para o intervalo de hoje até os próximos 7 dias,
        /// atualizando asteroides, aproximações e avaliações de risco no banco de dados.
        /// </remarks>
        /// <returns>Mensagem de confirmação quando a sincronização é concluída com sucesso.</returns>
        /// <response code="200">Sincronização concluída com sucesso.</response>
        /// <response code="500">Erro interno ao tentar comunicar com a API da NASA ou ao persistir os dados.</response>
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

        /// <summary>
        /// Reseta os dados de alerta do sistema, removendo todos os asteroides e limpando os alertas das zonas de risco.
        /// </summary>
        /// <remarks>
        /// Esta operação é destrutiva e irreversível. Ela:
        /// - Remove todos os registros de asteroides (e, em cascata, suas aproximações e avaliações de risco).
        /// - Zera os campos <c>RiskAssessmentId</c>, <c>AlertLevel</c> e <c>RadiusKm</c> de todas as zonas de risco cadastradas,
        ///   sem excluir as zonas em si.
        /// </remarks>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Reset realizado com sucesso. Nenhum conteúdo é retornado.</response>
        [HttpDelete("reset")]
        public async Task<IActionResult> DeleteAll()
        {
            // ZERA OS ALERTAS DAS ZONAS (Sem deletar as cidades cadastradas)
            await _context.RiskZones
                .ExecuteUpdateAsync(s => s
                    .SetProperty(z => z.RiskAssessmentId, (int?)null)
                    .SetProperty(z => z.AlertLevel, (AlertLevel?)null)
                    .SetProperty(z => z.RadiusKm, (decimal?)null));

            await _context.Asteroids.ExecuteDeleteAsync();

            return NoContent();
        }

    }
}