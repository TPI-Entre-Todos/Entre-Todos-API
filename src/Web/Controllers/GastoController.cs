using System.Security.Claims;
using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,User")]
    public class GastoController : ControllerBase
    {
        private readonly IGastoService _gastoService;

        public GastoController(IGastoService gastoService)
        {
            _gastoService = gastoService;
        }

        // ─── Creación ─────────────────────────────────────────────────────────────
        [HttpPost("igualitario")]
        public IActionResult PostIgualitario([FromBody] GastoIgualitarioRequest dto)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            var resultado = _gastoService.CrearIgualitario(dto, userId, esAdmin);
            return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
        }

        [HttpPost("porcentaje")]
        public IActionResult PostPorPorcentaje([FromBody] GastoPorPorcentajeRequest dto)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            var resultado = _gastoService.CrearPorPorcentaje(dto, userId, esAdmin);
            return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
        }

        [HttpPost("personalizado")]
        public IActionResult PostPersonalizado([FromBody] GastoPersonalizadoRequest dto)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            var resultado = _gastoService.CrearPersonalizado(dto, userId, esAdmin);
            return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
        }

        // ─── Consulta ─────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult GetAll()
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            return Ok(_gastoService.ObtenerTodos(userId, esAdmin));
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            var gasto = _gastoService.ObtenerGastoPorId(id, userId, esAdmin);
            return Ok(gasto);
        }

        [HttpGet("viaje/{viajeId:int}")]
        public IActionResult GetPorViaje(int viajeId)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            return Ok(_gastoService.ObtenerGastosPorViaje(viajeId, userId, esAdmin));
        }

        // ─── Actualización ────────────────────────────────────────────────────────
        [HttpPut("{id:int}/igualitario")]
        public IActionResult PutIgualitario(int id, [FromBody] GastoIgualitarioRequest dto)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            return Ok(_gastoService.ActualizarIgualitario(id, dto, userId, esAdmin));
        }

        [HttpPut("{id:int}/porcentaje")]
        public IActionResult PutPorPorcentaje(int id, [FromBody] GastoPorPorcentajeRequest dto)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            return Ok(_gastoService.ActualizarPorPorcentaje(id, dto, userId, esAdmin));
        }

        [HttpPut("{id:int}/personalizado")]
        public IActionResult PutPersonalizado(int id, [FromBody] GastoPersonalizadoRequest dto)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            return Ok(_gastoService.ActualizarPersonalizado(id, dto, userId, esAdmin));
        }

        // ─── Eliminación ───────────────────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var (userId, esAdmin) = ObtenerIdentidad();
            _gastoService.EliminarGasto(id, userId, esAdmin);
            return NoContent();
        }

        // ─── Helpers ───────────────────────────────────────────────────────────────
        private (int userId, bool esAdmin) ObtenerIdentidad()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool esAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
            return (userId, esAdmin);
        }
    }
}
