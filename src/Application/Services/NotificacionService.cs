using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;

        public NotificacionService(INotificacionRepository notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        public NotificacionDto CrearNotificacion(NotificacionCreateDto dto)
        {
            var nueva = new Notificacion(dto.UsuarioId, dto.Mensaje);

            var creada = _notificacionRepository.Add(nueva);

            return NotificacionDto.Create(creada);
        }

        public List<NotificacionDto> ObtenerPorUsuario(int usuarioId)
        {
            var lista = _notificacionRepository.GetByUsuarioId(usuarioId);
            return NotificacionDto.CreateList(lista);
        }

        public void MarcarComoLeida(int id)
        {
            var notificacion = _notificacionRepository.GetById(id);
            if (notificacion == null) throw new NotFoundException("Notificación no encontrada.");

            notificacion.Leida = true;
            _notificacionRepository.Update(notificacion);
        }
    }
}
