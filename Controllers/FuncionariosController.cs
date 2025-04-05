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
        
        // Verifica se o CPF já está cadastrado
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
            
            // Verifica se o CPF já está cadastrado
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
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ListarFuncionarios()
    {
        var funcionarios = await _context.Funcionarios.ToListAsync();
        return Ok(funcionarios);
    }
    
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