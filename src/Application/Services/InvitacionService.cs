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

        public InvitacionService(IInvitacionRepository invitacionRepository, IEmailService emailService)
        {
            _invitacionRepository = invitacionRepository;
            _emailService = emailService;
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
            var invitacion = new Invitacion(request.ViajeId, request.UsuarioInvitadorId, request.EmailInvitado, request.FechaExpiracion);

            _invitacionRepository.Add(invitacion);
            // Enviar email de invitación en segundo plano (no bloquear)
            try
            {
                var subject = $"Invitación al viaje #{invitacion.ViajeId}";
                var link = $"/invitaciones/accept?token={invitacion.Token}";
                var html = $"<p>Has sido invitado al viaje #{invitacion.ViajeId}.</p><p>Usá este token: <strong>{invitacion.Token}</strong></p><p>Link: <a href=\"{link}\">Aceptar invitación</a></p>";
                _ = Task.Run(async () => await _emailService.EnviarEmailAsync(invitacion.EmailInvitado, subject, html));
            }
            catch
            {
                // No bloquear la creación por fallas en el envío de email
            }

            return InvitacionDto.Create(invitacion);
        }

        public void Delete(int id)
        {
            _invitacionRepository.Delete(id);
        }
    }
}
