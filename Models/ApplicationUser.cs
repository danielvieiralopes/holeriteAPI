using Microsoft.AspNetCore.Identity;

namespace HoleriteApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Matricula { get; set; }
        public string Cargo { get; set; }
        public string Departamento { get; set; }
        public string Empresa { get; set; }
        public DateTime DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public string EmailCorporativo { get; set; }
    }
}
