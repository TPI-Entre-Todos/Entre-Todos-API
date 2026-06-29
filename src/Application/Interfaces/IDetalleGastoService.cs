using Application.Models;

namespace Application.Interfaces
{
    public interface IDetalleGastoService
    {
        List<DetalleGastoDto> ObtenerDetallesPorGasto(int gastoId, int userId, bool esAdmin);
    }
}
