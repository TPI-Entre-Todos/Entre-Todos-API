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
            ValidarPagoParaCreacion(request);
            
            // Usamos request.Monto.Value porque ya sabemos que no es nulo gracias a la validación
            Pago pago = new(request.ParticipanteId, request.ViajeId, request.Monto.Value, request.Metodo, request.Comprobante);
            _pagoRepository.Add(pago);
            return PagoDto.Create(pago);
        }

        public PagoDto Update(int id, PagoRequest request)
        {
            Pago existing = _pagoRepository.GetById(id);
            if (existing == null)
                return null;

            // Validamos solo los datos que el usuario está intentando modificar
            if (request.ParticipanteId > 0)
                existing.ParticipanteId = request.ParticipanteId;
            if (request.ViajeId > 0)
                existing.ViajeId = request.ViajeId;
            if (request.Monto.HasValue)
            {
                if (request.Monto.Value <= 0) 
                    throw new ArgumentException("El monto debe ser mayor a 0");
                existing.Monto = request.Monto.Value;
            }
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

        private void ValidarPagoParaCreacion(PagoRequest request)
        {
            if (request.ParticipanteId <= 0)
                throw new ArgumentException("ParticipanteId debe ser válido");
            if (request.ViajeId <= 0)
                throw new ArgumentException("ViajeId debe ser válido");
            if (!request.Monto.HasValue || request.Monto.Value <= 0)
                throw new ArgumentException("El monto es requerido y debe ser mayor a 0");
            if (string.IsNullOrEmpty(request.Metodo))
                throw new ArgumentException("El método de pago es requerido");
        }
    }
}