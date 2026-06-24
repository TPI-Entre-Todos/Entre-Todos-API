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
        private readonly INotificacionService _serviceNotificacion;

        // Inyectamos usando la interfaz perfectamente desacoplada
        public NotificacionController(INotificacionService serviceNotificacion)
        {
            _serviceNotificacion = serviceNotificacion;
        }

        // POST: api/Notificacion (Crear una notificación manual/sistema)
        [HttpPost]
        public IActionResult Post([FromBody] NotificacionCreateDto dto)
        {
            try
            {
                var resultado = _serviceNotificacion.CrearNotificacion(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Notificacion/usuario/3 (Traer las alertas de un usuario)
        [HttpGet("usuario/{usuarioId:int}")]
        public IActionResult GetPorUsuario(int usuarioId)
        {
            try
            {
                var notificaciones = _serviceNotificacion.ObtenerPorUsuario(usuarioId);
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Notificacion/5/leer (Marcar como leída)
        [HttpPut("{id:int}/leer")]
        public IActionResult MarcarLeida(int id)
        {
            try
            {
                _serviceNotificacion.MarcarComoLeida(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}