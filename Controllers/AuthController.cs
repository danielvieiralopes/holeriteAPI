using HoleriteApi.Controllers.Requests;
using HoleriteApi.Models;
using HoleriteApi.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginRequest = HoleriteApi.Controllers.Requests.LoginRequest;
using RegisterRequest = HoleriteApi.Requests.RegisterRequest;
using ForgotPasswordRequest = HoleriteApi.DTOs.Requests.EsqueciMinhaSenhaRequest;
using ResetPasswordRequest = HoleriteApi.DTOs.Requests.RedefinirSenhaRequest;

namespace HoleriteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<ApplicationUser> userManager,
                          SignInManager<ApplicationUser> signInManager,
                          IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    /// <summary>
    /// Registra um novo usuário no sistema.
    /// </summary>
    /// <param name="request">Dados para registro do usuário.</param>
    /// <returns>Mensagem de sucesso ou erros de validação.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/auth/register
    /// Content-Type: application/json
    /// {
    ///   "cpf": "12345678900",
    ///   "senha": "SenhaForte@123",
    ///   "email": "usuario@email.com",
    ///   "role": "Funcionario"
    /// }
    /// 
    /// Exemplo de response (sucesso):
    /// "Usuário criado com sucesso."
    /// 
    /// Exemplo de response (erro):
    /// [
    ///   { "code": "DuplicateUserName", "description": "Username '12345678900' is already taken." }
    /// ]
    /// </remarks>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser { UserName = request.Cpf, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Senha);

        if (!result.Succeeded) return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, request.Role);

        return Ok("Usuário criado com sucesso.");
    }

    /// <summary>
    /// Realiza o login do usuário e retorna um token JWT.
    /// </summary>
    /// <param name="request">Dados de login (CPF e senha).</param>
    /// <returns>Token JWT ou mensagem de erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/auth/login
    /// Content-Type: application/json
    /// {
    ///   "cpf": "12345678900",
    ///   "senha": "SenhaForte@123"
    /// }
    /// 
    /// Exemplo de response (sucesso):
    /// {
    ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    /// }
    /// 
    /// Exemplo de response (erro):
    /// "Usuário inválido."
    /// </remarks>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Cpf);
        if (user == null) return Unauthorized("Usuário inválido.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Senha, false);
        if (!result.Succeeded) return Unauthorized("Senha inválida.");

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }

    /// <summary>
    /// Altera a senha do usuário autenticado.
    /// </summary>
    /// <param name="request">Dados para alteração de senha.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/auth/change-password
    /// Content-Type: application/json
    /// {
    ///   "cpf": "12345678900",
    ///   "senhaAtual": "SenhaAntiga@123",
    ///   "novaSenha": "SenhaNova@123"
    /// }
    /// 
    /// Exemplo de response (sucesso):
    /// "Senha alterada com sucesso."
    /// 
    /// Exemplo de response (erro):
    /// [
    ///   { "code": "PasswordMismatch", "description": "Incorrect password." }
    /// ]
    /// </remarks>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var cpfLogado = User.Identity?.Name;
        if (cpfLogado == null || cpfLogado != request.Cpf)
            return Unauthorized("Você não pode alterar a senha de outro usuário.");

        var user = await _userManager.FindByNameAsync(cpfLogado);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        var result = await _userManager.ChangePasswordAsync(user, request.SenhaAtual, request.NovaSenha);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Senha alterada com sucesso.");
    }

    /// <summary>
    /// Solicita redefinição de senha para o usuário informado.
    /// </summary>
    /// <param name="request">Dados para solicitação de redefinição de senha.</param>
    /// <returns>Token de redefinição ou mensagem de erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/auth/forgot-password
    /// Content-Type: application/json
    /// {
    ///   "cpf": "12345678900"
    /// }
    /// 
    /// Exemplo de response (sucesso):
    /// {
    ///   "token": "código_do_token"
    /// }
    /// 
    /// Exemplo de response (erro):
    /// "Usuário não encontrado."
    /// </remarks>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Cpf);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Enviar por e-mail. Por enquanto, retornamos o token.
        return Ok(new { Token = token });
    }

    /// <summary>
    /// Redefine a senha do usuário utilizando o token de redefinição.
    /// </summary>
    /// <param name="request">Dados para redefinição de senha.</param>
    /// <returns>Mensagem de sucesso ou erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/auth/reset-password
    /// Content-Type: application/json
    /// {
    ///   "cpf": "12345678900",
    ///   "token": "código_do_token",
    ///   "novaSenha": "SenhaNova@123"
    /// }
    /// 
    /// Exemplo de response (sucesso):
    /// "Senha redefinida com sucesso."
    /// 
    /// Exemplo de response (erro):
    /// [
    ///   { "code": "InvalidToken", "description": "Invalid token." }
    /// ]
    /// </remarks>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Cpf);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NovaSenha);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Senha redefinida com sucesso.");
    }
}
