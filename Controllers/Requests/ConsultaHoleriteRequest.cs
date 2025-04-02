using HoleriteApi.Models.Enum;

namespace HoleriteApi.Controllers.Requests;

public class ConsultaHoleriteRequest
{
    public string Cpf { get; set; }
    public DateTime DataNascimento { get; set; }
    public ETipoHolerite TipoHolerite { get; set; }
    public int MesReferencia { get; set; }
    public int AnoReferencia { get; set; }
}