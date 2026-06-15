using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using Domain.Entities;
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
            if (existe != null) throw new Exception("El usuario ya se encuentra registrado o invitado.");

            var nuevo = new ParticipanteViaje(dto.UsuarioId, dto.ViajeId, dto.EsOrganizador);
            var creado = _repository.Add(nuevo);

            // Mapeamos directo acá sin usar funciones externas
            return ParticipanteViajeDto.Create(creado);
        }

        // 2. CONSULTA: Obtener por Viaje
        public List<ParticipanteViajeDto> ObtenerPorViaje(int viajeId)
        {
            var lista = _repository.GetByViajeId(viajeId);
            return ParticipanteViajeDto.CreateList(lista);
        }

        // CONSULTA: Obtener todos los participantes
        public List<ParticipanteViajeDto> ObtenerTodos()
        {
            var lista = _repository.ObtenerTodos();
            return ParticipanteViajeDto.CreateList(lista);
        }

        // 4. BAJA: Eliminar participante
        public void EliminarParticipante(int id)
        {
            var participante = _repository.GetById(id);
            if (participante == null) throw new Exception("Participante no encontrado.");

            if (participante.SaldoTotal != 0)
                throw new Exception("No se puede eliminar un participante con saldos pendientes.");

            _repository.Delete(id);
        }
        public void VerificarOrganizador(int usuarioId, int viajeId)
        {
            var participante = _repository.GetByIds(usuarioId, viajeId);
            if (participante == null)
                throw new Exception("No pertenece a este viaje.");

            if (!participante.EsOrganizador)
                throw new Exception("Solo el organizador puede hacer esta acción.");
        }

    }
}