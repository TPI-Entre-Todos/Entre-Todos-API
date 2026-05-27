using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application;      
using Application.Models;
namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipanteViajeController : ControllerBase
    {
        private readonly ParticipanteViajeService _service;

        // Inyectamos el servicio que acabamos de crear
        public ParticipanteViajeController(ParticipanteViajeService service)
        {
            _service = service;
        }

        // POST: api/ParticipanteViaje (Alta / Invitar)
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ParticipanteViajeCreateDto dto)
        {
            try
            {
                var resultado = await _service.RegistrarParticipanteAsync(dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/ParticipanteViaje/viaje/5 (Listar los de un viaje)
        [HttpGet("viaje/{viajeId}")]
        public async Task<IActionResult> GetPorViaje(int viajeId)
        {
            var participantes = await _service.ObtenerPorViajeAsync(viajeId);
            return Ok(participantes);
        }

        // PUT: api/ParticipanteViaje/5/responder (Modificación de estado)
        [HttpPut("{id}/responder")]
        public async Task<IActionResult> ResponderInvitacion(int id, [FromBody] string nuevoEstado)
        {
            try
            {
                await _service.ResponderInvitacionAsync(id, nuevoEstado);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/ParticipanteViaje/5 (Baja)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.EliminarParticipanteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}