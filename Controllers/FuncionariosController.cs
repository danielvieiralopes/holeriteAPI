using HoleriteApi.Controllers.Requests;
using HoleriteApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoleriteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncionariosController : ControllerBase
{
    private readonly HoleriteDbContext _context;

    public FuncionariosController(HoleriteDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cadastra um novo funcionário.
    /// </summary>
    /// <param name="funcionario">Dados do funcionário a ser cadastrado.</param>
    /// <returns>O funcionário cadastrado ou mensagem de erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/funcionarios
    /// Content-Type: application/json
    /// {
    ///   "nome": "João Silva",
    ///   "cpf": "12345678900",
    ///   "dataNascimento": "1990-01-01T00:00:00"
    /// }
    /// 
    /// Exemplo de response (sucesso):
    /// {
    ///   "id": 1,
    ///   "nome": "João Silva",
    ///   "cpf": "12345678900",
    ///   "dataNascimento": "1990-01-01T00:00:00"
    /// }
    /// 
    /// Exemplo de response (erro):
    /// "Funcionário já cadastrado com esse CPF."
    /// </remarks>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CadastrarFuncionario([FromBody] FuncionarioRequest funcionario)
    {
        var f = new Funcionario
        {
            Nome = funcionario.Nome,
            Cpf = funcionario.Cpf,
            DataNascimento = funcionario.DataNascimento
        };

        var funcionarioExistente = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Cpf == funcionario.Cpf);

        if (funcionarioExistente != null)
        {
            return BadRequest("Funcionário já cadastrado com esse CPF.");
        }

        _context.Funcionarios.Add(f);
        await _context.SaveChangesAsync();
        return Ok(f);
    }

    /// <summary>
    /// Cadastra uma lista de funcionários.
    /// </summary>
    /// <param name="funcionarios">Lista de funcionários a serem cadastrados.</param>
    /// <returns>Lista de funcionários cadastrados ou mensagem de erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/funcionarios/cadastrar-lista
    /// Content-Type: application/json
    /// [
    ///   {
    ///     "nome": "João Silva",
    ///     "cpf": "12345678900",
    ///     "dataNascimento": "1990-01-01T00:00:00"
    ///   },
    ///   {
    ///     "nome": "Maria Souza",
    ///     "cpf": "98765432100",
    ///     "dataNascimento": "1985-05-10T00:00:00"
    ///   }
    /// ]
    /// 
    /// Exemplo de response (sucesso):
    /// [
    ///   {
    ///     "id": 1,
    ///     "nome": "João Silva",
    ///     "cpf": "12345678900",
    ///     "dataNascimento": "1990-01-01T00:00:00"
    ///   },
    ///   {
    ///     "id": 2,
    ///     "nome": "Maria Souza",
    ///     "cpf": "98765432100",
    ///     "dataNascimento": "1985-05-10T00:00:00"
    ///   }
    /// ]
    /// 
    /// Exemplo de response (erro):
    /// "Funcionário Maria Souza já cadastrado com esse CPF."
    /// </remarks>
    [Authorize]
    [HttpPost("cadastrar-lista")]
    public async Task<IActionResult> CadastrarListaFuncionarios([FromBody] List<FuncionarioRequest> funcionarios)
    {
        var funcionariosCadastrados = new List<Funcionario>();

        foreach (var funcionario in funcionarios)
        {
            var f = new Funcionario
            {
                Nome = funcionario.Nome,
                Cpf = funcionario.Cpf,
                DataNascimento = funcionario.DataNascimento
            };

            var funcionarioExistente = await _context.Funcionarios
                .FirstOrDefaultAsync(f => f.Cpf == funcionario.Cpf);

            if (funcionarioExistente != null)
            {
                return BadRequest($"Funcionário {funcionario.Nome} já cadastrado com esse CPF.");
            }

            funcionariosCadastrados.Add(f);
        }

        _context.Funcionarios.AddRange(funcionariosCadastrados);
        await _context.SaveChangesAsync();
        return Ok(funcionariosCadastrados);
    }

    /// <summary>
    /// Lista todos os funcionários cadastrados.
    /// </summary>
    /// <returns>Lista de funcionários.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// GET /api/funcionarios
    /// 
    /// Exemplo de response:
    /// [
    ///   {
    ///     "id": 1,
    ///     "nome": "João Silva",
    ///     "cpf": "12345678900",
    ///     "dataNascimento": "1990-01-01T00:00:00"
    ///   },
    ///   {
    ///     "id": 2,
    ///     "nome": "Maria Souza",
    ///     "cpf": "98765432100",
    ///     "dataNascimento": "1985-05-10T00:00:00"
    ///   }
    /// ]
    /// </remarks>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ListarFuncionarios()
    {
        var funcionarios = await _context.Funcionarios.ToListAsync();
        return Ok(funcionarios);
    }

    /// <summary>
    /// Obtém um funcionário pelo ID.
    /// </summary>
    /// <param name="id">ID do funcionário.</param>
    /// <returns>O funcionário encontrado ou NotFound.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// GET /api/funcionarios/1
    /// 
    /// Exemplo de response (sucesso):
    /// {
    ///   "id": 1,
    ///   "nome": "João Silva",
    ///   "cpf": "12345678900",
    ///   "dataNascimento": "1990-01-01T00:00:00"
    /// }
    /// 
    /// Exemplo de response (erro):
    /// 404 Not Found
    /// </remarks>
    [HttpGet("{id}")]
    public async Task<IActionResult> ObterFuncionario(int id)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null)
        {
            return NotFound();
        }
        return Ok(funcionario);
    }

    /// <summary>
    /// Atualiza os dados de um funcionário.
    /// </summary>
    /// <param name="id">ID do funcionário.</param>
    /// <param name="funcionario">Novos dados do funcionário.</param>
    /// <returns>O funcionário atualizado ou NotFound.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// PUT /api/funcionarios/1
    /// Content-Type: application/json
    /// {
    ///   "nome": "João Silva Atualizado",
    ///   "cpf": "12345678900",
    ///   "dataNascimento": "1990-01-01T00:00:00"
    /// }
    /// 
    /// Exemplo de response (sucesso):
    /// {
    ///   "id": 1,
    ///   "nome": "João Silva Atualizado",
    ///   "cpf": "12345678900",
    ///   "dataNascimento": "1990-01-01T00:00:00"
    /// }
    /// 
    /// Exemplo de response (erro):
    /// 404 Not Found
    /// </remarks>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarFuncionario(int id, [FromBody] FuncionarioRequest funcionario)
    {
        var funcionarioExistente = await _context.Funcionarios.FindAsync(id);
        if (funcionarioExistente == null)
        {
            return NotFound();
        }

        funcionarioExistente.Nome = funcionario.Nome;
        funcionarioExistente.Cpf = funcionario.Cpf;
        funcionarioExistente.DataNascimento = funcionario.DataNascimento;

        _context.Funcionarios.Update(funcionarioExistente);
        await _context.SaveChangesAsync();
        return Ok(funcionarioExistente);
    }

    /// <summary>
    /// Remove um funcionário pelo ID.
    /// </summary>
    /// <param name="id">ID do funcionário.</param>
    /// <returns>NoContent se removido ou NotFound.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// DELETE /api/funcionarios/1
    /// 
    /// Exemplo de response (sucesso):
    /// 204 No Content
    /// 
    /// Exemplo de response (erro):
    /// 404 Not Found
    /// </remarks>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarFuncionario(int id)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null)
        {
            return NotFound();
        }

        _context.Funcionarios.Remove(funcionario);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Obtém um funcionário pelo CPF.
    /// </summary>
    /// <param name="cpf">CPF do funcionário.</param>
    /// <returns>O funcionário encontrado ou NotFound.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// GET /api/funcionarios/cpf/12345678900
    /// 
    /// Exemplo de response (sucesso):
    /// {
    ///   "id": 1,
    ///   "nome": "João Silva",
    ///   "cpf": "12345678900",
    ///   "dataNascimento": "1990-01-01T00:00:00"
    /// }
    /// 
    /// Exemplo de response (erro):
    /// 404 Not Found
    /// </remarks>
    [Authorize]
    [HttpGet("cpf/{cpf}")]
    public async Task<IActionResult> ObterFuncionarioPorCpf(string cpf)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Cpf == cpf);
        if (funcionario == null)
        {
            return NotFound();
        }
        return Ok(funcionario);
    }

    /// <summary>
    /// Obtém um funcionário pelo nome (busca parcial).
    /// </summary>
    /// <param name="nome">Nome do funcionário.</param>
    /// <returns>O funcionário encontrado ou NotFound.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// GET /api/funcionarios/nome/João
    /// 
    /// Exemplo de response (sucesso):
    /// {
    ///   "id": 1,
    ///   "nome": "João Silva",
    ///   "cpf": "12345678900",
    ///   "dataNascimento": "1990-01-01T00:00:00"
    /// }
    /// 
    /// Exemplo de response (erro):
    /// 404 Not Found
    /// </remarks>
    [Authorize]
    [HttpGet("nome/{nome}")]
    public async Task<IActionResult> ObterFuncionarioPorNome(string nome)
    {
        var funcionario = await _context.Funcionarios
            .FirstOrDefaultAsync(f => f.Nome.Contains(nome));
        if (funcionario == null)
        {
            return NotFound();
        }
        return Ok(funcionario);
    }
}
