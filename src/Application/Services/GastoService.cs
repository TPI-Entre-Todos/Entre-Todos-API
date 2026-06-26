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

        // ─── Creación como User ───────────────────────────────────────────────────
        // El participanteId se resuelve buscando al usuario en el viaje

        public GastoDto CrearIgualitarioComoUser(GastoIgualitarioRequest dto, int userId)
        {
            int participanteId = ResolverParticipanteId(dto.ViajeId, userId);
            ValidarCabecera(dto.ViajeId, participanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.ParticipantesIds);

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.ParticipantesIds, participantesViaje);
            ValidarSinDuplicados(dto.ParticipantesIds);

            var montos = CalcularIgualitario(dto.Monto, dto.ParticipantesIds);
            return PersistirGasto(dto.ViajeId, participanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Igualitario, montos);
        }

        public GastoDto CrearPorPorcentajeComoUser(GastoPorPorcentajeRequest dto, int userId)
        {
            int participanteId = ResolverParticipanteId(dto.ViajeId, userId);
            ValidarCabecera(dto.ViajeId, participanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarPorcentajes(dto.Participantes);

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            var montos = CalcularPorPorcentaje(dto.Monto, dto.Participantes);
            return PersistirGasto(dto.ViajeId, participanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.PorPorcentaje, montos);
        }

        public GastoDto CrearPersonalizadoComoUser(GastoPersonalizadoRequest dto, int userId)
        {
            int participanteId = ResolverParticipanteId(dto.ViajeId, userId);
            ValidarCabecera(dto.ViajeId, participanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarMontosPersonalizados(dto.Participantes, dto.Monto);

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            var montos = dto.Participantes.ToDictionary(p => p.ParticipanteId, p => p.Monto);
            return PersistirGasto(dto.ViajeId, participanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Personalizado, montos);
        }

        // ─── Creación como Admin ──────────────────────────────────────────────────
        // El participanteId viene explícito en el request

        public GastoDto CrearIgualitarioComoAdmin(GastoIgualitarioAdminRequest dto)
        {
            ValidarCabecera(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.ParticipantesIds);

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.ParticipantesIds, participantesViaje);
            ValidarSinDuplicados(dto.ParticipantesIds);

            var montos = CalcularIgualitario(dto.Monto, dto.ParticipantesIds);
            return PersistirGasto(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Igualitario, montos);
        }

        public GastoDto CrearPorPorcentajeComoAdmin(GastoPorPorcentajeAdminRequest dto)
        {
            ValidarCabecera(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarPorcentajes(dto.Participantes);

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            var montos = CalcularPorPorcentaje(dto.Monto, dto.Participantes);
            return PersistirGasto(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.PorPorcentaje, montos);
        }

        public GastoDto CrearPersonalizadoComoAdmin(GastoPersonalizadoAdminRequest dto)
        {
            ValidarCabecera(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarMontosPersonalizados(dto.Participantes, dto.Monto);

            var participantesViaje = ObtenerParticipantesViaje(dto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            var montos = dto.Participantes.ToDictionary(p => p.ParticipanteId, p => p.Monto);
            return PersistirGasto(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Personalizado, montos);
        }

        // ─── Consulta ─────────────────────────────────────────────────────────────

        public List<GastoDto> ObtenerGastosPorViaje(int viajeId, int userId, bool esAdmin)
        {
            ValidarParticipanteDelViaje(viajeId, userId, esAdmin);
            return GastoDto.CreateList(_gastoRepository.GetByViajeId(viajeId));
        }

        public GastoDto? ObtenerGastoPorId(int id, int userId, bool esAdmin)
        {
            var gasto = _gastoRepository.GetById(id);
            if (gasto == null) return null;

            ValidarParticipanteDelViaje(gasto.ViajeId, userId, esAdmin);
            return GastoDto.Create(gasto);
        }

        // ─── Actualización ────────────────────────────────────────────────────────

        public GastoDto ActualizarGasto(int id, GastoConDetallesRequest dto, int userId, bool esAdmin)
        {
            var gastoExistente = _gastoRepository.GetById(id)
                ?? throw new ArgumentException("El gasto no existe.");

            ValidarPermisoModificacion(gastoExistente, userId, esAdmin);

            if (dto.Detalles == null || dto.Detalles.Count == 0)
                throw new ArgumentException("Debe incluir al menos un participante en la división.");

            var participantesViaje = ObtenerParticipantesViaje(gastoExistente.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Detalles.Select(d => d.ParticipanteId).ToList(), participantesViaje);

            var saldoReversal = CalcularCambiosSaldo(
                gastoExistente.ParticipanteId,
                -gastoExistente.Monto,
                gastoExistente.DetallesGasto.ToDictionary(d => d.ParticipanteId, d => -d.MontoDebe)
            );

            var montosNuevos = CalcularMontosDesdeRequest(dto.TipoDivision, dto.Monto, dto.Detalles);
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
                gastoExistente.DetallesGasto.Add(new DetalleGasto(participanteId, monto, montoPagado));
            }

            return GastoDto.Create(_gastoRepository.UpdateWithDetalles(gastoExistente, saldoReversal));
        }

        // ─── Baja ─────────────────────────────────────────────────────────────────

        public void EliminarGasto(int id, int userId, bool esAdmin)
        {
            var gasto = _gastoRepository.GetById(id)
                ?? throw new ArgumentException("El gasto no existe.");

            ValidarPermisoModificacion(gasto, userId, esAdmin);
            _gastoRepository.DeleteWithSaldoReversal(id);
        }

        // ─── Helpers de cálculo ───────────────────────────────────────────────────

        private static Dictionary<int, decimal> CalcularIgualitario(decimal montoTotal, List<int> ids)
        {
            int cantidad = ids.Count;
            decimal montoBase = Math.Round(montoTotal / cantidad, 2, MidpointRounding.ToEven);
            decimal resto = montoTotal - (montoBase * cantidad);

            var resultado = new Dictionary<int, decimal>();
            for (int i = 0; i < ids.Count; i++)
                resultado[ids[i]] = i == 0 ? montoBase + resto : montoBase;

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
            var pagador = _participanteViajeRepository.GetById(pagadorId)
                ?? throw new ArgumentException("El participante que pagó no existe.");

            if (pagador.ViajeId != viajeId)
                throw new ArgumentException("El participante que pagó no pertenece a este viaje.");

            var gasto = new Gasto(viajeId, pagadorId, descripcion, monto, tipoDivision, categoria, comprobante);
            if (fecha.HasValue) gasto.Fecha = fecha.Value;

            foreach (var (participanteId, montoDebe) in montosCalculados)
            {
                decimal montoPagado = participanteId == pagadorId ? montoDebe : 0;
                gasto.DetallesGasto.Add(new DetalleGasto(participanteId, montoDebe, montoPagado));
            }

            var saldoChanges = CalcularCambiosSaldo(pagadorId, monto, montosCalculados);
            return GastoDto.Create(_gastoRepository.AddWithDetalles(gasto, saldoChanges));
        }

        // ─── Autorización ─────────────────────────────────────────────────────────

        /// <summary>
        /// Resuelve el ParticipanteViaje.Id del usuario autenticado dentro del viaje.
        /// Lanza UnauthorizedAccessException si el usuario no pertenece al viaje.
        /// </summary>
        private int ResolverParticipanteId(int viajeId, int userId)
        {
            var participante = _participanteViajeRepository.GetByIds(userId, viajeId)
                ?? throw new UnauthorizedAccessException("No pertenecés a este viaje.");
            return participante.Id;
        }

        private void ValidarParticipanteDelViaje(int viajeId, int userId, bool esAdmin)
        {
            if (esAdmin) return;
            var participante = _participanteViajeRepository.GetByIds(userId, viajeId);
            if (participante == null)
                throw new UnauthorizedAccessException("No pertenecés a este viaje.");
        }

        private void ValidarPermisoModificacion(Gasto gasto, int userId, bool esAdmin)
        {
            if (esAdmin) return;

            var participante = _participanteViajeRepository.GetByIds(userId, gasto.ViajeId)
                ?? throw new UnauthorizedAccessException("No pertenecés a este viaje.");

            if (participante.EsOrganizador) return;

            if (participante.Id != gasto.ParticipanteId)
                throw new UnauthorizedAccessException("Solo podés modificar tus propios gastos.");
        }

        // ─── Validaciones de datos ────────────────────────────────────────────────

        private Dictionary<int, ParticipanteViaje> ObtenerParticipantesViaje(int viajeId)
        {
            return _participanteViajeRepository.GetByViajeId(viajeId).ToDictionary(p => p.Id);
        }

        private static void ValidarCabecera(int viajeId, int participanteId, string descripcion, decimal monto)
        {
            if (viajeId <= 0) throw new ArgumentException("ViajeId inválido.");
            if (participanteId <= 0) throw new ArgumentException("ParticipanteId inválido.");
            if (string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("La descripción es obligatoria.");
            if (monto <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");
        }

        private static void ValidarListaNoVacia<T>(List<T> lista)
        {
            if (lista == null || lista.Count == 0)
                throw new ArgumentException("Debe incluir al menos un participante.");
        }

        private static void ValidarPorcentajes(List<ParticipantePorcentajeItem> participantes)
        {
            if (participantes.Any(p => p.Porcentaje <= 0))
                throw new ArgumentException("Todos los porcentajes deben ser mayores a cero.");

            var suma = participantes.Sum(p => p.Porcentaje);
            if (Math.Abs(suma - 100) > 0.01m)
                throw new ArgumentException($"La suma de los porcentajes debe ser 100. Suma actual: {suma}.");
        }

        private static void ValidarMontosPersonalizados(List<ParticipanteMontoItem> participantes, decimal montoTotal)
        {
            if (participantes.Any(p => p.Monto <= 0))
                throw new ArgumentException("Todos los montos individuales deben ser mayores a cero.");

            var suma = participantes.Sum(p => p.Monto);
            if (Math.Abs(suma - montoTotal) > 0.01m)
                throw new ArgumentException($"La suma de los montos individuales ({suma}) debe coincidir con el monto total ({montoTotal}).");
        }

        private static void ValidarParticipantesPerteneceViaje(List<int> ids, Dictionary<int, ParticipanteViaje> participantesViaje)
        {
            foreach (var id in ids)
            {
                if (!participantesViaje.ContainsKey(id))
                    throw new ArgumentException($"El participante {id} no pertenece al viaje.");
            }
        }

        private static void ValidarSinDuplicados(List<int> ids)
        {
            if (ids.Count != ids.Distinct().Count())
                throw new ArgumentException("No se puede incluir al mismo participante más de una vez.");
        }
    }
}