using System.ComponentModel.DataAnnotations;
using HoleriteApi.Models.Enum;
using Microsoft.AspNetCore.Identity;

namespace HoleriteApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(255)]
        public string NomeFuncionario { get; set; }
        [MaxLength(450)]
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool PrecisaTrocarSenha { get; set; } = true;
        public bool UsuarioAtivo { get; set; } = true;
        public ERoles TipoUsuario { get; set; }

        public ICollection<Holerite>? Holerites { get; set; }
    }
}
