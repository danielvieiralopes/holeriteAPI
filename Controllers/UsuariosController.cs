using HoleriteApi.Models;
using HoleriteApi.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HoleriteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] 
public class UsuariosController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsuariosController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userManager.Users.Select(u => new {
            u.Id,
            u.NomeFuncionario,
            u.UserName,
            u.Cpf,
            u.DataNascimento,
            u.TipoUsuario
        }).ToList();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.NomeFuncionario,
            user.UserName,
            user.Cpf,
            user.DataNascimento,
            user.TipoUsuario
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser updated)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Atualiza campos
        user.UserName = updated.UserName;
        user.NomeFuncionario = updated.NomeFuncionario;
        user.Cpf = updated.Cpf;
        user.DataNascimento = updated.DataNascimento;
        user.TipoUsuario = updated.TipoUsuario;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok("Usuário atualizado com sucesso.");
    }


    [HttpPut("desativar/{id}")]
    public async Task<IActionResult> DesativarUsuario (string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        user.UsuarioAtivo = false; 
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok("Usuário desativado com sucesso.");
    }

}
