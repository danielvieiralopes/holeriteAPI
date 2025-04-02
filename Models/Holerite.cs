using HoleriteApi.Models.Enum;

public class Holerite
{
    public int Id { get; set; }
    public string NomeFuncionarioExtraido { get; set; }
    public DateTime DataUpload { get; set; } 
    public int MesReferencia { get; set; }
    public int AnoReferencia { get; set; }
    public ETipoHolerite TipoHolerite { get; set; }
    public byte[] ArquivoPdf { get; set; }

    public int? FuncionarioId { get; set; }
    public Funcionario Funcionario { get; set; }
}