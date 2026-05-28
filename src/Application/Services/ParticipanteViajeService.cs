using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;

namespace Application.Services
{
    public class ParticipanteViajeService : IParticipanteViajeService
    {
        private readonly IParticipanteViajeRepository _repository;

        public ParticipanteViajeService(IParticipanteViajeRepository repository)
        {
            _repository = repository;
        }

        // 1. ALTA: Registrar o invitar participante al viaje
        public async Task<ParticipanteViajeDto> RegistrarParticipanteAsync(ParticipanteViajeCreateDto dto)
        {
            var existe = await _repository.GetByIdsAsync(dto.UsuarioId, dto.ViajeId);
            if (existe != null) throw new Exception("El usuario ya se encuentra registrado o invitado.");

            var nuevo = new ParticipanteViaje
            {
                UsuarioId = dto.UsuarioId,
                ViajeId = dto.ViajeId,
                EsOrganizador = dto.EsOrganizador,
                SaldoTotal = 0,
                FechaIngreso = DateTime.Now,
                Estado = "Activo",
                EstadoInvitacion = dto.EsOrganizador ? "Aceptada" : "Pendiente"
            };

            var creado = await _repository.AddAsync(nuevo);

            // Mapeamos directo acá sin usar funciones externas
            return new ParticipanteViajeDto
            {
                Id = creado.Id,
                UsuarioId = creado.UsuarioId,
                ViajeId = creado.ViajeId,
                EsOrganizador = creado.EsOrganizador,
                SaldoTotal = creado.SaldoTotal,
                FechaIngreso = creado.FechaIngreso,
                Estado = creado.Estado,
                EstadoInvitacion = creado.EstadoInvitacion
            };
        }

        // 2. CONSULTA: Obtener por Viaje
        public async Task<List<ParticipanteViajeDto>> ObtenerPorViajeAsync(int viajeId)
        {
            var lista = await _repository.GetByViajeIdAsync(viajeId);

            // Usamos una línea directa de LINQ para convertir la lista a DTOs
            return lista.Select(p => new ParticipanteViajeDto
            {
                Id = p.Id,
                UsuarioId = p.UsuarioId,
                ViajeId = p.ViajeId,
                EsOrganizador = p.EsOrganizador,
                SaldoTotal = p.SaldoTotal,
                FechaIngreso = p.FechaIngreso,
                Estado = p.Estado,
                EstadoInvitacion = p.EstadoInvitacion
            }).ToList();
        }

        // 3. MODIFICACIÓN: Aceptar o rechazar invitación
        public async Task ResponderInvitacionAsync(int id, string nuevoEstado)
        {
            var participante = await _repository.GetByIdAsync(id);
            if (participante == null) throw new Exception("Participante no encontrado.");

            participante.EstadoInvitacion = nuevoEstado;
            if (nuevoEstado == "Rechazada")
            {
                participante.Estado = "Inactivo";
            }

            await _repository.UpdateAsync(participante);
        }

        // 4. BAJA: Eliminar participante
        public async Task EliminarParticipanteAsync(int id)
        {
            var participante = await _repository.GetByIdAsync(id);
            if (participante == null) throw new Exception("Participante no encontrado.");

            if (participante.SaldoTotal != 0)
                throw new Exception("No se puede eliminar un participante con saldos pendientes.");

            await _repository.DeleteAsync(id);
        }
    }
}