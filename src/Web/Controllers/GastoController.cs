using System.Security.Claims;
using Application.Interfaces;
using Application.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GastoController : ControllerBase
    {
        private readonly IGastoService _gastoService;

        public GastoController(IGastoService gastoService)
        {
            _gastoService = gastoService;
        }

        // ─── Creación como User ───────────────────────────────────────────────────
        // El participante que pagó se resuelve automáticamente desde el token JWT.
        // No hace falta enviar ParticipanteId en el body.

        [HttpPost("igualitario")]
        [Authorize(Roles = "User")]
        public IActionResult PostIgualitario([FromBody] GastoIgualitarioRequest dto)
        {
            try
            {
                int userId = ObtenerUserId();
                var resultado = _gastoService.CrearIgualitarioComoUser(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("porcentaje")]
        [Authorize(Roles = "User")]
        public IActionResult PostPorPorcentaje([FromBody] GastoPorPorcentajeRequest dto)
        {
            try
            {
                int userId = ObtenerUserId();
                var resultado = _gastoService.CrearPorPorcentajeComoUser(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("personalizado")]
        [Authorize(Roles = "User")]
        public IActionResult PostPersonalizado([FromBody] GastoPersonalizadoRequest dto)
        {
            try
            {
                int userId = ObtenerUserId();
                var resultado = _gastoService.CrearPersonalizadoComoUser(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        // ─── Creación como Admin ──────────────────────────────────────────────────
        // El admin especifica explícitamente el ParticipanteId de quien pagó.

        [HttpPost("admin/igualitario")]
        [Authorize(Roles = "Admin")]
        public IActionResult PostIgualitarioAdmin([FromBody] GastoIgualitarioAdminRequest dto)
        {
            try
            {
                var resultado = _gastoService.CrearIgualitarioComoAdmin(dto);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("admin/porcentaje")]
        [Authorize(Roles = "Admin")]
        public IActionResult PostPorPorcentajeAdmin([FromBody] GastoPorPorcentajeAdminRequest dto)
        {
            try
            {
                var resultado = _gastoService.CrearPorPorcentajeComoAdmin(dto);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("admin/personalizado")]
        [Authorize(Roles = "Admin")]
        public IActionResult PostPersonalizadoAdmin([FromBody] GastoPersonalizadoAdminRequest dto)
        {
            try
            {
                var resultado = _gastoService.CrearPersonalizadoComoAdmin(dto);
                return CreatedAtAction(nameof(GetById), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        // ─── Consulta ─────────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetAll()
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                return Ok(_gastoService.ObtenerTodos(userId, esAdmin));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,User")]
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
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetPorViaje(int viajeId)
        {
            try
            {
                var (userId, esAdmin) = ObtenerIdentidad();
                return Ok(_gastoService.ObtenerGastosPorViaje(viajeId, userId, esAdmin));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // ─── Actualización como User ──────────────────────────────────────────────

        [HttpPut("{id:int}/igualitario")]
        [Authorize(Roles = "User")]
        public IActionResult PutIgualitario(int id, [FromBody] ActualizarGastoIgualitarioRequest dto)
        {
            try
            {
                int userId = ObtenerUserId();
                return Ok(_gastoService.ActualizarIgualitarioComoUser(id, dto, userId));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}/porcentaje")]
        [Authorize(Roles = "User")]
        public IActionResult PutPorPorcentaje(int id, [FromBody] ActualizarGastoPorPorcentajeRequest dto)
        {
            try
            {
                int userId = ObtenerUserId();
                return Ok(_gastoService.ActualizarPorPorcentajeComoUser(id, dto, userId));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}/personalizado")]
        [Authorize(Roles = "User")]
        public IActionResult PutPersonalizado(int id, [FromBody] ActualizarGastoPersonalizadoRequest dto)
        {
            try
            {
                int userId = ObtenerUserId();
                return Ok(_gastoService.ActualizarPersonalizadoComoUser(id, dto, userId));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        // ─── Actualización como Admin ─────────────────────────────────────────────

        [HttpPut("{id:int}/admin/igualitario")]
        [Authorize(Roles = "Admin")]
        public IActionResult PutIgualitarioAdmin(int id, [FromBody] ActualizarGastoIgualitarioAdminRequest dto)
        {
            try
            {
                return Ok(_gastoService.ActualizarIgualitarioComoAdmin(id, dto));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}/admin/porcentaje")]
        [Authorize(Roles = "Admin")]
        public IActionResult PutPorPorcentajeAdmin(int id, [FromBody] ActualizarGastoPorPorcentajeAdminRequest dto)
        {
            try
            {
                return Ok(_gastoService.ActualizarPorPorcentajeComoAdmin(id, dto));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}/admin/personalizado")]
        [Authorize(Roles = "Admin")]
        public IActionResult PutPersonalizadoAdmin(int id, [FromBody] ActualizarGastoPersonalizadoAdminRequest dto)
        {
            try
            {
                return Ok(_gastoService.ActualizarPersonalizadoComoAdmin(id, dto));
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,User")]
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

        // ─── Helpers ──────────────────────────────────────────────────────────────

        private int ObtenerUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        private (int userId, bool esAdmin) ObtenerIdentidad()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool esAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";
            return (userId, esAdmin);
        }
    }
}
