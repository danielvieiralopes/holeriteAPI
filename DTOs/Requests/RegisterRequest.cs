using HoleriteApi.Models.Enum;

namespace HoleriteApi.Requests;

public class RegisterRequest
{
    public string NomeFuncionario { get; set; }
    public string Cpf { get; set; }
    public DateTime DataNascimento { get; set; }
    public ERoles tipoUsuario { get; set; } 
}
