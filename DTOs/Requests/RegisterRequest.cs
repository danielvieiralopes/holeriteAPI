using HoleriteApi.Models.Enum;

namespace HoleriteApi.Requests;

public class RegisterRequest
{
    public string Cpf { get; set; }
    public string Senha { get; set; }
    public ERoles Role { get; set; } 
}
