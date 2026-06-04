using Application.Interfaces;
using Application.Models;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Enums;

namespace Application.Services
{
    public class InvitacionService : IInvitacionService
    {
        private readonly IInvitacionRepository _invitacionRepository;
        private readonly IEmailService _emailService;
        private readonly IParticipanteViajeRepository _participanteRepository;

        public InvitacionService(IInvitacionRepository invitacionRepository, IEmailService emailService, IParticipanteViajeRepository participanteRepository)
        {
            _invitacionRepository = invitacionRepository;
            _emailService = emailService;
            _participanteRepository = participanteRepository;
        }

        public List<InvitacionDto> GetAll()
        {
            var invitaciones = _invitacionRepository.GetAll();
            return InvitacionDto.CreateList(invitaciones);
        }

        public InvitacionDto? GetById(int id)
        {
            var invitacion = _invitacionRepository.GetById(id);
            return invitacion == null ? null : InvitacionDto.Create(invitacion);
        }

        public InvitacionDto Add(InvitacionRequest request)
        {
            // Enviar email de invitación en segundo plano (no bloquear)
            try
            {
                var invitacion = new Invitacion(request.ViajeId, request.UsuarioInvitadorId, request.EmailInvitado, request.FechaExpiracion);

                _invitacionRepository.Add(invitacion);
                var subject = $"Invitación al viaje #{invitacion.ViajeId}";
                var link = $"/invitaciones/accept?token={invitacion.Token}";
                var html = $"<p>Has sido invitado al viaje #{invitacion.ViajeId}.</p><p>Usá este token: <strong>{invitacion.Token}</strong></p><p>Link: <a href=\"{link}\">Aceptar invitación</a></p>";
                _ = Task.Run(async () => await _emailService.EnviarEmailAsync(invitacion.EmailInvitado, subject, html));

                return InvitacionDto.Create(invitacion);
            }
            catch
            {
                throw new Exception("Error al enviar el email de invitación. Por favor, intentá nuevamente.");
            }
        }

        public InvitacionDto AceptarInvitacion(string token, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token inválido.", nameof(token));

            var invitacion = _invitacionRepository.GetByToken(token);
            if (invitacion == null)
                throw new Exception("Invitación no encontrada.");

            if (invitacion.Estado != EstadoInvitacion.Pendiente)
                throw new Exception("La invitación ya fue respondida.");

            if (invitacion.FechaExpiracion < DateTime.UtcNow)
                throw new Exception("La invitación está expirada.");

            var participanteExistente = _participanteRepository.GetByIds(usuarioId, invitacion.ViajeId);
            if (participanteExistente != null)
                throw new Exception("El usuario ya se encuentra registrado en el viaje.");

            var participante = new ParticipanteViaje
            {
                UsuarioId = usuarioId,
                ViajeId = invitacion.ViajeId,
                EsOrganizador = false,
                SaldoTotal = 0,
                FechaIngreso = DateTime.UtcNow,
                Estado = "Activo",
                EstadoInvitacion = "Aceptada"
            };

            _participanteRepository.Add(participante);

            invitacion.Estado = EstadoInvitacion.Aceptada;
            invitacion.FechaRespuesta = DateTime.UtcNow;
            _invitacionRepository.Update(invitacion);

            return InvitacionDto.Create(invitacion);
        }

        public InvitacionDto RechazarInvitacion(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token inválido.", nameof(token));

            var invitacion = _invitacionRepository.GetByToken(token);
            if (invitacion == null)
                throw new Exception("Invitación no encontrada.");

            if (invitacion.Estado != EstadoInvitacion.Pendiente)
                throw new Exception("La invitación ya fue respondida.");

            invitacion.Estado = EstadoInvitacion.Rechazada;
            invitacion.FechaRespuesta = DateTime.UtcNow;
            _invitacionRepository.Update(invitacion);

            return InvitacionDto.Create(invitacion);
        }

        public void Delete(int id)
        {
            _invitacionRepository.Delete(id);
        }
    }
}
