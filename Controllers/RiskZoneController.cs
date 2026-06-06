using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;

namespace NEI
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de zonas de risco monitoradas pelo sistema NEI.
    /// Zonas de risco representam regiões geográficas que podem ser afetadas por asteroides classificados
    /// como <c>HIGH</c> ou <c>CRITICAL</c>.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RiskZoneController : ControllerBase
    {
        
        private readonly AppDbContext _context;

        public RiskZoneController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna a lista de todas as zonas de risco cadastradas.
        /// </summary>
        /// <remarks>
        /// Inclui zonas com e sem alerta ativo. Os campos <c>AlertLevel</c>, <c>RadiusKm</c> e
        /// <c>RiskAssessmentId</c> podem ser nulos quando nenhuma ameaça foi associada à zona.
        /// </remarks>
        /// <returns>Uma lista com todas as zonas de risco presentes no banco de dados.</returns>
        /// <response code="200">Lista retornada com sucesso. Pode ser uma lista vazia.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var zones = await _context.RiskZones.ToListAsync();
            return Ok(zones);
        }

        /// <summary>
        /// Retorna uma zona de risco específica pelo seu identificador interno.
        /// </summary>
        /// <param name="id">Identificador único da zona de risco.</param>
        /// <returns>A zona de risco correspondente ao ID informado.</returns>
        /// <response code="200">Zona de risco encontrada e retornada com sucesso.</response>
        /// <response code="404">Nenhuma zona de risco encontrada com o ID informado.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var zone = await _context.RiskZones.FindAsync(id);
            return Ok(zone);
        }

        /// <summary>
        /// Cadastra uma nova zona de risco geográfica para monitoramento.
        /// </summary>
        /// <remarks>
        /// Ao criar uma zona, os campos de alerta (<c>AlertLevel</c>, <c>RadiusKm</c>, <c>RiskAssessmentId</c>)
        /// ficam nulos até que a sincronização com a NASA identifique uma ameaça relevante para a região.
        /// Coordenadas devem seguir o padrão decimal (ex.: latitude <c>-23.55</c>, longitude <c>-46.63</c>).
        /// </remarks>
        /// <param name="request">Dados da zona de risco a ser criada.</param>
        /// <returns>A zona de risco recém-criada com seu ID gerado.</returns>
        /// <response code="201">Zona de risco criada com sucesso.</response>
        /// <response code="400">Dados inválidos na requisição.</response>
        [HttpPost]
        public async Task<IActionResult> Create(RiskZoneRequest request)
        {
            var riskZone = request.ToEntity();
            _context.Add(riskZone);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = riskZone.Id }, riskZone);
        }

        /// <summary>
        /// Atualiza os dados geográficos de uma zona de risco existente.
        /// </summary>
        /// <remarks>
        /// Apenas os campos <c>RegionName</c>, <c>Latitude</c> e <c>Longitude</c> são atualizados.
        /// Os campos de alerta (<c>AlertLevel</c>, <c>RadiusKm</c>, <c>RiskAssessmentId</c>)
        /// são gerenciados automaticamente pelo sistema e não são alterados por este endpoint.
        /// </remarks>
        /// <param name="id">Identificador único da zona de risco a ser atualizada.</param>
        /// <param name="updatedZone">Novos dados geográficos da zona.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Zona de risco atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos na requisição.</response>
        /// <response code="404">Nenhuma zona de risco encontrada com o ID informado.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RiskZoneRequest updatedZone)
        {
            var zone = await _context.RiskZones.FindAsync(id);

            if (zone == null) return NotFound();

            zone.Update(updatedZone.RegionName, updatedZone.Latitude, updatedZone.Longitude);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove uma zona de risco pelo seu identificador interno.
        /// </summary>
        /// <remarks>
        /// A remoção de uma zona de risco é permanente. Use <c>DELETE /api/admin/reset</c> caso queira
        /// apenas limpar os alertas sem excluir as zonas cadastradas.
        /// </remarks>
        /// <param name="id">Identificador único da zona de risco a ser removida.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Zona de risco removida com sucesso.</response>
        /// <response code="404">Nenhuma zona de risco encontrada com o ID informado.</response>
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
