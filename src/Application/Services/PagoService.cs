using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models;
using System;
using System.Collections.Generic;

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
            if (pago == null)
                throw new NotFoundException("Pago no encontrado");

            return PagoDto.Create(pago);
        }

        public PagoDto Add(PagoRequest request)
        {
            ValidarPagoParaCreacion(request);
            
            // Usamos el constructor actualizado de la entidad Pago (RemitenteId, DestinatarioId, ViajeId, Monto, Metodo, Comprobante)
            Pago pago = new Pago(
                request.ParticipanteId, // Remitente
                request.ParticipanteId, // Destinatario (Temporalmente el mismo hasta mapear DestinatarioId en PagoRequest)
                request.ViajeId, 
                request.Monto.Value, 
                request.Metodo, 
                request.Comprobante
            );

            _pagoRepository.Add(pago);
            return PagoDto.Create(pago);    
        }

        public PagoDto Update(int id, PagoRequest request)
        {
            Pago existing = _pagoRepository.GetById(id);
            if (existing == null)
                throw new NotFoundException("Pago no encontrado");

            // Validamos solo los datos que el usuario está intentando modificar
            if (request.ParticipanteId > 0)
                existing.RemitenteId = request.ParticipanteId; // 👈 Corregido: propiedad de la entidad actualizada
                
            if (request.ViajeId > 0)
                existing.ViajeId = request.ViajeId;
                
            if (request.Monto.HasValue)
            {
                if (request.Monto.Value <= 0) 
                    throw new BadRequestException("El monto debe ser mayor a 0");
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
                throw new BadRequestException("ParticipanteId debe ser válido");
            if (request.ViajeId <= 0)
                throw new BadRequestException("ViajeId debe ser válido");
            if (!request.Monto.HasValue || request.Monto.Value <= 0)
                throw new BadRequestException("El monto es requerido y debe ser mayor a 0");
            if (string.IsNullOrEmpty(request.Metodo))
                throw new BadRequestException("El método de pago es requerido");
        }
    }
}
