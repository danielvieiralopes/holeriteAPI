using HoleriteApi.Models.Enum;

namespace HoleriteApi.Controllers.Requests
{
    public class UploadHoleriteRequest
    {
        public IFormFile ArquivoPdf { get; set; } 
        public int MesReferencia { get; set; }
        public int AnoReferencia { get; set; }
        public ETipoHolerite TipoHolerite { get; set; }
        
    }
}