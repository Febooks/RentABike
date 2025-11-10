using Microsoft.AspNetCore.Mvc;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;
using Serilog;

namespace RentABike.API.Controllers;

/// <summary>
/// Controller para gerenciamento de locações
/// </summary>
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

    /// <summary>
    /// Cria uma nova locação
    /// </summary>
    /// <param name="dto">Dados da locação (ID da Moto, ID do Entregador, Plano em dias: 7, 15, 30, 45 ou 50)</param>
    /// <returns>Locação criada com sucesso</returns>
    /// <response code="201">Locação criada com sucesso</response>
    /// <response code="400">Erro de validação (entregador não habilitado, locação ativa existente, plano inválido ou dados inválidos)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Obtém uma locação específica pelo ID
    /// </summary>
    /// <param name="id">ID da locação</param>
    /// <returns>Dados da locação</returns>
    /// <response code="200">Locação encontrada</response>
    /// <response code="404">Locação não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Realiza a devolução de uma locação com cálculo de multas e valores adicionais
    /// </summary>
    /// <param name="id">ID da locação</param>
    /// <param name="dto">Data de devolução</param>
    /// <returns>Locação devolvida com valores calculados</returns>
    /// <response code="200">Locação devolvida com sucesso</response>
    /// <response code="400">Erro de validação (data inválida ou dados inválidos)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("{id}/return")]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Calcula o valor total de uma locação sem persistir a devolução (simulação)
    /// </summary>
    /// <param name="id">ID da locação</param>
    /// <param name="dto">Data de devolução para cálculo</param>
    /// <returns>Valores calculados da locação (sem persistir)</returns>
    /// <response code="200">Cálculo realizado com sucesso</response>
    /// <response code="400">Erro de validação (data inválida ou dados inválidos)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("{id}/calculate")]
    [ProducesResponseType(typeof(RentalDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

