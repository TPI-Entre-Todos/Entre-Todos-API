using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IGastoService
    {
        // ─── Creación como User (participanteId resuelto desde userId del token) ──
        GastoDto CrearIgualitarioComoUser(GastoIgualitarioRequest dto, int userId);
        GastoDto CrearPorPorcentajeComoUser(GastoPorPorcentajeRequest dto, int userId);
        GastoDto CrearPersonalizadoComoUser(GastoPersonalizadoRequest dto, int userId);

        // ─── Creación como Admin (participanteId especificado en el request) ──────
        GastoDto CrearIgualitarioComoAdmin(GastoIgualitarioAdminRequest dto);
        GastoDto CrearPorPorcentajeComoAdmin(GastoPorPorcentajeAdminRequest dto);
        GastoDto CrearPersonalizadoComoAdmin(GastoPersonalizadoAdminRequest dto);

        // ─── Consulta ─────────────────────────────────────────────────────────────
        List<GastoDto> ObtenerGastosPorViaje(int viajeId, int userId, bool esAdmin);
        GastoDto? ObtenerGastoPorId(int id, int userId, bool esAdmin);

        // ─── Modificación y baja ──────────────────────────────────────────────────
        GastoDto ActualizarGasto(int id, GastoConDetallesRequest dto, int userId, bool esAdmin);
        void EliminarGasto(int id, int userId, bool esAdmin);
    }
}
