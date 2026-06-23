using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;

namespace Application
{
    public interface IDetalleGastoService
    {
        Task RegistrarGastoConDetallesAsync(GastoConDetallesCreateDto dto);
        Task<List<DetalleGastoDto>> ObtenerDetallesPorGastoAsync(int gastoId);
    }
}