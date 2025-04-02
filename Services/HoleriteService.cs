using System.Text.RegularExpressions;
using HoleriteApi.Data;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using PdfPigUgly = UglyToad.PdfPig;


namespace HoleriteApi.Services
{
    public class HoleriteService
    {
        private readonly HoleriteDbContext _context;

        public HoleriteService(HoleriteDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SalvarHoleriteAsync(IFormFile arquivoPdf)
        {
            try
            {
                var pastaSaida = "PdfsSeparados";
                Directory.CreateDirectory(pastaSaida);

                var mapa = new Dictionary<string, List<int>>();
                
                

                using var pdfTexto = PdfPigUgly.PdfDocument.Open(arquivoPdf.OpenReadStream());
                for (int i = 0; i < pdfTexto.NumberOfPages; i++)
                {
                    var textoPagina = pdfTexto.GetPage(i + 1).Text;
                    var nomeFuncionario = ExtrairNomeFuncionario(textoPagina);

                    if (!mapa.ContainsKey(nomeFuncionario))
                        mapa[nomeFuncionario] = new List<int>();

                    mapa[nomeFuncionario].Add(i);
                }

                using var pdfOriginal = new PdfDocument(new PdfReader(arquivoPdf.OpenReadStream()));

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

                ArmazenarHoleritesNoBancoDeDados(mapa, pastaSaida, pdfOriginal);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
        }
        
        private  void ArmazenarHoleritesNoBancoDeDados(Dictionary<string, List<int>> mapa, string pastaSaida, PdfDocument pdfOriginal)
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

           
            var funcionario = _context.Funcionarios.FirstOrDefault(f => f.Nome == nomeFuncionario); 
           
            if (funcionario == null)
            {
                Console.WriteLine($"Funcionário {nomeFuncionario} não encontrado no banco de dados.");
                continue;
            }
            
            var holerite = new Holerite
            {
                FuncionarioId = funcionario.Id,
                NomeFuncionarioExtraido = nomeArquivo,
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

        public async Task<List<Holerite>> ConsultarHoleritesAsync(string cpf, DateTime dataNascimento)
        {
            var funcionario = await _context.Funcionarios
                .Include(f => f.Holerites)
                .FirstOrDefaultAsync(f => f.Cpf == cpf && f.DataNascimento == dataNascimento);

            return funcionario?.Holerites.ToList() ?? new List<Holerite>();
        }
        
        public async Task<List<Holerite>> ObterTodosHoleritesAsync()
        {
            return await _context.Holerites.ToListAsync();
        }

        public async Task<Holerite?> ObterHoleritePorIdAsync(int id)
        {
            return await _context.Holerites.FindAsync(id);
        }
    }
}