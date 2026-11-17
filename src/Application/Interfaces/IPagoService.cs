using System.Collections.Generic;
using Application.Models;
using Application.Models.Requests;

namespace Application.Interfaces
{
    public interface IPagoService
    {
        List<PagoDto> GetAll();
        PagoDto GetById(int id);
        PagoDto PagarSimple(PagoSimpleRequest request);
        PagoDto PagarMultiple(PagoMultipleRequest request);
        PagoDto ActualizarSimple(int id, PagoSimpleRequest request);
        PagoDto ActualizarMultiple(int id, PagoMultipleRequest request);
        Task DeleteAsync(int id, int usuarioId, bool esAdmin, CancellationToken ct = default);
        Task<PagoDto> SubirComprobanteAsync(int id, Stream contenido, long tamanio, int usuarioId, bool esAdmin, CancellationToken ct = default);
        string ObtenerUrlComprobante(int id, int usuarioId, bool esAdmin);
        List<PagoDto> GetByViajeId(int viajeId);
        List<PagoDto> GetByParticipanteId(int participanteId); 
        List<SaldoDto> CalcularSaldosDelViaje(int viajeId);
    }
}