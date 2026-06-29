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
        void Delete(int id);
        List<PagoDto> GetByViajeId(int viajeId);
        List<PagoDto> GetByParticipanteId(int participanteId); 
        List<SaldoDto> CalcularSaldosDelViaje(int viajeId);
    }
}