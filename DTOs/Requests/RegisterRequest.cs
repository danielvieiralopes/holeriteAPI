namespace HoleriteApi.Requests;

public class RegisterRequest
{
    public string Cpf { get; set; }
    public string Senha { get; set; }
    public string Email { get; set; }
    public string Role { get; set; } 
}
