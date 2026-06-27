using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class GastoService : IGastoService
    {
        private readonly IGastoRepository _gastoRepository;
        private readonly IParticipanteViajeRepository _participanteViajeRepository;

        public GastoService(IGastoRepository gastoRepository, IParticipanteViajeRepository participanteViajeRepository)
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

        public List<GastoDto> ObtenerTodos(int userId, bool esAdmin)
        {
            var gastos = _gastoRepository.GetAll();

            if (!esAdmin)
            {
                var viajesDelUsuario = _participanteViajeRepository.GetByUsuarioId(userId)
                    .Select(pv => pv.ViajeId)
                    .ToList();

                gastos = gastos.Where(g => viajesDelUsuario.Contains(g.ViajeId)).ToList();
            }

            return GastoDto.CreateList(gastos);
        }

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

        // ─── Actualización como User ──────────────────────────────────────────────
        // El participanteId (quién pagó) se resuelve desde el token JWT

        public GastoDto ActualizarIgualitarioComoUser(int id, ActualizarGastoIgualitarioRequest dto, int userId)
        {
            var gasto = ObtenerGastoValidado(id);
            int participanteId = ResolverParticipanteId(gasto.ViajeId, userId);
            ValidarPermisoModificacion(gasto, userId, esAdmin: false);
            ValidarCabecera(gasto.ViajeId, participanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.ParticipantesIds);

            var participantesViaje = ObtenerParticipantesViaje(gasto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.ParticipantesIds, participantesViaje);
            ValidarSinDuplicados(dto.ParticipantesIds);

            var montosNuevos = CalcularIgualitario(dto.Monto, dto.ParticipantesIds);
            return PersistirActualizacion(gasto, participanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Igualitario, montosNuevos);
        }

        public GastoDto ActualizarPorPorcentajeComoUser(int id, ActualizarGastoPorPorcentajeRequest dto, int userId)
        {
            var gasto = ObtenerGastoValidado(id);
            int participanteId = ResolverParticipanteId(gasto.ViajeId, userId);
            ValidarPermisoModificacion(gasto, userId, esAdmin: false);
            ValidarCabecera(gasto.ViajeId, participanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarPorcentajes(dto.Participantes);

            var participantesViaje = ObtenerParticipantesViaje(gasto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            var montosNuevos = CalcularPorPorcentaje(dto.Monto, dto.Participantes);
            return PersistirActualizacion(gasto, participanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.PorPorcentaje, montosNuevos);
        }

        public GastoDto ActualizarPersonalizadoComoUser(int id, ActualizarGastoPersonalizadoRequest dto, int userId)
        {
            var gasto = ObtenerGastoValidado(id);
            int participanteId = ResolverParticipanteId(gasto.ViajeId, userId);
            ValidarPermisoModificacion(gasto, userId, esAdmin: false);
            ValidarCabecera(gasto.ViajeId, participanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarMontosPersonalizados(dto.Participantes, dto.Monto);

            var participantesViaje = ObtenerParticipantesViaje(gasto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            var montosNuevos = dto.Participantes.ToDictionary(p => p.ParticipanteId, p => p.Monto);
            return PersistirActualizacion(gasto, participanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Personalizado, montosNuevos);
        }

        // ─── Actualización como Admin ─────────────────────────────────────────────
        // El participanteId viene explícito en el request

        public GastoDto ActualizarIgualitarioComoAdmin(int id, ActualizarGastoIgualitarioAdminRequest dto)
        {
            var gasto = ObtenerGastoValidado(id);
            ValidarCabecera(gasto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.ParticipantesIds);

            var participantesViaje = ObtenerParticipantesViaje(gasto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.ParticipantesIds, participantesViaje);
            ValidarSinDuplicados(dto.ParticipantesIds);

            var montosNuevos = CalcularIgualitario(dto.Monto, dto.ParticipantesIds);
            return PersistirActualizacion(gasto, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Igualitario, montosNuevos);
        }

        public GastoDto ActualizarPorPorcentajeComoAdmin(int id, ActualizarGastoPorPorcentajeAdminRequest dto)
        {
            var gasto = ObtenerGastoValidado(id);
            ValidarCabecera(gasto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarPorcentajes(dto.Participantes);

            var participantesViaje = ObtenerParticipantesViaje(gasto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            var montosNuevos = CalcularPorPorcentaje(dto.Monto, dto.Participantes);
            return PersistirActualizacion(gasto, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.PorPorcentaje, montosNuevos);
        }

        public GastoDto ActualizarPersonalizadoComoAdmin(int id, ActualizarGastoPersonalizadoAdminRequest dto)
        {
            var gasto = ObtenerGastoValidado(id);
            ValidarCabecera(gasto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarMontosPersonalizados(dto.Participantes, dto.Monto);

            var participantesViaje = ObtenerParticipantesViaje(gasto.ViajeId);
            ValidarParticipantesPerteneceViaje(dto.Participantes.Select(p => p.ParticipanteId).ToList(), participantesViaje);
            ValidarSinDuplicados(dto.Participantes.Select(p => p.ParticipanteId).ToList());

            var montosNuevos = dto.Participantes.ToDictionary(p => p.ParticipanteId, p => p.Monto);
            return PersistirActualizacion(gasto, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Personalizado, montosNuevos);
        }

        // ─── Baja ─────────────────────────────────────────────────────────────────

        public void EliminarGasto(int id, int userId, bool esAdmin)
        {
            var gasto = _gastoRepository.GetById(id)
                ?? throw new NotFoundException("El gasto no existe.");

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

        private GastoDto PersistirActualizacion(
            Gasto gastoExistente, int nuevoPagadorId, string descripcion, decimal monto,
            DateTime? fecha, string? categoria, string? comprobante,
            TipoDivision tipoDivision, Dictionary<int, decimal> montosNuevos)
        {
            var pagador = _participanteViajeRepository.GetById(nuevoPagadorId)
                ?? throw new BadRequestException("El participante que pagó no existe.");

            if (pagador.ViajeId != gastoExistente.ViajeId)
                throw new BadRequestException("El participante que pagó no pertenece a este viaje.");

            // Revertir el saldo del gasto anterior
            var saldoReversal = CalcularCambiosSaldo(
                gastoExistente.ParticipanteId,
                -gastoExistente.Monto,
                gastoExistente.DetallesGasto.ToDictionary(d => d.ParticipanteId, d => -d.MontoDebe)
            );

            // Calcular el saldo del gasto nuevo
            var saldoNuevo = CalcularCambiosSaldo(nuevoPagadorId, monto, montosNuevos);

            // Combinar reversión + nuevo en un solo pase
            foreach (var (participanteId, delta) in saldoNuevo)
            {
                if (saldoReversal.ContainsKey(participanteId))
                    saldoReversal[participanteId] += delta;
                else
                    saldoReversal[participanteId] = delta;
            }

            gastoExistente.ParticipanteId = nuevoPagadorId;
            gastoExistente.Descripcion = descripcion;
            gastoExistente.Monto = monto;
            gastoExistente.TipoDivision = tipoDivision;
            gastoExistente.Categoria = categoria;
            gastoExistente.Comprobante = comprobante;
            if (fecha.HasValue) gastoExistente.Fecha = fecha.Value;

            // Obtener DetallesGasto actuales indexados por ParticipanteId
            var detallesActuales = gastoExistente.DetallesGasto.ToDictionary(d => d.ParticipanteId);

            // Actualizar DetallesGasto existentes y crear nuevos
            foreach (var (participanteId, montoDebe) in montosNuevos)
            {
                if (detallesActuales.TryGetValue(participanteId, out var detalleExistente))
                {
                    detalleExistente.MontoDebe = montoDebe;
                    detalleExistente.MontoPagado = participanteId == nuevoPagadorId ? montoDebe : 0;
                }
                else
                {
                    decimal montoPagado = participanteId == nuevoPagadorId ? montoDebe : 0;
                    gastoExistente.DetallesGasto.Add(new DetalleGasto(participanteId, montoDebe, montoPagado));
                }
            }

            // Eliminar DetallesGasto de participantes que ya no están en la nueva división
            var participantesAEliminar = detallesActuales.Keys
                .Where(id => !montosNuevos.ContainsKey(id))
                .ToList();

            foreach (var participanteId in participantesAEliminar)
            {
                var detalleAEliminar = gastoExistente.DetallesGasto.First(d => d.ParticipanteId == participanteId);
                gastoExistente.DetallesGasto.Remove(detalleAEliminar);
            }

            return GastoDto.Create(_gastoRepository.UpdateWithDetalles(gastoExistente, saldoReversal));
        }

        private Gasto ObtenerGastoValidado(int id)
        {
            return _gastoRepository.GetById(id)
                ?? throw new BadRequestException("El gasto no existe.");
        }

        private GastoDto PersistirGasto(
            int viajeId, int pagadorId, string descripcion, decimal monto,
            DateTime? fecha, string? categoria, string? comprobante,
            TipoDivision tipoDivision, Dictionary<int, decimal> montosCalculados)
        {
            var pagador = _participanteViajeRepository.GetById(pagadorId)
                ?? throw new BadRequestException("El participante que pagó no existe.");

            if (pagador.ViajeId != viajeId)
                throw new BadRequestException("El participante que pagó no pertenece a este viaje.");

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
                ?? throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");
            return participante.Id;
        }

        private void ValidarParticipanteDelViaje(int viajeId, int userId, bool esAdmin)
        {
            if (esAdmin) return;
            var participante = _participanteViajeRepository.GetByIds(userId, viajeId);
            if (participante == null)
                throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");
        }

        private void ValidarPermisoModificacion(Gasto gasto, int userId, bool esAdmin)
        {
            if (esAdmin) return;

            var participante = _participanteViajeRepository.GetByIds(userId, gasto.ViajeId)
                ?? throw new Domain.Exceptions.UnauthorizedAccessException("No pertenecés a este viaje.");

            if (participante.EsOrganizador) return;

            if (participante.Id != gasto.ParticipanteId)
                throw new Domain.Exceptions.UnauthorizedAccessException("Solo podés modificar tus propios gastos.");
        }

        // ─── Validaciones de datos ────────────────────────────────────────────────

        private Dictionary<int, ParticipanteViaje> ObtenerParticipantesViaje(int viajeId)
        {
            return _participanteViajeRepository.GetByViajeId(viajeId).ToDictionary(p => p.Id);
        }

        private static void ValidarCabecera(int viajeId, int participanteId, string descripcion, decimal monto)
        {
            if (viajeId <= 0) throw new BadRequestException("ViajeId inválido.");
            if (participanteId <= 0) throw new BadRequestException("ParticipanteId inválido.");
            if (string.IsNullOrWhiteSpace(descripcion)) throw new BadRequestException("La descripción es obligatoria.");
            if (monto <= 0) throw new BadRequestException("El monto debe ser mayor a cero.");
        }

        private static void ValidarListaNoVacia<T>(List<T> lista)
        {
            if (lista == null || lista.Count == 0)
                throw new BadRequestException("Debe incluir al menos un participante.");
        }

        private static void ValidarPorcentajes(List<ParticipantePorcentajeItem> participantes)
        {
            if (participantes.Any(p => p.Porcentaje <= 0))
                throw new BadRequestException("Todos los porcentajes deben ser mayores a cero.");

            var suma = participantes.Sum(p => p.Porcentaje);
            if (Math.Abs(suma - 100) > 0.01m)
                throw new BadRequestException($"La suma de los porcentajes debe ser 100. Suma actual: {suma}.");
        }

        private static void ValidarMontosPersonalizados(List<ParticipanteMontoItem> participantes, decimal montoTotal)
        {
            if (participantes.Any(p => p.Monto <= 0))
                throw new BadRequestException("Todos los montos individuales deben ser mayores a cero.");

            var suma = participantes.Sum(p => p.Monto);
            if (Math.Abs(suma - montoTotal) > 0.01m)
                throw new BadRequestException($"La suma de los montos individuales ({suma}) debe coincidir con el monto total ({montoTotal}).");
        }

        private static void ValidarParticipantesPerteneceViaje(List<int> ids, Dictionary<int, ParticipanteViaje> participantesViaje)
        {
            foreach (var id in ids)
            {
                if (!participantesViaje.ContainsKey(id))
                    throw new BadRequestException($"El participante {id} no pertenece al viaje.");
            }
        }

        private static void ValidarSinDuplicados(List<int> ids)
        {
            if (ids.Count != ids.Distinct().Count())
                throw new BadRequestException("No se puede incluir al mismo participante más de una vez.");
        }
    }
}
