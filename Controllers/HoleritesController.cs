using HoleriteApi.Controllers.Requests;
using HoleriteApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HoleriteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HoleritesController : ControllerBase
{
    private readonly HoleriteService _holeriteService;

    public HoleritesController(HoleriteService holeriteService)
    {
        _holeriteService = holeriteService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadHolerite([FromForm] UploadHoleriteRequest request)
    {
        try
        {
            
            if (request.arquivoPdf == null || request.arquivoPdf.Length == 0)
                return BadRequest("Arquivo PDF não enviado.");
            if (request.arquivoPdf.ContentType != "application/pdf")
                return BadRequest("Formato de arquivo inválido. Apenas PDF é aceito.");
            
            var sucesso = await _holeriteService.SalvarHoleriteAsync(request.arquivoPdf);
            return sucesso ? Ok("Holerite salvo.") : BadRequest("Falha no upload.");
        }
        catch (Exception e)
        {
            
            Console.WriteLine(e.Message);
            throw;
        }
    }

    [HttpGet("consulta")]
    public async Task<IActionResult> ConsultarHolerites(string cpf, DateTime dataNascimento)
    {
        var holerites = await _holeriteService.ConsultarHoleritesAsync(cpf, dataNascimento);
        if (!holerites.Any())
            return NotFound("Nenhum holerite encontrado.");

        return Ok(holerites.Select(h => new { h.Id, h.DataReferencia }));
    }

    [HttpGet("{id}/arquivo")]
    public async Task<IActionResult> Download(int id)
    {
        var holerite = await _holeriteService.ObterHoleritePorIdAsync(id);
        if (holerite == null) return NotFound();

        return File(holerite.ArquivoPdf, "application/pdf", $"Holerite_{id}.pdf");
    }
    
    
    [HttpGet("todos")]
    public async Task<IActionResult> ObterTodosHolerites()
    {
        var holerites = await _holeriteService.ObterTodosHoleritesAsync();
        if (!holerites.Any())
            return NotFound("Nenhum holerite encontrado.");

        return Ok(holerites.Select(h => new { h.Id,h.FuncionarioId, h.NomeFuncionarioExtraido, h.DataReferencia }));
    }
}