using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;
using NEI.Models;

namespace NEI
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de asteroides registrados no sistema NEI.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AsteroidController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsteroidController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna a lista de todos os asteroides cadastrados.
        /// </summary>
        /// <returns>Uma lista com todos os asteroides presentes no banco de dados.</returns>
        /// <response code="200">Lista retornada com sucesso. Pode ser uma lista vazia.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var asteroids = await _context.Asteroids.ToListAsync();
            return Ok(asteroids);
        }

        /// <summary>
        /// Retorna um asteroide específico pelo seu identificador interno.
        /// </summary>
        /// <param name="id">Identificador único (ID interno) do asteroide.</param>
        /// <returns>O asteroide correspondente ao ID informado.</returns>
        /// <response code="200">Asteroide encontrado e retornado com sucesso.</response>
        /// <response code="404">Nenhum asteroide encontrado com o ID informado.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var asteroid = await _context.Asteroids.FindAsync(id);
            if (asteroid == null) return NotFound($"Asteroid de id {id} não encontrado!");
            return Ok(asteroid);
        }

        /// <summary>
        /// Busca asteroides cujo nome contenha o termo informado.
        /// </summary>
        /// <remarks>
        /// A busca é feita com <c>Contains</c>, sendo case-sensitive de acordo com o collation do banco Oracle.
        /// </remarks>
        /// <param name="name">Parte do nome do asteroide a ser pesquisado.</param>
        /// <returns>Lista de asteroides cujo nome contenha o termo informado.</returns>
        /// <response code="200">Busca realizada com sucesso. Pode retornar lista vazia.</response>
        [HttpGet("search")]
        public async Task<ActionResult<List<Asteroid>>> GetByName(string name)
        {
            var asteroids = await _context.Asteroids
                .Where(a => a.Name.Contains(name))
                .ToListAsync();

            return Ok(asteroids);
        }

    }
}
