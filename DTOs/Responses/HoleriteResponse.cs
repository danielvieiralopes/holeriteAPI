namespace HoleriteApi.DTOs.Responses
{
    public class HoleriteResponse
    {
        public int Id { get; set; }
        public string NomeFuncionario { get; set; }
        public string Cpf { get; set; }
        public DateTime DataUpload { get; set; }
        public int MesReferencia { get; set; }
        public int AnoReferencia { get; set; }
        public int TipoHolerite { get; set; }
    }
}
