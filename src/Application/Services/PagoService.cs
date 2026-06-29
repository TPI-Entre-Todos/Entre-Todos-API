using Domain.Entities;
using Domain.Exceptions;
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
        private readonly IDetalleGastoRepository _detalleGastoRepository;
        private readonly IParticipanteViajeRepository _participanteRepository;
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IViajeRepository _viajeRepository;

        public PagoService(
            IPagoRepository pagoRepository,
            IGastoRepository gastoRepository,
            IDetalleGastoRepository detalleGastoRepository,
            IParticipanteViajeRepository participanteRepository,
            INotificacionRepository notificacionRepository,
            IUsuarioRepository usuarioRepository,
            IViajeRepository viajeRepository)
        {
            _pagoRepository = pagoRepository;
            _gastoRepository = gastoRepository;
            _detalleGastoRepository = detalleGastoRepository;
            _participanteRepository = participanteRepository;
            _notificacionRepository = notificacionRepository;
            _usuarioRepository = usuarioRepository;
            _viajeRepository = viajeRepository;
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


        public PagoDto PagarSimple(PagoSimpleRequest request)
        {
            ValidarPagoBase(request);
            if (request.DetalleGastoId <= 0)
                throw new BadRequestException("DetalleGastoId debe ser válido");

            if (_viajeRepository.GetById(request.ViajeId) == null)
                throw new NotFoundException("Viaje no encontrado");

            var remitente = _participanteRepository.GetById(request.ParticipanteId)
                ?? throw new NotFoundException("Participante remitente no encontrado");
            var destinatario = _participanteRepository.GetById(request.DestinatarioId)
                ?? throw new NotFoundException("Participante destinatario no encontrado");

            ValidarParticipantesDelViaje(remitente, destinatario, request.ViajeId);

            var pago = new Pago(
                request.ParticipanteId,
                request.DestinatarioId,
                request.ViajeId,
                request.Monto!.Value,
                request.Metodo,
                request.Comprobante
            );

            var detalles = ActualizarDetallesGasto(
                [new PagoDetalleGastoItem { DetalleGastoId = request.DetalleGastoId, Monto = request.Monto!.Value }],
                destinatario,
                request.ViajeId
            );
            pago.DetallesPagados = detalles;

            _pagoRepository.Add(pago);
            EnviarNotificacion(remitente, destinatario, pago);

            return PagoDto.Create(pago);
        }

        public PagoDto PagarMultiple(PagoMultipleRequest request)
        {
            ValidarPagoBase(request);
            if (request.DetallesPagados == null || request.DetallesPagados.Count == 0)
                throw new BadRequestException("Debe especificar al menos un DetalleGasto a pagar");

            ValidarMontoDetalles(request.DetallesPagados, request.Monto!.Value);

            if (_viajeRepository.GetById(request.ViajeId) == null)
                throw new NotFoundException("Viaje no encontrado");

            var remitente = _participanteRepository.GetById(request.ParticipanteId)
                ?? throw new NotFoundException("Participante remitente no encontrado");
            var destinatario = _participanteRepository.GetById(request.DestinatarioId)
                ?? throw new NotFoundException("Participante destinatario no encontrado");

            ValidarParticipantesDelViaje(remitente, destinatario, request.ViajeId);

            var pago = new Pago(
                request.ParticipanteId,
                request.DestinatarioId,
                request.ViajeId,
                request.Monto!.Value,
                request.Metodo,
                request.Comprobante
            );

            var detalles = ActualizarDetallesGasto(request.DetallesPagados, destinatario, request.ViajeId);
            pago.DetallesPagados = detalles;

            _pagoRepository.Add(pago);
            EnviarNotificacion(remitente, destinatario, pago);

            return PagoDto.Create(pago);
        }

        private List<DetalleGasto> ActualizarDetallesGasto(List<PagoDetalleGastoItem> detallesPagados, ParticipanteViaje destinatario, int viajeId)
        {
            var detallesEntidades = new List<DetalleGasto>();

            foreach (var detalle in detallesPagados)
            {
                if (detalle.Monto <= 0)
                    throw new BadRequestException($"El monto del ítem DetalleGasto {detalle.DetalleGastoId} debe ser mayor a 0");

                var detalleGasto = _detalleGastoRepository.GetById(detalle.DetalleGastoId)
                    ?? throw new NotFoundException($"DetalleGasto {detalle.DetalleGastoId} no encontrado");

                var gastoDelDetalle = _gastoRepository.GetById(detalleGasto.GastoId)
                    ?? throw new NotFoundException($"Gasto asociado al DetalleGasto {detalle.DetalleGastoId} no encontrado");

                if (gastoDelDetalle.ViajeId != viajeId)
                    throw new BadRequestException($"El DetalleGasto {detalle.DetalleGastoId} no pertenece al viaje indicado");

                if (detalleGasto.ParticipanteId != destinatario.Id)
                    throw new BadRequestException($"El DetalleGasto {detalle.DetalleGastoId} no corresponde al destinatario del pago");

                if (detalle.Monto > detalleGasto.SaldoPendiente)
                    throw new BadRequestException(
                        $"El monto a pagar ({detalle.Monto}) excede lo adeudado ({detalleGasto.SaldoPendiente}) en DetalleGasto {detalle.DetalleGastoId}");

                detalleGasto.MontoPagado += detalle.Monto;
                _detalleGastoRepository.Update(detalleGasto);

                var participanteDeudor = _participanteRepository.GetById(detalleGasto.ParticipanteId)
                    ?? throw new NotFoundException($"Participante {detalleGasto.ParticipanteId} no encontrado");
                participanteDeudor.SaldoTotal += detalle.Monto;
                _participanteRepository.Update(participanteDeudor);

                destinatario.SaldoTotal -= detalle.Monto;
                detallesEntidades.Add(detalleGasto);
            }

            _participanteRepository.Update(destinatario);
            return detallesEntidades;
        }

        public PagoDto ActualizarSimple(int id, PagoSimpleRequest request)
        {
            ValidarPagoBase(request);
            if (request.DetalleGastoId <= 0)
                throw new BadRequestException("DetalleGastoId debe ser válido");

            if (_viajeRepository.GetById(request.ViajeId) == null)
                throw new NotFoundException("Viaje no encontrado");

            var existing = _pagoRepository.GetById(id)
                ?? throw new NotFoundException("Pago no encontrado");

            var remitente = _participanteRepository.GetById(request.ParticipanteId)
                ?? throw new NotFoundException("Participante remitente no encontrado");
            var destinatarioNuevo = _participanteRepository.GetById(request.DestinatarioId)
                ?? throw new NotFoundException("Participante destinatario no encontrado");

            ValidarParticipantesDelViaje(remitente, destinatarioNuevo, request.ViajeId);

            var destinatarioAnterior = _participanteRepository.GetById(existing.DestinatarioId)
                ?? throw new NotFoundException("Participante destinatario anterior no encontrado");

            RevertirDetallesGasto(existing.DetallesPagados.ToList(), destinatarioAnterior);

            ActualizarCamposPago(existing, request);

            var detalles = ActualizarDetallesGasto(
                [new PagoDetalleGastoItem { DetalleGastoId = request.DetalleGastoId, Monto = request.Monto!.Value }],
                destinatarioNuevo,
                request.ViajeId
            );
            existing.DetallesPagados = detalles;

            EnviarNotificacion(remitente, destinatarioNuevo, existing);
            return PagoDto.Create(_pagoRepository.Update(existing));
        }

        public PagoDto ActualizarMultiple(int id, PagoMultipleRequest request)
        {
            ValidarPagoBase(request);
            if (request.DetallesPagados == null || request.DetallesPagados.Count == 0)
                throw new BadRequestException("Debe especificar al menos un DetalleGasto a pagar");

            ValidarMontoDetalles(request.DetallesPagados, request.Monto!.Value);

            if (_viajeRepository.GetById(request.ViajeId) == null)
                throw new NotFoundException("Viaje no encontrado");

            var existing = _pagoRepository.GetById(id)
                ?? throw new NotFoundException("Pago no encontrado");

            var remitente = _participanteRepository.GetById(request.ParticipanteId)
                ?? throw new NotFoundException("Participante remitente no encontrado");
            var destinatarioNuevo = _participanteRepository.GetById(request.DestinatarioId)
                ?? throw new NotFoundException("Participante destinatario no encontrado");

            ValidarParticipantesDelViaje(remitente, destinatarioNuevo, request.ViajeId);

            var destinatarioAnterior = _participanteRepository.GetById(existing.DestinatarioId)
                ?? throw new NotFoundException("Participante destinatario anterior no encontrado");

            RevertirDetallesGasto(existing.DetallesPagados.ToList(), destinatarioAnterior);

            ActualizarCamposPago(existing, request);

            var detalles = ActualizarDetallesGasto(request.DetallesPagados, destinatarioNuevo, request.ViajeId);
            existing.DetallesPagados = detalles;

            EnviarNotificacion(remitente, destinatarioNuevo, existing);
            return PagoDto.Create(_pagoRepository.Update(existing));
        }

        private void ActualizarCamposPago(Pago pago, PagoBaseRequest request)
        {
            pago.RemitenteId = request.ParticipanteId;
            pago.DestinatarioId = request.DestinatarioId;
            pago.ViajeId = request.ViajeId;
            pago.Monto = request.Monto!.Value;
            pago.Metodo = request.Metodo;
            if (!string.IsNullOrEmpty(request.Comprobante))
                pago.Comprobante = request.Comprobante;
        }

        private void RevertirDetallesGasto(List<DetalleGasto> detallesPrevios, ParticipanteViaje destinatarioAnterior)
        {
            foreach (var detalle in detallesPrevios)
            {
                var detalleGasto = _detalleGastoRepository.GetById(detalle.Id)
                    ?? throw new NotFoundException($"DetalleGasto {detalle.Id} no encontrado");

                detalleGasto.MontoPagado -= detalle.MontoPagado;
                if (detalleGasto.MontoPagado < 0) detalleGasto.MontoPagado = 0;
                _detalleGastoRepository.Update(detalleGasto);

                var participanteDeudor = _participanteRepository.GetById(detalleGasto.ParticipanteId)
                    ?? throw new NotFoundException($"Participante {detalleGasto.ParticipanteId} no encontrado");
                participanteDeudor.SaldoTotal -= detalle.MontoPagado;
                _participanteRepository.Update(participanteDeudor);

                destinatarioAnterior.SaldoTotal += detalle.MontoPagado;
            }

            _participanteRepository.Update(destinatarioAnterior);
        }

        public void Delete(int id)
        {
            var pago = _pagoRepository.GetById(id)
                ?? throw new NotFoundException("Pago no encontrado");

            if (pago.DetallesPagados.Count > 0)
            {
                var destinatario = _participanteRepository.GetById(pago.DestinatarioId)
                    ?? throw new NotFoundException("Participante destinatario no encontrado");

                RevertirDetallesGasto(pago.DetallesPagados.ToList(), destinatario);
            }

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
            var gastos = _gastoRepository.GetByViajeId(viajeId);

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
                                               .Sum(dg => dg.MontoDebe),

                    // Suma las transferencias directas que mandó
                    TransferenciasEnviadas = pagos.Where(pago => pago.RemitenteId == p.Id).Sum(pago => pago.Monto),

                    // Suma las transferencias directas que recibió de otros deudores
                    TransferenciasRecibidas = pagos.Where(pago => pago.DestinatarioId == p.Id).Sum(pago => pago.Monto)
                };

                resumenSaldos.Add(saldo);
            }

            return resumenSaldos;
        }

        private void ValidarPagoBase(PagoBaseRequest request)
        {
            if (request.ParticipanteId <= 0) throw new BadRequestException("El Remitente debe ser válido");
            if (request.DestinatarioId <= 0) throw new BadRequestException("El Destinatario debe ser válido");
            if (request.ParticipanteId == request.DestinatarioId) throw new BadRequestException("Un participante no puede transferirse plata a sí mismo.");
            if (request.ViajeId <= 0) throw new BadRequestException("ViajeId debe ser válido");
            if (!request.Monto.HasValue || request.Monto.Value <= 0) throw new BadRequestException("El monto es requerido y debe ser mayor a 0");
            if (string.IsNullOrEmpty(request.Metodo)) throw new BadRequestException("El método de pago es requerido");
        }

        private void ValidarParticipantesDelViaje(ParticipanteViaje remitente, ParticipanteViaje destinatario, int viajeId)
        {
            if (remitente.ViajeId != viajeId)
                throw new BadRequestException("El remitente no pertenece al viaje indicado");
            if (destinatario.ViajeId != viajeId)
                throw new BadRequestException("El destinatario no pertenece al viaje indicado");
            if (remitente.Estado != "Activo")
                throw new BadRequestException("El remitente no es un participante activo del viaje");
            if (destinatario.Estado != "Activo")
                throw new BadRequestException("El destinatario no es un participante activo del viaje");
        }

        private static void ValidarMontoDetalles(List<PagoDetalleGastoItem> detalles, decimal montoTotal)
        {
            var sumaDetalles = detalles.Sum(d => d.Monto);
            if (sumaDetalles != montoTotal)
                throw new BadRequestException(
                    $"La suma de los montos de los detalles ({sumaDetalles}) no coincide con el monto total del pago ({montoTotal})");
        }

        private void EnviarNotificacion(ParticipanteViaje remitente, ParticipanteViaje destinatario, Pago pago)
        {
            var usuario = _usuarioRepository.GetById(remitente.UsuarioId)
                ?? throw new NotFoundException($"Usuario del remitente no encontrado");
            var viaje = _viajeRepository.GetById(pago.ViajeId)
                ?? throw new NotFoundException("Viaje no encontrado al enviar notificación");
            var mensaje = $"Usuario {usuario.Nombre} cargó un pago de ${pago.Monto} en el viaje {viaje.Nombre}";
            _notificacionRepository.Add(new Notificacion(destinatario.UsuarioId, mensaje));
        }
    }
}
