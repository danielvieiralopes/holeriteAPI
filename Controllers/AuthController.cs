using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HoleriteApi.Controllers.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // SIMULA um usuário fixo, como se estivesse no banco
        if (request.Cpf == "12345678900" && request.Senha == "1234")
        {
            var token = GerarTokenJwt("12345678900", "admin");
            return Ok(new { Token = token });
        }

        return Unauthorized("Usuário ou senha inválidos.");
    }

    private string GerarTokenJwt(string cpf, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, cpf),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}