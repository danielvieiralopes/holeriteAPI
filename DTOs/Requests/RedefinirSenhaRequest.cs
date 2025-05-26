namespace HoleriteApi.DTOs.Requests
{
    public class RedefinirSenhaRequest
    {
        public string Cpf { get; set; }
        public string NovaSenha { get; set; }
        public string Token { get; set; }
    }
}
