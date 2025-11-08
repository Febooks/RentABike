using Microsoft.AspNetCore.Mvc;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;

namespace RentABike.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveryPersonsController : ControllerBase
{
    private readonly IDeliveryPersonService _deliveryPersonService;

    public DeliveryPersonsController(IDeliveryPersonService deliveryPersonService)
    {
        _deliveryPersonService = deliveryPersonService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
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

    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryPersonDTO>> GetDeliveryPerson(Guid id)
    {
        var deliveryPerson = await _deliveryPersonService.GetDeliveryPersonByIdAsync(id);

        if (deliveryPerson == null)
        {
            return NotFound();
        }

        return Ok(deliveryPerson);
    }

    [HttpPut("{id}/license-image")]
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

