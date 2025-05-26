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
    /// Atualiza um holerite existente.
    /// </summary>
    /// <param name="id">ID do holerite a ser atualizado.</param>
    /// <param name="request">Dados atualizados do holerite.</param>
    /// <returns>Retorna uma mensagem de sucesso ou erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// PUT /api/holerites/{id}
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
    /// {
    ///     "message": "Holerite atualizado com sucesso."
    /// }
    /// 
    /// Exemplo de retorno (erro):
    /// {
    ///     "message": "Holerite não encontrado."
    /// }
    /// </remarks>
    [Authorize]
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AtualizarHolerite(int id, [FromForm] UploadHoleriteRequest request)
    {
        var holerite = await _holeriteService.ObterHoleritePorIdAsync(id);
        if (holerite == null)
            return NotFound("Holerite não encontrado.");

        var atualizar = await _holeriteService.AtualizarHoleriteAsync(new Holerite
        {
            Id = id,
            ArquivoPdf = request.ArquivoPdf != null ? await _holeriteService.LerArquivoAsync(request.ArquivoPdf) : holerite.ArquivoPdf,
            MesReferencia = request.MesReferencia,
            AnoReferencia = request.AnoReferencia,
            TipoHolerite = request.TipoHolerite,
            DataUpload = DateTime.Now
        });

        if (atualizar)
        {
            return Ok("Holerite atualizado com sucesso.");
        }


        return BadRequest("Falha ao atualizar o holerite.");

    }



    /// <summary>
    /// Exclui um holerite existente.
    /// </summary>
    /// <param name="id">ID do holerite a ser excluído.</param>
    /// <returns>Retorna uma mensagem de sucesso ou erro.</returns>
    /// <remarks>
    /// Exemplo de request:
    /// 
    /// DELETE /api/holerites/{id}
    /// 
    /// Exemplo de retorno (sucesso):
    /// {
    ///     "message": "Holerite excluído com sucesso."
    /// }
    /// 
    /// Exemplo de retorno (erro):
    /// {
    ///     "message": "Holerite não encontrado."
    /// }
    /// </remarks>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirHolerite(int id)
    {
        var excluir = await _holeriteService.ExcluirHoleriteAsync(id);
        if (excluir)
        {
            return Ok("Holerite excluído com sucesso.");
        }
        return NotFound("Holerite não encontrado.");
    }

}