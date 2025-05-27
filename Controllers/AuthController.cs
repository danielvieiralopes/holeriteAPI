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
using HoleriteApi.Models.Enum;

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
    ///   "nomeFuncionario": "João da Silva",
    ///   "cpf": "12345678900",
    ///   "dataNascimento": "1990-01-01T00:00:00Z",
    ///   "role": 1 // 1 para Admin, 2 para Funcionário
    /// }
    /// </remarks>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Normaliza o CPF (remove espaços e mantém apenas números)
        var cpf = request.Cpf?.Trim();
        if (string.IsNullOrWhiteSpace(cpf))
            return BadRequest(new[] { new IdentityError { Code = "InvalidCpf", Description = "CPF é obrigatório." } });

        // Verifica se o CPF já está cadastrado como username
        var existingUser = await _userManager.FindByNameAsync(cpf);
        if (existingUser != null)
        {
            return BadRequest(new[] {
            new IdentityError {
                Code = "DuplicateUserName",
                Description = $"Já existe um usuário com o CPF '{cpf}'."
            }
        });
        }

        // Extrai o primeiro nome para compor a senha
        var primeiroNome = request.NomeFuncionario?
            .Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(primeiroNome))
            primeiroNome = "Usuario";
        else
            primeiroNome = char.ToUpper(primeiroNome[0]) + primeiroNome[1..].ToLower();

        // Gera senha inicial com base na data de nascimento + primeiro nome
        var senhaInicial = $"{request.DataNascimento:ddMMyyyy}@{primeiroNome}";

        var user = new ApplicationUser
        {
            NomeFuncionario = request.NomeFuncionario?.Trim(),
            Cpf = cpf,
            UserName = cpf,
            TipoUsuario = request.tipoUsuario,
            DataNascimento = request.DataNascimento,
            PrecisaTrocarSenha = true,
        };

        var createResult = await _userManager.CreateAsync(user, senhaInicial);
        if (!createResult.Succeeded)
            return BadRequest(createResult.Errors);

        // Define a role (Admin ou Funcionário)
        var role = Enum.GetName(typeof(ERoles), user.TipoUsuario);
        if (string.IsNullOrEmpty(role))
            return BadRequest(new[] { new IdentityError { Code = "InvalidRole", Description = "Role inválida." } });
        await _userManager.AddToRoleAsync(user, role);


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

        if (!user.UsuarioAtivo)
        {
            return BadRequest(new { mensagem = "Usuário inativo. Entre em contato com o administrador." });
        }

        var roles = await _userManager.GetRolesAsync(user);
       
        var role = roles.FirstOrDefault();

       
        ERoles tipoUsuario;
        if (role == "Admin")
        {
            tipoUsuario = ERoles.Admin;
        }
        else
        {
            tipoUsuario = ERoles.Usuario; 
        }

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
            Audience = _config["Jwt:Audience"],

        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        if (user.PrecisaTrocarSenha)
        {
            return Ok(new { Token = tokenHandler.WriteToken(token), precisaTrocarSenha = true, tipoUsuario = tipoUsuario, mensagem = "\"Usuário precisa trocar a senha antes de continuar.\"" });
        }

        return Ok(new { Token = tokenHandler.WriteToken(token), tipoUsuario = tipoUsuario });
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
        var cpfLogado = User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(cpfLogado))
            return Unauthorized("Não foi possível identificar o usuário autenticado.");

        var user = await _userManager.FindByNameAsync(cpfLogado);
        if (user == null)
            return NotFound("Usuário não encontrado.");

        var result = await _userManager.ChangePasswordAsync(user, request.SenhaAtual, request.NovaSenha);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        user.PrecisaTrocarSenha = false; 
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return BadRequest(updateResult.Errors);


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
