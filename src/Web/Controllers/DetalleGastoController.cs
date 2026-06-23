using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application;
using Application.Models;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleGastoController : ControllerBase
    {
        private readonly IDetalleGastoService _service;

        // Inyección perfecta usando la Interfaz
        public DetalleGastoController(IDetalleGastoService service)
        {
            _service = service;
        }

        // POST: api/DetalleGasto (Registra el gasto con toda su división)
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GastoConDetallesCreateDto dto)
        {
            try
            {
                await _service.RegistrarGastoConDetallesAsync(dto);
                return Ok(new { mensaje = "Gasto y sus detalles divididos correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/DetalleGasto/gasto/5 (Ver cómo se dividió un gasto específico)
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