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
            var existe = _repository.GetByIds(dto.UsuarioId, dto.ViajeId);
            if (existe != null) throw new BadRequestException("El usuario ya se encuentra registrado o invitado.");

            var nuevo = new ParticipanteViaje(dto.UsuarioId, dto.ViajeId, dto.EsOrganizador);
            var creado = _repository.Add(nuevo);

            // Mapeamos directo acá sin usar funciones externas
            return ParticipanteViajeDto.Create(creado);
        }

        // 2. CONSULTA: Obtener por Viaje
        public List<ParticipanteViajeDto> ObtenerPorViaje(int viajeId, int usuarioId)
        {
            var miembro = _repository.GetByIds(usuarioId, viajeId);
            if (miembro == null) throw new Domain.Exceptions.UnauthorizedAccessException("No estás registrado en este viaje.");

            var lista = _repository.GetByViajeId(viajeId);
            return ParticipanteViajeDto.CreateList(lista);
        }

        public List<ParticipanteViajeDto> ObtenerPorViajeAdmin(int viajeId)
        {
            var lista = _repository.GetByViajeId(viajeId);
            return ParticipanteViajeDto.CreateList(lista);
        }


        // CONSULTA: Obtener todos los participantes
        public List<ParticipanteViajeDto> ObtenerTodos(int usuarioId, bool esAdmin)
        {
            List<ParticipanteViaje> lista;
            if (esAdmin)
                lista = _repository.ObtenerTodos();
            else
                lista = _repository.GetByUsuarioId(usuarioId);

            return ParticipanteViajeDto.CreateList(lista);
        }

        // CAMBIAR ESTADO DE ORGANIZADOR
        public ParticipanteViajeDto CambiarEsOrganizador(int id, bool esOrganizador, int usuarioId, bool esAdmin)
        {
            var participante = _repository.GetById(id);
            if (participante == null) throw new NotFoundException("Participante no encontrado.");

            if (!esAdmin)
            {
                var organizadorDelViaje = _repository.GetByIds(usuarioId, participante.ViajeId);
                if (organizadorDelViaje == null || !organizadorDelViaje.EsOrganizador)
                    throw new UnauthorizedException("Solo administradores y organizadores del viaje pueden cambiar este estado.");
            }

            if (participante.EsOrganizador == esOrganizador)
                throw new BadRequestException($"El participante ya tiene asignado el estado de organizador: {esOrganizador}.");

            participante.EsOrganizador = esOrganizador;
            var actualizado = _repository.Update(participante);
            return ParticipanteViajeDto.Create(actualizado);
        }

        // 4. BAJA: Eliminar participante
        public void EliminarParticipante(int id)
        {
            var participante = _repository.GetById(id);
            if (participante == null) throw new NotFoundException("Participante no encontrado.");

            if (participante.SaldoTotal != 0)
                throw new BadRequestException("No se puede eliminar un participante con saldos pendientes.");

            _repository.Delete(id);
        }
    }
}
