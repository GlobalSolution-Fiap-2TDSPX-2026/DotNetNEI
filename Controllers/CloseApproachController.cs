using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;

namespace NEI
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de aproximações de asteroides à Terra (Close Approaches).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CloseApproachController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CloseApproachController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna a lista de todas as aproximações registradas.
        /// </summary>
        /// <returns>Uma lista com todas as aproximações presentes no banco de dados.</returns>
        /// <response code="200">Lista retornada com sucesso. Pode ser uma lista vazia.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var approaches = await _context.CloseApproaches.ToListAsync();
            return Ok(approaches);
        }

        /// <summary>
        /// Retorna uma aproximação específica pelo seu identificador interno.
        /// </summary>
        /// <param name="id">Identificador único da aproximação.</param>
        /// <returns>A aproximação correspondente ao ID informado.</returns>
        /// <response code="200">Aproximação encontrada e retornada com sucesso.</response>
        /// <response code="404">Nenhuma aproximação encontrada com o ID informado.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var approach = await _context.CloseApproaches.FindAsync(id);
            if (approach == null) return NotFound();
            return Ok(approach);
        }

         /// <summary>
        /// Cria um novo registro de aproximação de asteroide.
        /// </summary>
        /// <remarks>
        /// O campo <c>AsteroidId</c> deve corresponder a um asteroide existente no banco de dados.
        /// </remarks>
        /// <param name="request">Dados da aproximação a ser criada.</param>
        /// <returns>A aproximação recém-criada com seu ID gerado.</returns>
        /// <response code="201">Aproximação criada com sucesso.</response>
        /// <response code="400">Dados inválidos na requisição.</response>
        [HttpPost]
        public async Task<IActionResult> Create(CloseApproachRequest request)
        {
            var closeApproach = request.ToEntity();
            _context.Add(closeApproach);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = closeApproach.Id }, closeApproach);
        }

        /// <summary>
        /// Atualiza um registro de aproximação existente.
        /// </summary>
        /// <remarks>
        /// Todos os campos são substituídos pelos valores fornecidos no corpo da requisição.
        /// </remarks>
        /// <param name="id">Identificador único da aproximação a ser atualizada.</param>
        /// <param name="updatedApproach">Novos dados da aproximação.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Aproximação atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos na requisição.</response>
        /// <response code="404">Nenhuma aproximação encontrada com o ID informado.</response>
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

        /// <summary>
        /// Remove um registro de aproximação pelo seu identificador interno.
        /// </summary>
        /// <param name="id">Identificador único da aproximação a ser removida.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Aproximação removida com sucesso.</response>
        /// <response code="404">Nenhuma aproximação encontrada com o ID informado.</response>
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
