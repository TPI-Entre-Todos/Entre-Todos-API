using System.Security.Claims;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DetalleGastoController : ControllerBase
    {
        private readonly IDetalleGastoService _serviceDetalleGasto;

        public DetalleGastoController(IDetalleGastoService serviceDetalleGasto)
        {
            _serviceDetalleGasto = serviceDetalleGasto;
        }

        // Cualquier participante del viaje puede ver los detalles de un gasto
        [HttpGet("gasto/{gastoId:int}")]
        public IActionResult GetPorGasto(int gastoId)
        {


            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            bool esAdmin = User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

            var detalles = _serviceDetalleGasto.ObtenerDetallesPorGasto(gastoId, userId, esAdmin);
            return Ok(detalles);


        }
    }
}
