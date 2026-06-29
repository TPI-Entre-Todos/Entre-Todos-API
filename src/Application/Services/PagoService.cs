using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly IGastoRepository _gastoRepository;
        private readonly IParticipanteViajeRepository _participanteRepository;

        public PagoService(
            IPagoRepository pagoRepository, 
            IGastoRepository gastoRepository, 
            IParticipanteViajeRepository participanteRepository)
        {
            _pagoRepository = pagoRepository;
            _gastoRepository = gastoRepository;
            _participanteRepository = participanteRepository;
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
            
            // Creamos el pago vinculando al remitente y al destinatario real
            Pago pago = new Pago(
                request.ParticipanteId!.Value,   // Quien envía
                request.DestinatarioId!.Value,   // Quien recibe
                request.ViajeId!.Value, 
                request.Monto!.Value, 
                request.Metodo, 
                request.Comprobante
            );

            _pagoRepository.Add(pago);
            return PagoDto.Create(pago);
        }

        public PagoDto Update(int id, PagoRequest request)
        {
            Pago existing = _pagoRepository.GetById(id);
            if (existing == null) return null;

            if (request.ParticipanteId > 0) existing.RemitenteId = request.ParticipanteId.Value;
            if (request.DestinatarioId > 0) existing.DestinatarioId = request.DestinatarioId.Value;
            if (request.ViajeId > 0) existing.ViajeId = request.ViajeId.Value;
            
            if (request.Monto.HasValue)
            {
                if (request.Monto.Value <= 0) throw new ArgumentException("El monto debe ser mayor a 0");
                existing.Monto = request.Monto.Value;
            }
            if (!string.IsNullOrEmpty(request.Metodo)) existing.Metodo = request.Metodo;
            if (!string.IsNullOrEmpty(request.Comprobante)) existing.Comprobante = request.Comprobante;

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

        public List<SaldoDto> CalcularSaldosDelViaje(int viajeId)
        {
            var participantes = _participanteRepository.GetByViajeId(viajeId);
            var pagos = _pagoRepository.GetByViajeId(viajeId);
            
            // Como tu GastoRepository es Async, esperamos el resultado sincrónicamente acá para este flujo
            var gastos = _gastoRepository.GetByViajeIdAsync(viajeId).GetAwaiter().GetResult();

            var resumenSaldos = new List<SaldoDto>();

            foreach (var p in participantes)
            {
                var saldo = new SaldoDto
                {
                    ParticipanteId = p.Id,
                    Nombre = p.Usuario?.Nombre ?? $"Participante {p.Id}",
                    
                    // Suma lo que este participante pagó del bolsillo
                    TotalPagadoBolsillo = gastos.Where(g => g.ParticipanteId == p.Id).Sum(g => g.Monto),
                    
                    // Suma lo que se cargó a su cuenta en los detalles de deudas
                    TotalConsumidoDebe = gastos.SelectMany(g => g.DetallesGasto)
                                               .Where(dg => dg.ParticipanteId == p.Id)
                                               .Sum(dg => dg.MontoIndividual),
                    
                    // Suma las transferencias directas que mandó
                    TransferenciasEnviadas = pagos.Where(pago => pago.RemitenteId == p.Id).Sum(pago => pago.Monto),
                    
                    // Suma las transferencias directas que recibió de otros deudores
                    TransferenciasRecibidas = pagos.Where(pago => pago.DestinatarioId == p.Id).Sum(pago => pago.Monto)
                };

                resumenSaldos.Add(saldo);
            }

            return resumenSaldos;
        }

        private void ValidarPagoParaCreacion(PagoRequest request)
        {
            if (request.ParticipanteId <= 0) throw new ArgumentException("El Remitente debe ser válido");
            if (request.DestinatarioId <= 0) throw new ArgumentException("El Destinatario debe ser válido");
            if (request.ParticipanteId == request.DestinatarioId) throw new ArgumentException("Un participante no puede transferirse plata a sí mismo.");
            if (request.ViajeId <= 0) throw new ArgumentException("ViajeId debe ser válido");
            if (!request.Monto.HasValue || request.Monto.Value <= 0) throw new ArgumentException("El monto es requerido y debe ser mayor a 0");
            if (string.IsNullOrEmpty(request.Metodo)) throw new ArgumentException("El método de pago es requerido");
        }
    }
}