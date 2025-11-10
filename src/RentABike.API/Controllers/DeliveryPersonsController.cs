using Microsoft.AspNetCore.Mvc;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;

namespace RentABike.API.Controllers;

/// <summary>
/// Controller para gerenciamento de entregadores
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DeliveryPersonsController : ControllerBase
{
    private readonly IDeliveryPersonService _deliveryPersonService;

    public DeliveryPersonsController(IDeliveryPersonService deliveryPersonService)
    {
        _deliveryPersonService = deliveryPersonService;
    }

    /// <summary>
    /// Cria um novo entregador
    /// </summary>
    /// <param name="dto">Dados do entregador (Nome, CNPJ, Data de Nascimento, Número da CNH, Tipo da CNH, Imagem da CNH)</param>
    /// <returns>Entregador criado com sucesso</returns>
    /// <response code="201">Entregador criado com sucesso</response>
    /// <response code="400">Erro de validação (CNPJ duplicado, CNH duplicada, tipo de CNH inválido ou dados inválidos)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(DeliveryPersonDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeliveryPersonDTO>> CreateDeliveryPerson([FromForm] CreateDeliveryPersonDTO dto)
    {
        try
        {
            var result = await _deliveryPersonService.CreateDeliveryPersonAsync(dto);
            return CreatedAtAction(nameof(GetDeliveryPerson), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }

    /// <summary>
    /// Obtém um entregador específico pelo ID
    /// </summary>
    /// <param name="id">ID do entregador</param>
    /// <returns>Dados do entregador</returns>
    /// <response code="200">Entregador encontrado</response>
    /// <response code="404">Entregador não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DeliveryPersonDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeliveryPersonDTO>> GetDeliveryPerson(Guid id)
    {
        var deliveryPerson = await _deliveryPersonService.GetDeliveryPersonByIdAsync(id);

        if (deliveryPerson == null)
        {
            return NotFound();
        }

        return Ok(deliveryPerson);
    }

    /// <summary>
    /// Atualiza a imagem da CNH de um entregador
    /// </summary>
    /// <param name="id">ID do entregador</param>
    /// <param name="file">Arquivo de imagem da CNH (PNG ou BMP)</param>
    /// <returns>Entregador atualizado</returns>
    /// <response code="200">Imagem da CNH atualizada com sucesso</response>
    /// <response code="400">Arquivo não fornecido ou formato inválido</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id}/license-image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(DeliveryPersonDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeliveryPersonDTO>> UpdateLicenseImage(Guid id, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Arquivo não fornecido.");
        }

        using var imageStream = new MemoryStream();
        await file.CopyToAsync(imageStream);
        imageStream.Position = 0;

        try
        {
            var result = await _deliveryPersonService.UpdateLicenseImageAsync(
                id,
                imageStream,
                file.FileName,
                file.ContentType
            );

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }
}

