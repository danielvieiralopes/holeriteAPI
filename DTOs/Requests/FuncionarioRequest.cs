namespace HoleriteApi.Controllers.Requests;

public class FuncionarioRequest
{
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public DateTime DataNascimento { get; set; }
}