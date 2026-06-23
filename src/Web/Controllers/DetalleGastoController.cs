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
        private readonly IDetalleGastoService _service;

                public DetalleGastoController(IDetalleGastoService service)
        {
            _service = service;
        }

        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DetalleGastoCreateDto dto)
        {
            try
            {
                await _service.RegistrarGastoConDetallesAsync(dto);
                return Ok(new { mensaje = "Gasto y detalle registrados correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       
        [HttpGet("gasto/{gastoId}")]
        public async Task<IActionResult> GetPorGasto(int gastoId)
        {
            try
            {
                var detalles = await _service.ObtenerDetallesPorGastoAsync(gastoId);
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}