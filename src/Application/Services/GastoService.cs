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
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IViajeRepository _viajeRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public GastoService(IGastoRepository gastoRepository, IParticipanteViajeRepository participanteViajeRepository, INotificacionRepository notificacionRepository, IViajeRepository viajeRepository, IUsuarioRepository usuarioRepository)
        {
            _gastoRepository = gastoRepository;
            _participanteViajeRepository = participanteViajeRepository;
            _notificacionRepository = notificacionRepository;
            _viajeRepository = viajeRepository;
            _usuarioRepository = usuarioRepository;
        }

        // ─── Creación ─────────────────────────────────────────────────────────────
        public GastoDto CrearIgualitario(GastoIgualitarioRequest dto, int userId, bool esAdmin)
        {
            var participantePagador = ValidarYObtenerParticipante(dto.ParticipanteId, userId, esAdmin);
            var viajeId = participantePagador.ViajeId;

            ValidarCabecera(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.ParticipantesIds);
            ValidarSinDuplicados(dto.ParticipantesIds);

            var participantesViaje = ObtenerParticipantesViaje(viajeId);
            ValidarParticipantesPerteneceViaje(dto.ParticipantesIds, participantesViaje);

            var montosCalculados = CalcularIgualitario(dto.Monto, dto.ParticipantesIds);
            return PersistirGasto(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Igualitario, montosCalculados);
        }

        public GastoDto CrearPorPorcentaje(GastoPorPorcentajeRequest dto, int userId, bool esAdmin)
        {
            var participantePagador = ValidarYObtenerParticipante(dto.ParticipanteId, userId, esAdmin);
            var viajeId = participantePagador.ViajeId;

            ValidarCabecera(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarPorcentajes(dto.Participantes);

            var participantesViaje = ObtenerParticipantesViaje(viajeId);
            var participanteIds = dto.Participantes.Select(p => p.ParticipanteId).ToList();
            ValidarParticipantesPerteneceViaje(participanteIds, participantesViaje);
            ValidarSinDuplicados(participanteIds);

            var montosCalculados = CalcularPorPorcentaje(dto.Monto, dto.Participantes);
            return PersistirGasto(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.PorPorcentaje, montosCalculados);
        }

        public GastoDto CrearPersonalizado(GastoPersonalizadoRequest dto, int userId, bool esAdmin)
        {
            var participantePagador = ValidarYObtenerParticipante(dto.ParticipanteId, userId, esAdmin);
            var viajeId = participantePagador.ViajeId;

            ValidarCabecera(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarMontosPersonalizados(dto.Participantes, dto.Monto);

            var participantesViaje = ObtenerParticipantesViaje(viajeId);
            var participanteIds = dto.Participantes.Select(p => p.ParticipanteId).ToList();
            ValidarParticipantesPerteneceViaje(participanteIds, participantesViaje);
            ValidarSinDuplicados(participanteIds);

            var montosCalculados = dto.Participantes.ToDictionary(p => p.ParticipanteId, p => p.Monto);
            return PersistirGasto(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Personalizado, montosCalculados);
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

        // ─── Actualización ────────────────────────────────────────────────────────
        public GastoDto ActualizarIgualitario(int id, GastoIgualitarioRequest dto, int userId, bool esAdmin)
        {
            var gastoExistente = ObtenerGastoValidado(id);
            ValidarPermisoModificacion(gastoExistente, userId, esAdmin);

            var participantePagador = ValidarYObtenerParticipante(dto.ParticipanteId, userId, esAdmin);
            var viajeId = participantePagador.ViajeId;

            ValidarCabecera(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.ParticipantesIds);
            ValidarSinDuplicados(dto.ParticipantesIds);

            var participantesViaje = ObtenerParticipantesViaje(viajeId);
            ValidarParticipantesPerteneceViaje(dto.ParticipantesIds, participantesViaje);

            var montosCalculados = CalcularIgualitario(dto.Monto, dto.ParticipantesIds);
            return PersistirActualizacion(gastoExistente, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Igualitario, montosCalculados);
        }

        public GastoDto ActualizarPorPorcentaje(int id, GastoPorPorcentajeRequest dto, int userId, bool esAdmin)
        {
            var gastoExistente = ObtenerGastoValidado(id);
            ValidarPermisoModificacion(gastoExistente, userId, esAdmin);

            var participantePagador = ValidarYObtenerParticipante(dto.ParticipanteId, userId, esAdmin);
            var viajeId = participantePagador.ViajeId;

            ValidarCabecera(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarPorcentajes(dto.Participantes);

            var participantesViaje = ObtenerParticipantesViaje(viajeId);
            var participanteIds = dto.Participantes.Select(p => p.ParticipanteId).ToList();
            ValidarParticipantesPerteneceViaje(participanteIds, participantesViaje);
            ValidarSinDuplicados(participanteIds);

            var montosCalculados = CalcularPorPorcentaje(dto.Monto, dto.Participantes);
            return PersistirActualizacion(gastoExistente, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.PorPorcentaje, montosCalculados);
        }

        public GastoDto ActualizarPersonalizado(int id, GastoPersonalizadoRequest dto, int userId, bool esAdmin)
        {
            var gastoExistente = ObtenerGastoValidado(id);
            ValidarPermisoModificacion(gastoExistente, userId, esAdmin);

            var participantePagador = ValidarYObtenerParticipante(dto.ParticipanteId, userId, esAdmin);
            var viajeId = participantePagador.ViajeId;

            ValidarCabecera(viajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);
            ValidarListaNoVacia(dto.Participantes);
            ValidarMontosPersonalizados(dto.Participantes, dto.Monto);

            var participantesViaje = ObtenerParticipantesViaje(viajeId);
            var participanteIds = dto.Participantes.Select(p => p.ParticipanteId).ToList();
            ValidarParticipantesPerteneceViaje(participanteIds, participantesViaje);
            ValidarSinDuplicados(participanteIds);

            var montosCalculados = dto.Participantes.ToDictionary(p => p.ParticipanteId, p => p.Monto);
            return PersistirActualizacion(gastoExistente, dto.ParticipanteId, dto.Descripcion, dto.Monto,
                dto.Fecha, dto.Categoria, dto.Comprobante, TipoDivision.Personalizado, montosCalculados);
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

            var gastoActualizado = _gastoRepository.UpdateWithDetalles(gastoExistente, saldoReversal);

            CrearNotificacionesActualizacionGasto(gastoActualizado);

            return GastoDto.Create(gastoActualizado);
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
            var gastoCreado = _gastoRepository.AddWithDetalles(gasto, saldoChanges);

            CrearNotificacionesGasto(gastoCreado);

            return GastoDto.Create(gastoCreado);
        }

        // ─── Autorización ─────────────────────────────────────────────────────────

        /// <summary>
        /// Valida que el ParticipanteId existe.
        /// Si no es admin, valida que pertenece al usuario autenticado.
        /// </summary>
        private ParticipanteViaje ValidarYObtenerParticipante(int participanteId, int userId, bool esAdmin)
        {
            var participante = _participanteViajeRepository.GetById(participanteId)
                ?? throw new BadRequestException("El participante no existe.");

            if (!esAdmin && participante.UsuarioId != userId)
                throw new Domain.Exceptions.UnauthorizedAccessException(
                    "No puedes crear un gasto en nombre de otro usuario.");

            return participante;
        }

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
        private void CrearNotificacionesGasto(Gasto gasto)
        {
            var viaje = _viajeRepository.GetById(gasto.ViajeId);
            var pagadorParticipante = _participanteViajeRepository.GetById(gasto.ParticipanteId);
            if (pagadorParticipante == null || viaje == null) return;

            var pagador = _usuarioRepository.GetById(pagadorParticipante.UsuarioId);
            if (pagador == null) return;

            var participantesViaje = _participanteViajeRepository.GetByViajeId(gasto.ViajeId);

            var mensaje = $"Usuario {pagador.Nombre} añadió un nuevo gasto en el viaje {viaje.Nombre}";
            foreach (var participante in participantesViaje)
            {
                if (participante.UsuarioId != pagador.Id)
                {
                    var notificacion = new Notificacion(participante.UsuarioId, mensaje);
                    _notificacionRepository.Add(notificacion);
                }
            }
        }

        private void CrearNotificacionesActualizacionGasto(Gasto gasto)
        {
            var viaje = _viajeRepository.GetById(gasto.ViajeId);
            var pagadorParticipante = _participanteViajeRepository.GetById(gasto.ParticipanteId);
            if (pagadorParticipante == null || viaje == null) return;

            var pagador = _usuarioRepository.GetById(pagadorParticipante.UsuarioId);
            if (pagador == null) return;
            var participantesViaje = _participanteViajeRepository.GetByViajeId(gasto.ViajeId);

            var mensaje = $"Usuario {pagador.Nombre} actualizó un gasto en el viaje {viaje.Nombre}";
            foreach (var participante in participantesViaje)
            {
                if (participante.UsuarioId != pagador.Id)
                {
                    var notificacion = new Notificacion(participante.UsuarioId, mensaje);
                    _notificacionRepository.Add(notificacion);
                }
            }
        }
    }
}