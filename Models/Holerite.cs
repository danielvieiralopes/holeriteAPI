using System.ComponentModel.DataAnnotations;
using HoleriteApi.Models;
using HoleriteApi.Models.Enum;

public class Holerite
{
    public int Id { get; set; }
    [MaxLength(255)]
    public string NomeFuncionarioExtraido { get; set; }
    public DateTime DataUpload { get; set; } 
    public int MesReferencia { get; set; }
    public int AnoReferencia { get; set; }
    public ETipoHolerite TipoHolerite { get; set; }
    public byte[] ArquivoPdf { get; set; }

    [MaxLength(450)]
    public string UsuarioId { get; set; }
    public ApplicationUser Usuario { get; set; }
}