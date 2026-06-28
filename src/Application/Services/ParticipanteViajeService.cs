using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Application.Interfaces;
using Application.Models.Requests;

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
        public ParticipanteViajeDto RegistrarParticipante(ParticipanteViajeCreateRequest dto)
        {
            ValidarSolicitud(dto);

            var existe = _repository.GetByIds(dto.UsuarioId, dto.ViajeId);
            if (existe != null)
                throw new BadRequestException("El usuario ya se encuentra registrado o invitado.");

            var nuevo = new ParticipanteViaje(dto.UsuarioId, dto.ViajeId, dto.EsOrganizador);
            var creado = _repository.Add(nuevo);

            return ParticipanteViajeDto.Create(creado);
        }

        // 2. CONSULTA: Obtener por Viaje
        public List<ParticipanteViajeDto> ObtenerPorViaje(int viajeId, int usuarioId)
        {
            if (viajeId <= 0)
                throw new BadRequestException("Id de viaje inválido.");

            if (usuarioId <= 0)
                throw new UnauthorizedException("Usuario no autenticado.");

            var miembro = _repository.GetByIds(usuarioId, viajeId);
            if (miembro == null)
                throw new UnauthorizedException("No estás registrado en este viaje.");

            var lista = _repository.GetByViajeId(viajeId);
            return ParticipanteViajeDto.CreateList(lista);
        }

        public List<ParticipanteViajeDto> ObtenerPorViajeAdmin(int viajeId)
        {
            if (viajeId <= 0)
                throw new BadRequestException("Id de viaje inválido.");

            var lista = _repository.GetByViajeId(viajeId);
            return ParticipanteViajeDto.CreateList(lista);
        }

        // CONSULTA: Obtener todos los participantes
        public List<ParticipanteViajeDto> ObtenerTodos(int usuarioId, bool esAdmin)
        {
            if (!esAdmin && usuarioId <= 0)
                throw new UnauthorizedException("Usuario no autenticado.");

            List<ParticipanteViaje> lista;
            if (esAdmin)
                lista = _repository.ObtenerTodos();
            else
                lista = _repository.GetByUsuarioId(usuarioId);

            return ParticipanteViajeDto.CreateList(lista);
        }

        // 4. BAJA: Eliminar participante
        public void EliminarParticipante(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Id de participante inválido.");

            var participante = _repository.GetById(id);
            if (participante == null)
                throw new NotFoundException("Participante no encontrado.");

            if (participante.SaldoTotal != 0)
                throw new BadRequestException("No se puede eliminar un participante con saldos pendientes.");

            _repository.Delete(id);
        }

        private static void ValidarSolicitud(ParticipanteViajeCreateRequest dto)
        {
            if (dto == null)
                throw new BadRequestException("Solicitud de participante inválida.");

            if (dto.UsuarioId <= 0)
                throw new BadRequestException("UsuarioId inválido.");

            if (dto.ViajeId <= 0)
                throw new BadRequestException("ViajeId inválido.");
        }
    }
}
