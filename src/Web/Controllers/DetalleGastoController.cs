using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Models;
using System;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleGastoController : ControllerBase
    {
        private readonly IDetalleGastoService _serviceDetalleGasto;

        public DetalleGastoController(IDetalleGastoService serviceDetalleGasto)
        {
            _serviceDetalleGasto = serviceDetalleGasto;
        }


        [HttpPost]
        public IActionResult Post([FromBody] DetalleGastoCreateDto dto)
        {
            try
            {
                _serviceDetalleGasto.RegistrarGastoConDetalles(dto);
                return Ok("Gasto y detalle registrados correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("gasto/{gastoId}")]
        public IActionResult GetPorGasto(int gastoId)
        {
            try
            {
                var detalles = _serviceDetalleGasto.ObtenerDetallesPorGasto(gastoId);
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}