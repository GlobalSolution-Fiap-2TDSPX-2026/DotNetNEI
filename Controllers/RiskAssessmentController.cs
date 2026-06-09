using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;

namespace NEI
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de avaliações de risco associadas a asteroides.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RiskAssessmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RiskAssessmentController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna a lista de todas as avaliações de risco cadastradas.
        /// </summary>
        /// <returns>Uma lista com todas as avaliações de risco presentes no banco de dados.</returns>
        /// <response code="200">Lista retornada com sucesso. Pode ser uma lista vazia.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assessments = await _context.RiskAssessments.ToListAsync();
            return Ok(assessments);
        }

        /// <summary>
        /// Retorna uma avaliação de risco específica pelo seu identificador interno.
        /// </summary>
        /// <param name="id">Identificador único da avaliação de risco.</param>
        /// <returns>A avaliação de risco correspondente ao ID informado.</returns>
        /// <response code="200">Avaliação encontrada e retornada com sucesso.</response>
        /// <response code="404">Nenhuma avaliação encontrada com o ID informado.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var assessment = await _context.RiskAssessments.FindAsync(id);
            if (assessment == null) return NotFound($"Avaliação de risco de id {id} não encontrada!");
            return Ok(assessment);
        }

        /// <summary>
        /// Cria uma nova avaliação de risco manualmente.
        /// </summary>
        /// <remarks>
        /// Normalmente as avaliações são geradas automaticamente pela sincronização com a NASA.
        /// Este endpoint permite a criação manual para fins administrativos ou de teste.
        /// Os níveis de risco disponíveis são: <c>LOW</c>, <c>MEDIUM</c>, <c>HIGH</c> e <c>CRITICAL</c>.
        /// </remarks>
        /// <param name="request">Dados da avaliação de risco a ser criada.</param>
        /// <returns>A avaliação de risco recém-criada com seu ID gerado.</returns>
        /// <response code="201">Avaliação de risco criada com sucesso.</response>
        /// <response code="400">Dados inválidos na requisição.</response>
        [HttpPost]
        public async Task<IActionResult> Create(RiskAssessmentRequest request)
        {
            var riskAssessment = request.ToEntity();
            _context.Add(riskAssessment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = riskAssessment.Id }, riskAssessment);
        }

        /// <summary>
        /// Atualiza uma avaliação de risco existente.
        /// </summary>
        /// <remarks>
        /// Todos os campos são substituídos pelos valores fornecidos no corpo da requisição.
        /// </remarks>
        /// <param name="id">Identificador único da avaliação de risco a ser atualizada.</param>
        /// <param name="updatedAssessment">Novos dados da avaliação.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Avaliação atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos na requisição.</response>
        /// <response code="404">Nenhuma avaliação encontrada com o ID informado.</response>
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

        /// <summary>
        /// Remove uma avaliação de risco pelo seu identificador interno.
        /// </summary>
        /// <param name="id">Identificador único da avaliação de risco a ser removida.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Avaliação removida com sucesso.</response>
        /// <response code="404">Nenhuma avaliação encontrada com o ID informado.</response>
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
