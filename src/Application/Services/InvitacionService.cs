using System.Net;
using System.Net.Mail;
using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class InvitacionService : IInvitacionService
    {
        private readonly IInvitacionRepository _invitacionRepository;
        private readonly IEmailService _emailService;
        private readonly IParticipanteViajeRepository _participanteRepository;
        private readonly IViajeRepository _viajeRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly string _frontendBaseUrl;

        public InvitacionService(
            IInvitacionRepository invitacionRepository,
            IEmailService emailService,
            IParticipanteViajeRepository participanteRepository,
            IViajeRepository viajeRepository,
            IUsuarioRepository usuarioRepository,
            string frontendBaseUrl) // URL del front para el link del email (config: Frontend:BaseUrl)
        {
            _invitacionRepository = invitacionRepository;
            _emailService = emailService;
            _participanteRepository = participanteRepository;
            _viajeRepository = viajeRepository;
            _usuarioRepository = usuarioRepository;
            _frontendBaseUrl = frontendBaseUrl.TrimEnd('/');
        }

        public List<InvitacionDto> GetAll()
        {
            return InvitacionDto.CreateList(_invitacionRepository.GetAll());
        }

        public List<InvitacionDto> GetMias(int usuarioId)
        {
            var email = ObtenerUsuario(usuarioId).Email;

            return _invitacionRepository.GetAll()
                .Where(i => MismoEmail(i.EmailInvitado, email))
                .OrderByDescending(i => i.FechaCreacion)
                .Select(InvitacionDto.Create)
                .ToList();
        }

        public List<InvitacionDto> GetPorViaje(int viajeId, int usuarioId, bool esAdmin)
        {
            AsegurarPuedeGestionarViaje(viajeId, usuarioId, esAdmin);

            return _invitacionRepository.GetAll()
                .Where(i => i.ViajeId == viajeId)
                .OrderByDescending(i => i.FechaCreacion)
                .Select(i => InvitacionDto.Create(i).SinToken())
                .ToList();
        }

        public InvitacionDto? GetById(int id, int usuarioId, bool esAdmin)
        {
            var invitacion = _invitacionRepository.GetById(id)
                ?? throw new NotFoundException("Invitación no encontrada.");

            var esInvitado = MismoEmail(invitacion.EmailInvitado, ObtenerUsuario(usuarioId).Email);
            if (!esAdmin && !esInvitado && !EsOrganizador(invitacion.ViajeId, usuarioId))
                throw new Domain.Exceptions.UnauthorizedAccessException("No tenés acceso a esta invitación.");

            var dto = InvitacionDto.Create(invitacion);
            return esAdmin || esInvitado ? dto : dto.SinToken();
        }

        public InvitacionDto Add(InvitacionRequest request, int usuarioId, bool esAdmin)
        {
            if (request == null)
                throw new BadRequestException("Solicitud de invitación inválida.");

            var viaje = _viajeRepository.GetById(request.ViajeId)
                ?? throw new NotFoundException("Viaje no encontrado.");

            AsegurarPuedeGestionarViaje(viaje.Id, usuarioId, esAdmin);

            if (viaje.Estado == "Finalizado")
                throw new BadRequestException("El viaje está finalizado: no se pueden enviar invitaciones.");

            var email = NormalizarEmail(request.EmailInvitado);

            if (request.FechaExpiracion <= DateTime.UtcNow)
                throw new BadRequestException("La fecha de expiración debe ser futura.");

            // ¿Ya es parte del viaje?
            var usuarioInvitado = _usuarioRepository.GetUserByEmail(email);
            if (usuarioInvitado != null && _participanteRepository.GetByIds(usuarioInvitado.Id, viaje.Id) != null)
                throw new BadRequestException("Ese usuario ya es parte del viaje.");

            // ¿Ya tiene una invitación vigente?
            var yaInvitado = _invitacionRepository.GetAll().Any(i =>
                i.ViajeId == viaje.Id
                && MismoEmail(i.EmailInvitado, email)
                && i.Estado == EstadoInvitacion.Pendiente
                && i.FechaExpiracion > DateTime.UtcNow);
            if (yaInvitado)
                throw new BadRequestException("Ya hay una invitación pendiente para ese email en este viaje.");

            var invitacion = new Invitacion(viaje.Id, usuarioId, email, request.FechaExpiracion);
            _invitacionRepository.Add(invitacion);

            var invitador = ObtenerUsuario(usuarioId);
            EnviarEmail(invitacion, viaje, invitador.Nombre);

            // Se vuelve a leer para traer el nombre del viaje y del invitador.
            return InvitacionDto.Create(_invitacionRepository.GetById(invitacion.Id) ?? invitacion);
        }

        public InvitacionDto AceptarInvitacion(string token, int usuarioId)
        {
            var invitacion = ObtenerInvitacionRespondible(token, usuarioId);

            if (invitacion.FechaExpiracion < DateTime.UtcNow)
                throw new BadRequestException("La invitación está expirada.");

            var viaje = _viajeRepository.GetById(invitacion.ViajeId)
                ?? throw new NotFoundException("El viaje de la invitación ya no existe.");
            if (viaje.Estado == "Finalizado")
                throw new BadRequestException("El viaje está finalizado: ya no se puede ingresar.");

            if (_participanteRepository.GetByIds(usuarioId, invitacion.ViajeId) != null)
                throw new BadRequestException("Ya sos parte de este viaje.");

            _participanteRepository.Add(new ParticipanteViaje(usuarioId, invitacion.ViajeId, false));

            invitacion.Estado = EstadoInvitacion.Aceptada;
            invitacion.FechaRespuesta = DateTime.UtcNow;
            _invitacionRepository.Update(invitacion);

            return InvitacionDto.Create(invitacion);
        }

        public InvitacionDto RechazarInvitacion(string token, int usuarioId)
        {
            var invitacion = ObtenerInvitacionRespondible(token, usuarioId);

            invitacion.Estado = EstadoInvitacion.Rechazada;
            invitacion.FechaRespuesta = DateTime.UtcNow;
            _invitacionRepository.Update(invitacion);

            return InvitacionDto.Create(invitacion);
        }

        public void Delete(int id, int usuarioId, bool esAdmin)
        {
            var invitacion = _invitacionRepository.GetById(id)
                ?? throw new NotFoundException("Invitación no encontrada.");

            if (!esAdmin && invitacion.UsuarioInvitadorId != usuarioId && !EsOrganizador(invitacion.ViajeId, usuarioId))
                throw new Domain.Exceptions.UnauthorizedAccessException(
                    "Solo quien invitó o el organizador del viaje puede cancelar la invitación.");

            _invitacionRepository.Delete(id);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────

        // La invitación debe existir, estar pendiente y haber sido enviada al email del usuario.
        private Invitacion ObtenerInvitacionRespondible(string token, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new BadRequestException("Token inválido.");

            var invitacion = _invitacionRepository.GetByToken(token)
                ?? throw new NotFoundException("Invitación no encontrada.");

            if (!MismoEmail(invitacion.EmailInvitado, ObtenerUsuario(usuarioId).Email))
                throw new Domain.Exceptions.UnauthorizedAccessException("Esta invitación no fue enviada a tu email.");

            if (invitacion.Estado != EstadoInvitacion.Pendiente)
                throw new BadRequestException("La invitación ya fue respondida.");

            return invitacion;
        }

        private Usuario ObtenerUsuario(int usuarioId)
        {
            return _usuarioRepository.GetById(usuarioId)
                ?? throw new NotFoundException("Usuario no encontrado.");
        }

        private bool EsOrganizador(int viajeId, int usuarioId)
        {
            return _participanteRepository.GetByIds(usuarioId, viajeId)?.EsOrganizador == true;
        }

        private void AsegurarPuedeGestionarViaje(int viajeId, int usuarioId, bool esAdmin)
        {
            if (!esAdmin && !EsOrganizador(viajeId, usuarioId))
                throw new Domain.Exceptions.UnauthorizedAccessException(
                    "Solo el organizador del viaje puede gestionar sus invitaciones.");
        }

        private static bool MismoEmail(string? a, string? b)
        {
            return string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizarEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("El email del invitado es obligatorio.");

            var limpio = email.Trim();
            try
            {
                if (new MailAddress(limpio).Address != limpio)
                    throw new FormatException();
            }
            catch (FormatException)
            {
                throw new BadRequestException("El email del invitado no es válido.");
            }
            return limpio.ToLowerInvariant();
        }

        // En segundo plano: si el proveedor de email falla, la invitación igual queda creada
        // y el invitado la ve en la app al ingresar con ese email.
        private void EnviarEmail(Invitacion invitacion, Viaje viaje, string nombreInvitador)
        {
            var subject = $"{nombreInvitador} te invitó al viaje \"{viaje.Nombre}\" en Entre-Todos";
            var html = InvitacionEmailTemplate.Construir(
                nombreInvitador,
                viaje.Nombre,
                invitacion.EmailInvitado,
                invitacion.FechaExpiracion,
                $"{_frontendBaseUrl}/invitaciones",
                viaje.FechaInicio,
                viaje.FechaFin);

            _ = Task.Run(async () =>
            {
                try { await _emailService.EnviarEmailAsync(invitacion.EmailInvitado, subject, html); }
                catch (Exception ex)
                {
                    // La invitación ya está creada, pero el fallo del envío tiene que quedar visible.
                    Console.Error.WriteLine($"[Invitación] No se pudo enviar el email a {invitacion.EmailInvitado}: {ex.Message}");
                }
            });
        }
    }
}
