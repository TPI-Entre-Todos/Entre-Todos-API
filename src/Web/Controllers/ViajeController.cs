using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViajeController : ControllerBase
    {
        private readonly IViajeService _viajeService;

        public ViajeController(IViajeService viajeService)
        {
            _viajeService = viajeService;
        }

        [HttpPost]
        public IActionResult AgregarViaje(Viaje viaje)
        {
            _viajeService.AgregarViaje(viaje);
            return Ok();
        }

        [HttpGet]
        public IActionResult ObtenerViajes()
        {
            var viajes = _viajeService.ObtenerViajes();
            return Ok(viajes);
        }
        [HttpDelete("{id}")]
        public IActionResult EliminarViaje([FromRoute] int id)
        {
            _viajeService.EliminarViaje(id);
            return Ok();
        }
    }
}