using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Services
{
    public class GastoService : IGastoService
    {
        private readonly IGastoRepository _gastoRepository;
        private readonly IParticipanteViajeRepository _participanteViajeRepository;

        public GastoService(
            IGastoRepository gastoRepository,
            IParticipanteViajeRepository participanteViajeRepository)
        {
            _gastoRepository = gastoRepository;
            _participanteViajeRepository = participanteViajeRepository;
        }

        // ─── Creación por tipo ────────────────────────────────────────────────────

        public GastoDto CrearIgualitario(GastoIgualitarioRequest dto, int userId, bool esAdmin)
        {
            ValidarAccesoViaje(dto.ViajeId, userId, esAdmin);
            ValidarCabecera(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);

            if (dto.ParticipantesIds == null || dto.ParticipantesIds.Count == 0)
                throw new ArgumentException("Debe incluir al menos un participante.");

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.ParticipantesIds, participantesViaje);
            ValidarSinDuplicados(dto.ParticipantesIds);

            var montosCalculados = CalcularIgualitario(dto.Monto, dto.ParticipantesIds);
            return PersistirGasto(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Igualitario, montosCalculados);
        }

        public GastoDto CrearPorPorcentaje(GastoPorPorcentajeRequest dto, int userId, bool esAdmin)
        {
            ValidarAccesoViaje(dto.ViajeId, userId, esAdmin);
            ValidarCabecera(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);

            if (dto.Participantes == null || dto.Participantes.Count == 0)
                throw new ArgumentException("Debe incluir al menos un participante.");

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            if (dto.Participantes.Any(p => p.Porcentaje <= 0))
                throw new ArgumentException("Todos los porcentajes deben ser mayores a cero.");

            var sumaPorcentajes = dto.Participantes.Sum(p => p.Porcentaje);
            if (Math.Abs(sumaPorcentajes - 100) > 0.01m)
                throw new ArgumentException($"La suma de los porcentajes debe ser 100. Suma actual: {sumaPorcentajes}.");

            var montosCalculados = CalcularPorPorcentaje(dto.Monto, dto.Participantes);
            return PersistirGasto(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.PorPorcentaje, montosCalculados);
        }

        public GastoDto CrearPersonalizado(GastoPersonalizadoRequest dto, int userId, bool esAdmin)
        {
            ValidarAccesoViaje(dto.ViajeId, userId, esAdmin);
            ValidarCabecera(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);

            if (dto.Participantes == null || dto.Participantes.Count == 0)
                throw new ArgumentException("Debe incluir al menos un participante.");

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            if (dto.Participantes.Any(p => p.Monto <= 0))
                throw new ArgumentException("Todos los montos individuales deben ser mayores a cero.");

            var suma = dto.Participantes.Sum(p => p.Monto);
            if (Math.Abs(suma - dto.Monto) > 0.01m)
                throw new ArgumentException($"La suma de los montos individuales ({suma}) debe coincidir con el monto total ({dto.Monto}).");

            var montosCalculados = dto.Participantes.ToDictionary(p => p.ParticipanteId, p => p.Monto);
            return PersistirGasto(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Personalizado, montosCalculados);
        }

        // ─── Consulta ─────────────────────────────────────────────────────────────

        public List<GastoDto> ObtenerGastosPorViaje(int viajeId, int userId, bool esAdmin)
        {
            ValidarAccesoViaje(viajeId, userId, esAdmin);
            return GastoDto.CreateList(_gastoRepository.GetByViajeId(viajeId));
        }

        public GastoDto? ObtenerGastoPorId(int id, int userId, bool esAdmin)
        {
            var gasto = _gastoRepository.GetById(id);
            if (gasto == null) return null;

            ValidarAccesoViaje(gasto.ViajeId, userId, esAdmin);
            return GastoDto.Create(gasto);
        }

        // ─── Actualización (usa GastoConDetallesRequest genérico para edición) ───

        public GastoDto ActualizarGasto(int id, GastoConDetallesRequest dto, int userId, bool esAdmin)
        {
            var gastoExistente = _gastoRepository.GetById(id)
                ?? throw new ArgumentException("El gasto no existe.");

            ValidarAccesoViaje(gastoExistente.ViajeId, userId, esAdmin);

            if (dto.Detalles == null || dto.Detalles.Count == 0)
                throw new ArgumentException("Debe incluir al menos un participante en la división.");

            var participantesViaje = ObtenerParticipantesViaje(gastoExistente.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Detalles.Select(d => d.ParticipanteId).ToList(), participantesViaje);

            // Revertir saldos del gasto original
            var saldoReversal = CalcularCambiosSaldo(
                gastoExistente.ParticipanteId,
                -gastoExistente.Monto,
                gastoExistente.DetallesGasto.ToDictionary(d => d.ParticipanteId, d => -d.MontoDebe)
            );

            // Calcular nuevos montos según tipo de división
            var montosNuevos = CalcularMontosDesdeRequest(dto.TipoDivision, dto.Monto, dto.Detalles);

            // Combinar reversal + nuevos saldos
            var saldoNuevo = CalcularCambiosSaldo(dto.ParticipanteId, dto.Monto, montosNuevos);
            foreach (var (participanteId, delta) in saldoNuevo)
            {
                if (saldoReversal.ContainsKey(participanteId))
                    saldoReversal[participanteId] += delta;
                else
                    saldoReversal[participanteId] = delta;
            }

            gastoExistente.ParticipanteId = dto.ParticipanteId;
            gastoExistente.Descripcion = dto.Descripcion;
            gastoExistente.Monto = dto.Monto;
            gastoExistente.TipoDivision = dto.TipoDivision;
            gastoExistente.Categoria = dto.Categoria;
            gastoExistente.Comprobante = dto.Comprobante;
            if (dto.Fecha.HasValue) gastoExistente.Fecha = dto.Fecha.Value;

            gastoExistente.DetallesGasto.Clear();
            foreach (var (participanteId, monto) in montosNuevos)
            {
                decimal montoPagado = participanteId == dto.ParticipanteId ? monto : 0;

                gastoExistente.DetallesGasto.Add(new DetalleGasto
                {
                    ParticipanteId = participanteId,
                    MontoDebe = monto,
                    MontoPagado = montoPagado
                });
            }

            return GastoDto.Create(_gastoRepository.UpdateWithDetalles(gastoExistente, saldoReversal));
        }

        // ─── Baja ─────────────────────────────────────────────────────────────────

        public void EliminarGasto(int id, int userId, bool esAdmin)
        {
            var gasto = _gastoRepository.GetById(id)
                ?? throw new ArgumentException("El gasto no existe.");

            ValidarAccesoViaje(gasto.ViajeId, userId, esAdmin);
            _gastoRepository.DeleteWithSaldoReversal(id);
        }

        // ─── Helpers de cálculo ───────────────────────────────────────────────────

        private static Dictionary<int, decimal> CalcularIgualitario(decimal montoTotal, List<int> participantesIds)
        {
            int cantidad = participantesIds.Count;
            decimal montoBase = Math.Round(montoTotal / cantidad, 2, MidpointRounding.ToEven);
            decimal resto = montoTotal - montoBase * cantidad;

            var resultado = new Dictionary<int, decimal>();
            for (int i = 0; i < participantesIds.Count; i++)
                resultado[participantesIds[i]] = i == 0 ? montoBase + resto : montoBase;

            return resultado;
        }

        private static Dictionary<int, decimal> CalcularPorPorcentaje(decimal montoTotal, List<ParticipantePorcentajeItem> participantes)
        {
            var resultado = new Dictionary<int, decimal>();
            decimal totalAsignado = 0;

            for (int i = 0; i < participantes.Count; i++)
            {
                if (i == participantes.Count - 1)
                {
                    resultado[participantes[i].ParticipanteId] = montoTotal - totalAsignado;
                }
                else
                {
                    decimal monto = Math.Round(montoTotal * participantes[i].Porcentaje / 100, 2, MidpointRounding.ToEven);
                    resultado[participantes[i].ParticipanteId] = monto;
                    totalAsignado += monto;
                }
            }

            return resultado;
        }

        // Para actualización: calcula montos desde el request genérico
        private static Dictionary<int, decimal> CalcularMontosDesdeRequest(
            TipoDivision tipo, decimal montoTotal, List<DetalleGastoItemRequest> detalles)
        {
            return tipo switch
            {
                TipoDivision.Igualitario =>
                    CalcularIgualitario(montoTotal, detalles.Select(d => d.ParticipanteId).ToList()),

                TipoDivision.PorPorcentaje =>
                    CalcularPorPorcentaje(montoTotal, detalles
                        .Select(d => new ParticipantePorcentajeItem
                        {
                            ParticipanteId = d.ParticipanteId,
                            Porcentaje = d.Porcentaje ?? throw new ArgumentException(
                                $"Falta porcentaje para participante {d.ParticipanteId}.")
                        }).ToList()),

                TipoDivision.Personalizado =>
                    detalles.ToDictionary(
                        d => d.ParticipanteId,
                        d => d.MontoIndividual ?? throw new ArgumentException(
                            $"Falta monto para participante {d.ParticipanteId}.")),

                _ => throw new ArgumentException("Tipo de división no reconocido.")
            };
        }

        private static Dictionary<int, decimal> CalcularCambiosSaldo(
            int pagadorId, decimal montoTotal, Dictionary<int, decimal> montosIndividuales)
        {
            var cambios = new Dictionary<int, decimal> { [pagadorId] = montoTotal };

            foreach (var (participanteId, monto) in montosIndividuales)
            {
                if (cambios.ContainsKey(participanteId))
                    cambios[participanteId] -= monto;
                else
                    cambios[participanteId] = -monto;
            }

            return cambios;
        }

        // ─── Persistencia compartida ──────────────────────────────────────────────

        private GastoDto PersistirGasto(
            int viajeId, int pagadorId, string descripcion, decimal monto,
            DateTime? fecha, string? categoria, string? comprobante,
            TipoDivision tipoDivision, Dictionary<int, decimal> montosCalculados)
        {
            // Validar que el pagador pertenece al viaje
            var pagador = _participanteViajeRepository.GetById(pagadorId)
                ?? throw new ArgumentException("El participante que pagó no existe.");

            if (pagador.ViajeId != viajeId)
                throw new ArgumentException("El participante que pagó no pertenece a este viaje.");

            var gasto = new Gasto(viajeId, pagadorId, descripcion, monto, tipoDivision)
            {
                Categoria = categoria,
                Comprobante = comprobante
            };

            if (fecha.HasValue) gasto.Fecha = fecha.Value;

            foreach (var (participanteId, montoDebe) in montosCalculados)
            {
                // Si este participante es quien pagó el gasto, su parte ya está cubierta
                decimal montoPagado = participanteId == pagadorId ? montoDebe : 0;

                gasto.DetallesGasto.Add(new DetalleGasto
                {
                    ParticipanteId = participanteId,
                    MontoDebe = montoDebe,
                    MontoPagado = montoPagado
                });
            }

            var saldoChanges = CalcularCambiosSaldo(pagadorId, monto, montosCalculados);
            return GastoDto.Create(_gastoRepository.AddWithDetalles(gasto, saldoChanges));
        }

        // ─── Validaciones ─────────────────────────────────────────────────────────

        private void ValidarAccesoViaje(int viajeId, int userId, bool esAdmin)
        {
            if (esAdmin) return;
            var participante = _participanteViajeRepository.GetByIds(userId, viajeId);
            if (participante == null)
                throw new UnauthorizedAccessException("No estás autorizado para operar sobre este viaje.");
        }

        private Dictionary<int, ParticipanteViaje> ObtenerParticipantesViaje(int viajeId)
        {
            return _participanteViajeRepository.GetByViajeId(viajeId).ToDictionary(p => p.Id);
        }

        private static void ValidarCabecera(int viajeId, int participanteId, string descripcion, decimal monto)
        {
            if (viajeId <= 0)
                throw new ArgumentException("ViajeId inválido.");
            if (participanteId <= 0)
                throw new ArgumentException("ParticipanteId inválido.");
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción es obligatoria.");
            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");
        }

        private static void ValidarParticipantesPerteneceViaje(
            List<int> ids, Dictionary<int, ParticipanteViaje> participantesViaje)
        {
            foreach (var id in ids)
                if (!participantesViaje.ContainsKey(id))
                    throw new ArgumentException($"El participante {id} no pertenece al viaje.");
        }

        private static void ValidarSinDuplicados(List<int> ids)
        {
            if (ids.Count != ids.Distinct().Count())
                throw new ArgumentException("No se puede incluir al mismo participante más de una vez.");
        }
    }
}
