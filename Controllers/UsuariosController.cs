using HoleriteApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HoleriteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Apenas Admin pode acessar
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
            u.UserName,
            u.Email,
            u.Nome,
            u.Cpf,
            u.Cargo,
            u.Empresa,
            u.Departamento,
            u.DataAdmissao,
            u.DataDemissao
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
            user.UserName,
            user.Email,
            user.Nome,
            user.Cpf,
            user.Cargo,
            user.Empresa,
            user.Departamento,
            user.DataAdmissao,
            user.DataDemissao
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] ApplicationUser updated)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Atualiza campos
        user.Nome = updated.Nome;
        user.Email = updated.Email;
        user.Cargo = updated.Cargo;
        user.Departamento = updated.Departamento;
        user.Empresa = updated.Empresa;
        user.DataAdmissao = updated.DataAdmissao;
        user.DataDemissao = updated.DataDemissao;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok("Usuário atualizado com sucesso.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok("Usuário excluído com sucesso.");
    }

    [HttpPost("{id}/role")]
    public async Task<IActionResult> ChangeRole(string id, [FromBody] string newRole)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await _userManager.AddToRoleAsync(user, newRole);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok("Role alterada com sucesso.");
    }
}
