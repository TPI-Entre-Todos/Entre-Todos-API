using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Application.Models
{
    public class ParticipanteViajeDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int ViajeId { get; set; }
        public bool EsOrganizador { get; set; }
        public decimal SaldoTotal { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Estado { get; set; } = string.Empty;

        public static ParticipanteViajeDto Create(ParticipanteViaje entity)
        {
            return new ParticipanteViajeDto
            {
                Id = entity.Id,
                UsuarioId = entity.UsuarioId,
                NombreUsuario = entity.Usuario?.Nombre ?? string.Empty,
                ViajeId = entity.ViajeId,
                EsOrganizador = entity.EsOrganizador,
                SaldoTotal = entity.SaldoTotal,
                FechaIngreso = entity.FechaIngreso,
                Estado = entity.Estado,
            };
        }

        public static List<ParticipanteViajeDto> CreateList(List<ParticipanteViaje> participantes)
        {
            var list = new List<ParticipanteViajeDto>();
            foreach (var p in participantes)
            {
                list.Add(Create(p));
            }
            return list;
        }
    }
}