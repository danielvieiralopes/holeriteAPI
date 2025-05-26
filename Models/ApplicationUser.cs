using HoleriteApi.Models.Enum;
using Microsoft.AspNetCore.Identity;

namespace HoleriteApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Cpf { get; set; }

        public ERoles TipoUsuario { get; set; }

        public ICollection<Holerite> Holerites { get; set; }
    }
}
