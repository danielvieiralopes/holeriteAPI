public class Holerite
{
    public int Id { get; set; }
    public string NomeFuncionarioExtraido { get; set; }
    public DateTime DataReferencia { get; set; }
    public byte[] ArquivoPdf { get; set; }

    public int? FuncionarioId { get; set; }
    public Funcionario Funcionario { get; set; }
}