using Microsoft.AspNetCore.Mvc;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;

namespace RentABike.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MotorcyclesController : ControllerBase
{
    private readonly IMotorcycleService _motorcycleService;
    private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<MotorcyclesController>();

    public MotorcyclesController(IMotorcycleService motorcycleService)
    {
        _motorcycleService = motorcycleService;
    }

    [HttpPost]
    public async Task<ActionResult<MotorcycleDTO>> CreateMotorcycle([FromBody] CreateMotorcycleDTO dto)
    {
        try
        {
            var result = await _motorcycleService.CreateMotorcycleAsync(dto);
            return CreatedAtAction(nameof(GetMotorcycle), new { id = result.Id }, result);
        }
        catch(InvalidOperationException ex)
        {
            _logger.Error($"Erro ao criar moto: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch(Exception ex)
        {
            _logger.Error($"Erro ao criar moto: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MotorcycleDTO>>> ListMotorcycles([FromQuery] string? licensePlate)
    {
        try
        {
            var result = await _motorcycleService.ListMotorcyclesAsync(licensePlate);
            return Ok(result);
        }
        catch(Exception ex)
        {
            _logger.Error($"Erro ao listar motos: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MotorcycleDTO>> GetMotorcycle(Guid id)
    {
        try
        {
            var motorcycle = await _motorcycleService.GetMotorcycleByIdAsync(id);

            if (motorcycle == null)
            {
                return NotFound();
            }

            return Ok(motorcycle);
        }
        catch(Exception ex)
        {
            _logger.Error($"Erro ao obter moto: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }

    [HttpPut("{id}/license-plate")]
    public async Task<ActionResult<MotorcycleDTO>> UpdateLicensePlate(Guid id, [FromBody] UpdateMotorcycleLicensePlateDTO dto)
    {
        try
        {
            var result = await _motorcycleService.UpdateLicensePlateAsync(id, dto);
            return Ok(result);
        }
        catch(InvalidOperationException ex)
        {
            _logger.Error($"Erro ao atualizar placa da moto: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch(Exception ex)
        {
            _logger.Error($"Erro ao atualizar placa da moto: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveMotorcycle(Guid id)
    {
        try
        {
            await _motorcycleService.RemoveMotorcycleAsync(id);
            return NoContent();
        }
        catch(InvalidOperationException ex)
        {
            _logger.Error($"Erro ao remover moto: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch(Exception ex)
        {
            _logger.Error($"Erro ao remover moto: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }
}

