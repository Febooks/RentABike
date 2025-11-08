using Microsoft.AspNetCore.Mvc;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;
using Serilog;

namespace RentABike.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly Serilog.ILogger _logger = Log.ForContext<RentalsController>();

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpPost]
    public async Task<ActionResult<RentalDTO>> CreateRental([FromBody] CreateRentalDTO dto)
    {
        try
        {
            var result = await _rentalService.CreateRentalAsync(dto);
            return CreatedAtAction(nameof(GetRental), new { id = result.Id }, result);
        }
        catch(InvalidOperationException ex)
        {
            _logger.Error($"Erro ao criar locação: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch(Exception ex)
        {
            _logger.Error($"Erro ao criar locação: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RentalDTO>> GetRental(Guid id)
    {
        try
        {
            var rental = await _rentalService.GetRentalByIdAsync(id);

            if (rental == null)
            {
                return NotFound();
            }

            return Ok(rental);
        }
        catch(Exception ex)
        {
            _logger.Error($"Erro ao obter locação: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }

    [HttpPost("{id}/return")]
    public async Task<ActionResult<RentalDTO>> ReturnRental(Guid id, [FromBody] ReturnRentalDTO dto)
    {
        try
        {
            var result = await _rentalService.ReturnRentalAsync(id, dto);
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            _logger.Error($"Erro ao devolver locação: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch(Exception ex)
        {
            _logger.Error($"Erro ao devolver locação: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }

    [HttpPost("{id}/calculate")]
    public async Task<ActionResult<RentalDTO>> CalculateRental(Guid id, [FromBody] CalculateRentalDTO dto)
    {
        try
        {
            var result = await _rentalService.CalculateRentalAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.Error($"Erro ao calcular locação: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error($"Erro ao calcular locação: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno no servidor." });
        }
    }
}

