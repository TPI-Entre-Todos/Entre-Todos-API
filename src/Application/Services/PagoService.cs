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
        private readonly IFileStorageService _fileStorage;

        private const string ContenedorComprobantes = "pagos";
        private const long TamanioMaximoComprobanteBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly TimeSpan VigenciaUrlComprobante = TimeSpan.FromMinutes(15);

        public PagoService(
            IPagoRepository pagoRepository,
            IGastoRepository gastoRepository,
            IDetalleGastoRepository detalleGastoRepository,
            IParticipanteViajeRepository participanteRepository,
            INotificacionRepository notificacionRepository,
            IUsuarioRepository usuarioRepository,
            IViajeRepository viajeRepository,
            IFileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
            _pagoRepository = pagoRepository;
            _gastoRepository = gastoRepository;
            _detalleGastoRepository = detalleGastoRepository;
            _participanteRepository = participanteRepository;
            _notificacionRepository = notificacionRepository;
            _usuarioRepository = usuarioRepository;
            _viajeRepository = viajeRepository;
        }

        // Todos los pagos de todos los viajes: el controller lo restringe a Admin.
        public List<PagoDto> GetAll()
        {
            List<Pago> pagos = _pagoRepository.GetAll();
            return PagoDto.CreateList(pagos);
        }

        public PagoDto GetById(int id, int usuarioId, bool esAdmin)
        {
            Pago pago = _pagoRepository.GetById(id);
            if (pago == null)
                throw new NotFoundException("Pago no encontrado");

            ValidarMiembroDelViaje(pago.ViajeId, usuarioId, esAdmin);
            return PagoDto.Create(pago);
        }


        public PagoDto PagarSimple(PagoSimpleRequest request, int usuarioId, bool esAdmin)
        {
            ValidarPagoBase(request);
            if (request.DetalleGastoId <= 0)
                throw new BadRequestException("DetalleGastoId debe ser válido");

            ValidarPermisoRegistro(request.ViajeId, request.ParticipanteId, request.DestinatarioId, usuarioId, esAdmin);
            ValidarGrupoPago(request.GrupoPagoId, request.ViajeId, usuarioId, esAdmin);
            ObtenerViajeActivo(request.ViajeId);

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
                remitente,
                destinatario,
                request.ViajeId
            );
            pago.GrupoPagoId = request.GrupoPagoId;
            pago.DetallesPagados = detalles;

            _pagoRepository.Add(pago);
            EnviarNotificacion(remitente, destinatario, pago);

            return PagoDto.Create(pago);
        }

        public PagoDto PagarMultiple(PagoMultipleRequest request, int usuarioId, bool esAdmin)
        {
            ValidarPagoBase(request);
            if (request.DetallesPagados == null || request.DetallesPagados.Count == 0)
                throw new BadRequestException("Debe especificar al menos un DetalleGasto a pagar");

            ValidarMontoDetalles(request.DetallesPagados, request.Monto!.Value);

            ValidarPermisoRegistro(request.ViajeId, request.ParticipanteId, request.DestinatarioId, usuarioId, esAdmin);
            ValidarGrupoPago(request.GrupoPagoId, request.ViajeId, usuarioId, esAdmin);
            ObtenerViajeActivo(request.ViajeId);

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

            var detalles = ActualizarDetallesGasto(request.DetallesPagados, remitente, destinatario, request.ViajeId);
            pago.GrupoPagoId = request.GrupoPagoId;
            pago.DetallesPagados = detalles;

            _pagoRepository.Add(pago);
            EnviarNotificacion(remitente, destinatario, pago);

            return PagoDto.Create(pago);
        }

        // El remitente es quien debe (dueño del detalle) y el destinatario quien pagó el gasto.
        private List<PagoDetalle> ActualizarDetallesGasto(List<PagoDetalleGastoItem> detallesPagados, ParticipanteViaje remitente, ParticipanteViaje destinatario, int viajeId)
        {
            var detallesEntidades = new List<PagoDetalle>();

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

                if (detalleGasto.ParticipanteId != remitente.Id)
                    throw new BadRequestException($"El DetalleGasto {detalle.DetalleGastoId} no corresponde al remitente del pago");

                if (gastoDelDetalle.ParticipanteId != destinatario.Id)
                    throw new BadRequestException($"El gasto del DetalleGasto {detalle.DetalleGastoId} no fue pagado por el destinatario");

                if (detalle.Monto > detalleGasto.SaldoPendiente)
                    throw new BadRequestException(
                        $"El monto a pagar ({detalle.Monto}) excede lo adeudado ({detalleGasto.SaldoPendiente}) en DetalleGasto {detalle.DetalleGastoId}");

                detalleGasto.MontoPagado += detalle.Monto;
                _detalleGastoRepository.Update(detalleGasto);

                remitente.SaldoTotal += detalle.Monto;
                destinatario.SaldoTotal -= detalle.Monto;
                detallesEntidades.Add(new PagoDetalle(detalleGasto.Id, detalle.Monto) { DetalleGasto = detalleGasto });
            }

            _participanteRepository.Update(remitente);
            _participanteRepository.Update(destinatario);
            return detallesEntidades;
        }

        public PagoDto ActualizarSimple(int id, PagoSimpleRequest request, int usuarioId, bool esAdmin)
        {
            ValidarPagoBase(request);
            if (request.DetalleGastoId <= 0)
                throw new BadRequestException("DetalleGastoId debe ser válido");

            var existing = _pagoRepository.GetById(id)
                ?? throw new NotFoundException("Pago no encontrado");

            ValidarEdicion(existing, request, usuarioId, esAdmin);
            ObtenerViajeActivo(request.ViajeId);

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
                remitente,
                destinatarioNuevo,
                request.ViajeId
            );
            ReemplazarDetalles(existing, detalles);

            EnviarNotificacion(remitente, destinatarioNuevo, existing);
            return PagoDto.Create(_pagoRepository.Update(existing));
        }

        public PagoDto ActualizarMultiple(int id, PagoMultipleRequest request, int usuarioId, bool esAdmin)
        {
            ValidarPagoBase(request);
            if (request.DetallesPagados == null || request.DetallesPagados.Count == 0)
                throw new BadRequestException("Debe especificar al menos un DetalleGasto a pagar");

            ValidarMontoDetalles(request.DetallesPagados, request.Monto!.Value);

            var existing = _pagoRepository.GetById(id)
                ?? throw new NotFoundException("Pago no encontrado");

            ValidarEdicion(existing, request, usuarioId, esAdmin);
            ObtenerViajeActivo(request.ViajeId);

            var remitente = _participanteRepository.GetById(request.ParticipanteId)
                ?? throw new NotFoundException("Participante remitente no encontrado");
            var destinatarioNuevo = _participanteRepository.GetById(request.DestinatarioId)
                ?? throw new NotFoundException("Participante destinatario no encontrado");

            ValidarParticipantesDelViaje(remitente, destinatarioNuevo, request.ViajeId);

            var destinatarioAnterior = _participanteRepository.GetById(existing.DestinatarioId)
                ?? throw new NotFoundException("Participante destinatario anterior no encontrado");

            RevertirDetallesGasto(existing.DetallesPagados.ToList(), destinatarioAnterior);

            ActualizarCamposPago(existing, request);

            var detalles = ActualizarDetallesGasto(request.DetallesPagados, remitente, destinatarioNuevo, request.ViajeId);
            ReemplazarDetalles(existing, detalles);

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
            pago.GrupoPagoId = request.GrupoPagoId ?? pago.GrupoPagoId;
        }

        // Revierte exactamente lo que aportó este pago a cada detalle (PagoDetalle.Monto),
        // sin tocar lo que hayan aportado otros pagos al mismo detalle.
        private void RevertirDetallesGasto(List<PagoDetalle> aplicaciones, ParticipanteViaje destinatarioAnterior)
        {
            foreach (var aplicacion in aplicaciones)
            {
                var detalleGasto = _detalleGastoRepository.GetById(aplicacion.DetalleGastoId)
                    ?? throw new NotFoundException($"DetalleGasto {aplicacion.DetalleGastoId} no encontrado");

                detalleGasto.MontoPagado -= aplicacion.Monto;
                if (detalleGasto.MontoPagado < 0) detalleGasto.MontoPagado = 0;
                _detalleGastoRepository.Update(detalleGasto);

                var participanteDeudor = _participanteRepository.GetById(detalleGasto.ParticipanteId)
                    ?? throw new NotFoundException($"Participante {detalleGasto.ParticipanteId} no encontrado");
                participanteDeudor.SaldoTotal -= aplicacion.Monto;
                _participanteRepository.Update(participanteDeudor);

                destinatarioAnterior.SaldoTotal += aplicacion.Monto;
            }

            _participanteRepository.Update(destinatarioAnterior);
        }

        private static void ReemplazarDetalles(Pago pago, List<PagoDetalle> nuevos)
        {
            pago.DetallesPagados.Clear();
            foreach (var detalle in nuevos)
                pago.DetallesPagados.Add(detalle);
        }

        public async Task DeleteAsync(int id, int usuarioId, bool esAdmin, CancellationToken ct = default)
        {
            var pago = _pagoRepository.GetById(id)
                ?? throw new NotFoundException("Pago no encontrado");

            ValidarPermisoGestion(pago, usuarioId, esAdmin, "eliminarlo");
            ObtenerViajeActivo(pago.ViajeId);

            // Los pagos de un mismo grupo (p. ej. una compensación) se eliminan juntos.
            var aEliminar = new List<Pago> { pago };
            if (pago.GrupoPagoId.HasValue)
                aEliminar.AddRange(_pagoRepository.GetByGrupoPagoId(pago.GrupoPagoId.Value)
                    .Where(p => p.Id != pago.Id && p.ViajeId == pago.ViajeId));

            foreach (var item in aEliminar)
            {
                if (item.DetallesPagados.Count > 0)
                {
                    var destinatario = _participanteRepository.GetById(item.DestinatarioId)
                        ?? throw new NotFoundException("Participante destinatario no encontrado");

                    RevertirDetallesGasto(item.DetallesPagados.ToList(), destinatario);
                }

                _pagoRepository.Delete(item.Id);
            }

            // Los archivos se borran recién con los pagos ya eliminados de la base: si algo
            // fallara antes, no queremos pagos que apunten a comprobantes inexistentes.
            // Un borrado fallido acá sólo deja un huérfano en el bucket privado.
            foreach (var key in aEliminar.Select(p => p.ComprobanteKey).Where(k => !string.IsNullOrEmpty(k)))
            {
                try { await _fileStorage.EliminarPorKeyAsync(key!, ContenedorComprobantes, CancellationToken.None); } catch { }
            }
        }

        // ─── Comprobante ──────────────────────────────────────────────────────────

        public async Task<PagoDto> SubirComprobanteAsync(
            int id, Stream contenido, long tamanio, int usuarioId, bool esAdmin, CancellationToken ct = default)
        {
            var pago = _pagoRepository.GetById(id)
                ?? throw new NotFoundException("Pago no encontrado");

            ValidarPermisoGestion(pago, usuarioId, esAdmin, "cargar su comprobante");
            ObtenerViajeActivo(pago.ViajeId);

            // El formato se deduce del contenido del archivo, no de la extensión ni del Content-Type.
            var extension = ValidadorImagen.ValidarComprobanteYObtenerExtension(
                contenido, tamanio, TamanioMaximoComprobanteBytes);

            var keyAnterior = pago.ComprobanteKey;
            var keyNueva = await _fileStorage.SubirPrivadoAsync(
                contenido, $"{Guid.NewGuid():N}{extension}",
                ValidadorImagen.ContentTypePara(extension), ContenedorComprobantes, ct);

            try
            {
                pago.ComprobanteKey = keyNueva;
                _pagoRepository.Update(pago);
            }
            catch
            {
                // La base no quedó actualizada: se descarta el archivo recién subido.
                try { await _fileStorage.EliminarPorKeyAsync(keyNueva, ContenedorComprobantes, CancellationToken.None); } catch { }
                throw;
            }

            if (!string.IsNullOrEmpty(keyAnterior))
            {
                try { await _fileStorage.EliminarPorKeyAsync(keyAnterior, ContenedorComprobantes, CancellationToken.None); } catch { }
            }

            return PagoDto.Create(pago);
        }

        // Cualquier participante del viaje (o un Admin) puede ver el comprobante de un pago.
        public string ObtenerUrlComprobante(int id, int usuarioId, bool esAdmin)
        {
            var pago = _pagoRepository.GetById(id)
                ?? throw new NotFoundException("Pago no encontrado");

            if (!esAdmin && _participanteRepository.GetByIds(usuarioId, pago.ViajeId) == null)
                throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");

            if (string.IsNullOrEmpty(pago.ComprobanteKey))
                throw new NotFoundException("Este pago no tiene comprobante.");

            return _fileStorage.ObtenerUrlFirmada(pago.ComprobanteKey, ContenedorComprobantes, VigenciaUrlComprobante);
        }

        // Puede gestionar un pago (eliminarlo o cargar su comprobante) quien lo hizo (el remitente),
        // un organizador del viaje o un Admin.
        private void ValidarPermisoGestion(Pago pago, int usuarioId, bool esAdmin, string accion)
        {
            if (esAdmin) return;

            var participante = _participanteRepository.GetByIds(usuarioId, pago.ViajeId)
                ?? throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");

            if (participante.EsOrganizador || participante.Id == pago.RemitenteId) return;

            throw new Domain.Exceptions.UnauthorizedAccessException(
                $"Solo quien hizo el pago, un organizador del viaje o un administrador puede {accion}.");
        }

        // Ver pagos: cualquier participante del viaje o un Admin.
        private void ValidarMiembroDelViaje(int viajeId, int usuarioId, bool esAdmin)
        {
            if (esAdmin) return;

            if (_participanteRepository.GetByIds(usuarioId, viajeId) == null)
                throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");
        }

        // Registrar un pago: alguna de las dos partes (quien paga o quien recibe), un organizador
        // del viaje o un Admin. Se admite a quien recibe para que pueda anotar "me pagó" y para
        // las compensaciones, donde quien las carga es remitente de un pago y destinatario del otro.
        private void ValidarPermisoRegistro(int viajeId, int remitenteId, int destinatarioId, int usuarioId, bool esAdmin)
        {
            if (esAdmin) return;

            var participante = _participanteRepository.GetByIds(usuarioId, viajeId)
                ?? throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");

            if (EsParteUOrganizador(participante, remitenteId, destinatarioId)) return;

            throw new Domain.Exceptions.UnauthorizedAccessException(
                "Solo podés registrar pagos en los que sos quien paga o quien recibe, salvo que seas organizador del viaje.");
        }

        private static bool EsParteUOrganizador(ParticipanteViaje participante, int remitenteId, int destinatarioId)
            => participante.EsOrganizador || participante.Id == remitenteId || participante.Id == destinatarioId;

        // Los pagos de un mismo grupo se eliminan juntos. Sin este control, alguien podría sumar
        // un pago propio a un grupo ajeno (los GrupoPagoId se ven en los GET) y, al borrarlo,
        // arrastrar los pagos de otros.
        private void ValidarGrupoPago(Guid? grupoPagoId, int viajeId, int usuarioId, bool esAdmin, int? pagoIdExcluido = null)
        {
            if (!grupoPagoId.HasValue) return;

            var existentes = _pagoRepository.GetByGrupoPagoId(grupoPagoId.Value)
                .Where(p => p.Id != pagoIdExcluido)
                .ToList();
            if (existentes.Count == 0) return; // Grupo nuevo.

            if (existentes.Any(p => p.ViajeId != viajeId))
                throw new BadRequestException("El grupo de pagos indicado pertenece a otro viaje.");

            if (esAdmin) return;

            var participante = _participanteRepository.GetByIds(usuarioId, viajeId)
                ?? throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");

            if (existentes.Any(p => !EsParteUOrganizador(participante, p.RemitenteId, p.DestinatarioId)))
                throw new Domain.Exceptions.UnauthorizedAccessException(
                    "No podés sumar este pago a un grupo con pagos en los que no participás.");
        }

        // Editar: hay que poder gestionar el pago actual, y el pago resultante tiene que ser uno
        // que el usuario podría registrar. No se permite moverlo a otro viaje.
        private void ValidarEdicion(Pago existing, PagoBaseRequest request, int usuarioId, bool esAdmin)
        {
            ValidarPermisoGestion(existing, usuarioId, esAdmin, "editarlo");

            if (existing.ViajeId != request.ViajeId)
                throw new BadRequestException("No se puede mover un pago a otro viaje.");

            ValidarPermisoRegistro(request.ViajeId, request.ParticipanteId, request.DestinatarioId, usuarioId, esAdmin);

            if (request.GrupoPagoId.HasValue && request.GrupoPagoId != existing.GrupoPagoId)
                ValidarGrupoPago(request.GrupoPagoId, request.ViajeId, usuarioId, esAdmin, existing.Id);
        }

        // Un viaje finalizado es de solo lectura: no admite altas ni cambios de pagos.
        private void ObtenerViajeActivo(int viajeId)
        {
            var viaje = _viajeRepository.GetById(viajeId)
                ?? throw new NotFoundException("Viaje no encontrado");

            if (viaje.Estado == "Finalizado")
                throw new BadRequestException("El viaje está finalizado. Reabrilo para registrar o modificar pagos.");
        }

        public List<PagoDto> GetByViajeId(int viajeId, int usuarioId, bool esAdmin)
        {
            ValidarMiembroDelViaje(viajeId, usuarioId, esAdmin);

            List<Pago> pagos = _pagoRepository.GetByViajeId(viajeId);
            return PagoDto.CreateList(pagos);
        }

        public List<PagoDto> GetByParticipanteId(int participanteId, int usuarioId, bool esAdmin)
        {
            var participante = _participanteRepository.GetById(participanteId)
                ?? throw new NotFoundException("Participante no encontrado");

            ValidarMiembroDelViaje(participante.ViajeId, usuarioId, esAdmin);

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
