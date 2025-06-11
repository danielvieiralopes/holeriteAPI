using System.Text.RegularExpressions;
using HoleriteApi.Controllers.Requests;
using HoleriteApi.Data;
using HoleriteApi.Models.Enum;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using PdfPigUgly = UglyToad.PdfPig;
using HoleriteApi.DTOs.Responses;

namespace HoleriteApi.Services
{
    public class HoleriteService
    {
        private readonly HoleriteDbContext _context;

        public HoleriteService(HoleriteDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SalvarHoleriteAsync(UploadHoleriteRequest request)
        {
            try
            {
                var pastaSaida = "PdfsSeparados";
                Directory.CreateDirectory(pastaSaida);

                var mapa = new Dictionary<string, List<int>>();
                
                

                using var pdfTexto = PdfPigUgly.PdfDocument.Open(request.ArquivoPdf.OpenReadStream());
                for (int i = 0; i < pdfTexto.NumberOfPages; i++)
                {
                    var textoPagina = pdfTexto.GetPage(i + 1).Text;
                    var nomeFuncionario = ExtrairNomeFuncionario(textoPagina);

                    if (!mapa.ContainsKey(nomeFuncionario))
                        mapa[nomeFuncionario] = new List<int>();

                    mapa[nomeFuncionario].Add(i);
                }

                using var pdfOriginal = new PdfDocument(new PdfReader(request.ArquivoPdf.OpenReadStream()));

                foreach (var item in mapa)
                {
                    var nomeFuncionario = item.Key;
                    var paginas = item.Value;

                    var nomeArquivo = SanitizarNome(nomeFuncionario) + ".pdf";
                    var caminhoSaida = Path.Combine(pastaSaida, nomeArquivo);

                    using var pdfWriter = new PdfWriter(caminhoSaida);
                    using var pdfDestino = new PdfDocument(pdfWriter);
                    var merger = new PdfMerger(pdfDestino);

                    foreach (var pagina in paginas)
                    {
                        merger.Merge(pdfOriginal, pagina + 1, pagina + 1);
                    }

                    Console.WriteLine($"PDF gerado para: {nomeFuncionario}");
                }

                Console.WriteLine("\nPDFs separados por funcionário com sucesso.");

                ArmazenarHoleritesNoBancoDeDados(mapa, pastaSaida, pdfOriginal, request.MesReferencia, request.AnoReferencia, request.TipoHolerite);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
        }
        
        private  void ArmazenarHoleritesNoBancoDeDados(Dictionary<string, List<int>> mapa, string pastaSaida, PdfDocument pdfOriginal, int mesReferencia, int anoReferencia, ETipoHolerite tipoHolerite)
    {
        
        foreach (var item in mapa)
        {
            var nomeFuncionario = item.Key;
            var paginas = item.Value;

            var nomeArquivo = SanitizarNome(nomeFuncionario) + ".pdf";
            var caminhoSaida = Path.Combine(pastaSaida, nomeArquivo);

            using var pdfWriter = new PdfWriter(caminhoSaida);
            using var pdfDestino = new PdfDocument(pdfWriter);
            var merger = new PdfMerger(pdfDestino);

            foreach (var pagina in paginas)
            {
                merger.Merge(pdfOriginal, pagina + 1, pagina + 1);
            }

           
            pdfDestino.Close();
            var arquivoBytes = File.ReadAllBytes(caminhoSaida);

           
            var funcionario = _context.Usuarios.FirstOrDefault(f => f.NomeFuncionario == nomeFuncionario); 
           
            if (funcionario == null)
            {
                Console.WriteLine($"Funcionário {nomeFuncionario} não encontrado no banco de dados.");
                continue;
            }
            
            var holeriteExistente = _context.Holerites
                .FirstOrDefault(h => h.UsuarioId == funcionario.Id && h.MesReferencia == mesReferencia && h.AnoReferencia == anoReferencia && tipoHolerite == h.TipoHolerite);

            if (holeriteExistente != null)
            {
                continue;
            }

                var holerite = new Holerite
                {
                    UsuarioId = funcionario.Id,
                    NomeFuncionarioExtraido = nomeArquivo,
                    MesReferencia = mesReferencia,
                    AnoReferencia = anoReferencia,
                    TipoHolerite = tipoHolerite,
                    DataUpload = DateTime.UtcNow, 
                    ArquivoPdf = arquivoBytes
                };
            
            _context.Holerites.Add(holerite);
            _context.SaveChanges();

            Console.WriteLine($"PDF de {nomeFuncionario} salvo no banco de dados.");
        }
    }

     string ExtrairNomeFuncionario(string texto)
    {
        var match = Regex.Match(texto, @"Código\s*([A-ZÀ-Ú\s]+?)\s*Nome do Funcionário", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : "Funcionario_Desconhecido";
    }

     string SanitizarNome(string nome)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            nome = nome.Replace(c, '_');
        return nome;
    }

        public async Task<List<Holerite>> ConsultarHoleritesAsync(ConsultaHoleriteRequest request)
        {
            var funcionario = await _context.Usuarios
                .Include(f => f.Holerites)
                .FirstOrDefaultAsync(f => f.Cpf == request.Cpf);

            if (funcionario == null)
                return new List<Holerite>();

            return funcionario.Holerites
                .Where(h => h.TipoHolerite == request.TipoHolerite && h.MesReferencia == request.MesReferencia && h.AnoReferencia == request.AnoReferencia)
                .ToList();
        }

        public async Task<List<HoleriteResponse>> ObterTodosHoleritesAsync()
        {

             var holerites =  await _context.Holerites.ToListAsync();

             var holeriteResponse = holerites.Select(h => new HoleriteResponse
             {
                 Id = h.Id,
                 NomeFuncionario = _context.Usuarios.FirstOrDefault(u => u.Id == h.UsuarioId )?.NomeFuncionario ?? "Funcionario não encontrado",
                 Cpf = _context.Usuarios.FirstOrDefault(u => u.Id == h.UsuarioId)?.Cpf ?? "CPF não encontrado",
                 DataUpload = h.DataUpload,
                 MesReferencia = h.MesReferencia,
                 AnoReferencia = h.AnoReferencia,
                 TipoHolerite = (int)h.TipoHolerite
             }).ToList();

            return holeriteResponse;

        }

        public async Task<Holerite?> ObterHoleritePorIdAsync(int id)
        {
            return await _context.Holerites.FindAsync(id);
        }


        public async Task<bool> ExcluirHoleriteAsync(int id)
        {
            var holerite = await _context.Holerites.FindAsync(id);
            if (holerite == null)
                return false;
            _context.Holerites.Remove(holerite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AtualizarHoleriteAsync(Holerite holerite)
        {
            var existingHolerite = await _context.Holerites.FindAsync(holerite.Id);
            if (existingHolerite == null)
                return false;


            existingHolerite.NomeFuncionarioExtraido = holerite.NomeFuncionarioExtraido;
            existingHolerite.DataUpload = holerite.DataUpload.Kind == DateTimeKind.Utc
                ? holerite.DataUpload
                : DateTime.SpecifyKind(holerite.DataUpload, DateTimeKind.Utc);
            existingHolerite.MesReferencia = holerite.MesReferencia;
            existingHolerite.AnoReferencia = holerite.AnoReferencia;
            existingHolerite.TipoHolerite = holerite.TipoHolerite;
            existingHolerite.ArquivoPdf = holerite.ArquivoPdf;
            
            _context.Holerites.Update(existingHolerite);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<byte[]> LerArquivoAsync(IFormFile arquivoPdf)
        {            
            using (var stream = new MemoryStream())
            {
                await arquivoPdf.CopyToAsync(stream);
                return stream.ToArray();
            }
        }
    }
}