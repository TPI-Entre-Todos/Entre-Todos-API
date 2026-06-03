using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models;

namespace Application.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;

        public PagoService(IPagoRepository pagoRepository)
        {
            _pagoRepository = pagoRepository;
        }

        public List<PagoDto> GetAll()
        {
            List<Pago> pagos = _pagoRepository.GetAll();
            return PagoDto.CreateList(pagos);
        }

        public PagoDto GetById(int id)
        {
            Pago pago = _pagoRepository.GetById(id);
            return PagoDto.Create(pago);
        }

        public PagoDto Add(PagoRequest request)
        {
            ValidarPago(request);
            
            Pago pago = new(request.ParticipanteId, request.ViajeId, request.Monto, request.Metodo, request.Comprobante);
            _pagoRepository.Add(pago);
            return PagoDto.Create(pago);
        }

        public PagoDto Update(int id, PagoRequest request)
        {
            ValidarPago(request);
            
            Pago existing = _pagoRepository.GetById(id);
            if (existing == null)
                return null;

            if (request.ParticipanteId > 0)
                existing.ParticipanteId = request.ParticipanteId;
            if (request.ViajeId > 0)
                existing.ViajeId = request.ViajeId;
            if (request.Monto > 0)
                existing.Monto = request.Monto;
            if (!string.IsNullOrEmpty(request.Metodo))
                existing.Metodo = request.Metodo;
            if (!string.IsNullOrEmpty(request.Comprobante))
                existing.Comprobante = request.Comprobante;

            return PagoDto.Create(_pagoRepository.Update(existing));
        }

        public void Delete(int id)
        {
            _pagoRepository.Delete(id);
        }

        public List<PagoDto> GetByViajeId(int viajeId)
        {
            List<Pago> pagos = _pagoRepository.GetByViajeId(viajeId);
            return PagoDto.CreateList(pagos);
        }

        public List<PagoDto> GetByParticipanteId(int participanteId)
        {
            List<Pago> pagos = _pagoRepository.GetByParticipanteId(participanteId);
            return PagoDto.CreateList(pagos);
        }

        private void ValidarPago(PagoRequest request)
        {
            if (request.ParticipanteId <= 0)
                throw new ArgumentException("ParticipanteId debe ser válido");

            if (request.ViajeId <= 0)
                throw new ArgumentException("ViajeId debe ser válido");

            if (request.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a 0");

            if (string.IsNullOrEmpty(request.Metodo))
                throw new ArgumentException("El método de pago es requerido");
        }
    }
}
