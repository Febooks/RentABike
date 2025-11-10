using Microsoft.AspNetCore.Mvc;
using RentABike.Application.DTOs;
using RentABike.Application.Services.Interfaces;

namespace RentABike.API.Controllers;

/// <summary>
/// Controller para gerenciamento de motos
/// </summary>
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

    /// <summary>
    /// Cria uma nova moto
    /// </summary>
    /// <param name="dto">Dados da moto a ser criada (Ano, Modelo, Placa)</param>
    /// <returns>Moto criada com sucesso</returns>
    /// <response code="201">Moto criada com sucesso</response>
    /// <response code="400">Erro de validação (placa duplicada ou dados inválidos)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(MotorcycleDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Lista todas as motos cadastradas, com opção de filtrar por placa
    /// </summary>
    /// <param name="licensePlate">Placa da moto para filtrar (opcional)</param>
    /// <returns>Lista de motos cadastradas</returns>
    /// <response code="200">Lista de motos retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MotorcycleDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Obtém uma moto específica pelo ID
    /// </summary>
    /// <param name="id">ID da moto</param>
    /// <returns>Dados da moto</returns>
    /// <response code="200">Moto encontrada</response>
    /// <response code="404">Moto não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MotorcycleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Atualiza a placa de uma moto
    /// </summary>
    /// <param name="id">ID da moto</param>
    /// <param name="dto">Nova placa da moto</param>
    /// <returns>Moto atualizada</returns>
    /// <response code="200">Placa atualizada com sucesso</response>
    /// <response code="400">Erro de validação (placa duplicada ou dados inválidos)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id}/license-plate")]
    [ProducesResponseType(typeof(MotorcycleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Remove uma moto do sistema
    /// </summary>
    /// <param name="id">ID da moto a ser removida</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Moto removida com sucesso</response>
    /// <response code="400">Erro de validação (moto possui locações registradas)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

