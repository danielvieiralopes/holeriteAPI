using System.ComponentModel.DataAnnotations;

public class Funcionario
{
    public int Id { get; set; }
    [MaxLength(255)]
    public string Nome { get; set; }
    [MaxLength(450)]
    public string Cpf { get; set; }


    public ICollection<Holerite> Holerites { get; set; }
}