using System.Security.Claims;
using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GastoController : ControllerBase
    {
        private readonly IGastoService _gastoService;

        public GastoController(IGastoService gastoService)
        {
            _gastoService = gastoService;
        }

        // ─── Creación por tipo de división ────────────────────────────────────────

        [HttpPost("igualitario")]
        public IActionResult PostIgualitario([FromBody] GastoIgualitarioRequest dto)
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                var resultado = _gastoService.CrearIgualitario(dto, userId, esAdmin);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("porcentaje")]
        public IActionResult PostPorPorcentaje([FromBody] GastoPorPorcentajeRequest dto)
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                var resultado = _gastoService.CrearPorPorcentaje(dto, userId, esAdmin);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("personalizado")]
        public IActionResult PostPersonalizado([FromBody] GastoPersonalizadoRequest dto)
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                var resultado = _gastoService.CrearPersonalizado(dto, userId, esAdmin);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        // ─── Consulta ─────────────────────────────────────────────────────────────

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                var gasto = _gastoService.ObtenerGastoPorId(id, userId, esAdmin);
                if (gasto == null) return NotFound();
                return Ok(gasto);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("viaje/{viajeId:int}")]
        public IActionResult GetPorViaje(int viajeId)
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                return Ok(_gastoService.ObtenerGastosPorViaje(viajeId, userId, esAdmin));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // ─── Actualización y baja ─────────────────────────────────────────────────

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] GastoConDetallesRequest dto)
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                return Ok(_gastoService.ActualizarGasto(id, dto, userId, esAdmin));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                _gastoService.EliminarGasto(id, userId, esAdmin);
                return NoContent();
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        // ─── Helper ───────────────────────────────────────────────────────────────

        private (int userId, bool esAdmin) ObtenerIdentidad()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool esAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
            return (userId, esAdmin);
        }
    }
}
