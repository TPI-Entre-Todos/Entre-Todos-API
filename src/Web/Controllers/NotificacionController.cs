using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application;
using Application.Models;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _service;

        // Inyectamos usando la interfaz perfectamente desacoplada
        public NotificacionController(INotificacionService service)
        {
            _service = service;
        }

        // POST: api/Notificacion (Crear una notificación manual/sistema)
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotificacionCreateDto dto)
        {
            var resultado = await _service.CrearNotificacionAsync(dto);
            return Ok(resultado);
        }

        // GET: api/Notificacion/usuario/3 (Traer las alertas de un usuario)
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetPorUsuario(int usuarioId)
        {
            var notificaciones = await _service.ObtenerPorUsuarioAsync(usuarioId);
            return Ok(notificaciones);
        }

        // PUT: api/Notificacion/5/leer (Marcar como leída)
        [HttpPut("{id}/leer")]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            await _service.MarcarComoLeidaAsync(id);
            return NoContent();
        }
    }
}
