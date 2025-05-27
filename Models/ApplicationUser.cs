using HoleriteApi.Models.Enum;
using Microsoft.AspNetCore.Identity;

namespace HoleriteApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NomeFuncionario { get; set; }
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool PrecisaTrocarSenha { get; set; } = true;
        public bool UsuarioAtivo { get; set; } = true;
        public ERoles TipoUsuario { get; set; }

        public ICollection<Holerite> Holerites { get; set; }
    }
}
