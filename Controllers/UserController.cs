using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEI.Data;

namespace NEI
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de usuários do sistema NEI.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Retorna a lista de todos os usuários cadastrados.
        /// </summary>
        /// <returns>Uma lista com todos os usuários presentes no banco de dados.</returns>
        /// <response code="200">Lista retornada com sucesso. Pode ser uma lista vazia.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        /// <summary>
        /// Retorna um usuário específico pelo seu identificador interno.
        /// </summary>
        /// <param name="id">Identificador único do usuário.</param>
        /// <returns>O usuário correspondente ao ID informado.</returns>
        /// <response code="200">Usuário encontrado e retornado com sucesso.</response>
        /// <response code="404">Nenhum usuário encontrado com o ID informado.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        /// <remarks>
        /// Os campos <c>Username</c> e <c>Email</c> devem ser únicos no sistema.
        ///
        /// Os papéis disponíveis para o campo <c>Role</c> são: <c>ADMIN</c>, <c>ANALYST</c> e <c>VIEWER</c>.
        /// </remarks>
        /// <param name="request">Dados do usuário a ser criado.</param>
        /// <returns>O usuário recém-criado com seu ID gerado.</returns>
        /// <response code="201">Usuário criado com sucesso.</response>
        /// <response code="400">Dados inválidos ou campos obrigatórios ausentes.</response>
        /// <response code="409">Já existe um usuário com o mesmo <c>Username</c> ou <c>Email</c>.</response>
        [HttpPost]
        public async Task<IActionResult> Create(UserRequest request)
        {
            var user = request.ToEntity();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente.
        /// </summary>
        /// <remarks>
        /// Todos os campos são substituídos pelos valores fornecidos. Os valores de <c>Username</c> e
        /// <c>Email</c> devem continuar sendo únicos após a atualização.
        /// </remarks>
        /// <param name="id">Identificador único do usuário a ser atualizado.</param>
        /// <param name="updatedUser">Novos dados do usuário.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Usuário atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos na requisição.</response>
        /// <response code="404">Nenhum usuário encontrado com o ID informado.</response>
        /// <response code="409">Já existe outro usuário com o mesmo <c>Username</c> ou <c>Email</c>.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserRequest updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.Update(updatedUser.Username, updatedUser.Email, updatedUser.Role);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove um usuário pelo seu identificador interno.
        /// </summary>
        /// <param name="id">Identificador único do usuário a ser removido.</param>
        /// <returns>Sem conteúdo em caso de sucesso.</returns>
        /// <response code="204">Usuário removido com sucesso.</response>
        /// <response code="404">Nenhum usuário encontrado com o ID informado.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
