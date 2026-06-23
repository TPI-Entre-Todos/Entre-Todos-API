using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application.Interfaces
{
    public interface IDetalleGastoService
    {
        Task RegistrarGastoConDetallesAsync(DetalleGastoCreateDto dto); 
        Task<List<DetalleGastoDto>> ObtenerDetallesPorGastoAsync(int gastoId);
    }
}