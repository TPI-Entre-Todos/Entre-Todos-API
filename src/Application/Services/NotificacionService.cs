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
        private readonly INotificacionRepository _repository;

        public NotificacionService(INotificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<NotificacionDto> CrearNotificacionAsync(NotificacionCreateDto dto)
        {
            var nueva = new Notificacion
            {
                UsuarioId = dto.UsuarioId,
                Mensaje = dto.Mensaje,
                Fecha = DateTime.Now,
                Leida = false // Arranca sin leer
            };

            var creada = await _repository.AddAsync(nueva);

            return new NotificacionDto
            {
                Id = creada.Id,
                UsuarioId = creada.UsuarioId,
                Mensaje = creada.Mensaje,
                Fecha = creada.Fecha,
                Leida = creada.Leida
            };
        }

        public async Task<List<NotificacionDto>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            var lista = await _repository.GetByUsuarioIdAsync(usuarioId);
            return lista.Select(n => new NotificacionDto
            {
                Id = n.Id,
                UsuarioId = n.UsuarioId,
                Mensaje = n.Mensaje,
                Fecha = n.Fecha,
                Leida = n.Leida
            }).ToList();
        }

        public async Task MarcarComoLeidaAsync(int id)
        {
            var notificacion = await _repository.GetByIdAsync(id);
            if (notificacion == null) throw new NotFoundException("Notificación no encontrada.");

            notificacion.Leida = true;
            await _repository.UpdateAsync(notificacion);
        }
    }
}
