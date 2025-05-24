using HoleriteApi.Controllers.Requests;
using HoleriteApi.Models.Enum;
using HoleriteApi.Services;
using Microsoft.AspNetCore.Authorization;
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


    /// <summary>
    /// Faz o upload de um holerite em formato PDF.
    /// </summary>
    /// <param name="request">Dados do upload do holerite.</param>
    /// <returns>Retorna uma mensagem de sucesso ou erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/holerites/upload
    /// Content-Type: multipart/form-data
    /// 
    /// {
    ///     "ArquivoPdf": "arquivo.pdf",
    ///     "MesReferencia": 1,
    ///     "AnoReferencia": 2023,
    ///     "TipoHolerite": 0
    /// }
    /// 
    /// Exemplo de retorno (sucesso):
    /// 
    /// {
    ///     "message": "Holerite salvo."
    /// }
    /// 
    /// Exemplo de retorno (erro):
    /// 
    /// {
    ///     "message": "Arquivo PDF não enviado."
    /// }
    /// </remarks>
    [Authorize]
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadHolerite([FromForm] UploadHoleriteRequest request)
    {
        try
        {
            if (request.ArquivoPdf == null || request.ArquivoPdf.Length == 0)
                return BadRequest("Arquivo PDF não enviado.");
            if (request.ArquivoPdf.ContentType != "application/pdf")
                return BadRequest("Formato de arquivo inválido. Apenas PDF é aceito.");

            var sucesso = await _holeriteService.SalvarHoleriteAsync(request);
            return sucesso ? Ok("Holerite salvo.") : BadRequest("Falha no upload.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }


    /// <summary>
    /// Consulta holerites de um funcionário.
    /// </summary>
    /// <param name="request">Dados da consulta do holerite.</param>
    /// <returns>Retorna uma lista de holerites ou um arquivo PDF do holerite.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// POST /api/holerites/consulta
    /// Content-Type: application/json
    /// 
    /// {
    ///     "Cpf": "123.456.789-00",
    ///     "DataNascimento": "1990-01-01T00:00:00",
    ///     "TipoHolerite": 0,
    ///     "MesReferencia": 1,
    ///     "AnoReferencia": 2023
    /// }
    /// 
    /// Exemplo de retorno (sucesso):
    /// 
    /// Retorna o arquivo PDF do holerite ou uma lista de holerites.
    /// 
    /// Exemplo de retorno (erro):
    /// 
    /// {
    ///     "message": "Nenhum holerite encontrado."
    /// }
    /// </remarks>
    [Authorize]
    [HttpPost("consulta")]
    public async Task<IActionResult> ConsultarHolerites([FromBody] ConsultaHoleriteRequest request)
    {
        var holerites = await _holeriteService.ConsultarHoleritesAsync(request);

        if (!holerites.Any())
            return NotFound("Nenhum holerite encontrado.");

        if (holerites.Count() == 1)
        {
            var holerite = holerites.First();
            return File(holerite.ArquivoPdf, "application/pdf", $"Holerite_{holerite.Id}.pdf");
        }

        return Ok(holerites.Select(h => new
        {
            h.Id,
            h.MesReferencia,
            h.AnoReferencia,
            h.TipoHolerite,
            h.DataUpload
        }));
    }


    /// <summary>
    /// Obtém todos os holerites cadastrados.
    /// </summary>
    /// <returns>Retorna uma lista de holerites ou uma mensagem de erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// GET /api/holerites/todos
    /// 
    /// Exemplo de retorno (sucesso):
    /// 
    /// [
    ///     {
    ///         "Id": 1,
    ///         "FuncionarioId": 1,
    ///         "NomeFuncionarioExtraido": "João Silva",
    ///         "TipoHolerite": 0,
    ///         "MesReferencia": 1,
    ///         "AnoReferencia": 2023
    ///     }
    /// ]
    /// 
    /// Exemplo de retorno (erro):
    /// 
    /// {
    ///     "message": "Nenhum holerite encontrado."
    /// }
    /// </remarks>
    [Authorize]
    [HttpGet("todos")]
    public async Task<IActionResult> ObterTodosHolerites()
    {
        var holerites = await _holeriteService.ObterTodosHoleritesAsync();
        if (!holerites.Any())
            return NotFound("Nenhum holerite encontrado.");

        return Ok(holerites.Select(h => new
            { h.Id, h.FuncionarioId, h.NomeFuncionarioExtraido, h.TipoHolerite, h.MesReferencia, h.AnoReferencia }));
    }
}