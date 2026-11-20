using System.Collections.Generic;
using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IPagoService
    {
        List<PagoDto> GetAll();
        PagoDto GetById(int id, int usuarioId, bool esAdmin);
        PagoDto PagarSimple(PagoSimpleRequest request, int usuarioId, bool esAdmin);
        PagoDto PagarMultiple(PagoMultipleRequest request, int usuarioId, bool esAdmin);
        PagoDto ActualizarSimple(int id, PagoSimpleRequest request, int usuarioId, bool esAdmin);
        PagoDto ActualizarMultiple(int id, PagoMultipleRequest request, int usuarioId, bool esAdmin);
        Task DeleteAsync(int id, int usuarioId, bool esAdmin, CancellationToken ct = default);
        Task<PagoDto> SubirComprobanteAsync(int id, Stream contenido, long tamanio, int usuarioId, bool esAdmin, CancellationToken ct = default);
        string ObtenerUrlComprobante(int id, int usuarioId, bool esAdmin);
        List<PagoDto> GetByViajeId(int viajeId, int usuarioId, bool esAdmin);
        List<PagoDto> GetByParticipanteId(int participanteId, int usuarioId, bool esAdmin);
        List<SaldoDto> CalcularSaldosDelViaje(int viajeId);
    }
}