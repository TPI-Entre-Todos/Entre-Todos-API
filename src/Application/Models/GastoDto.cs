using System;
using Domain.Entities;
using Domain.Enums;
namespace Application.Models
{
    public class GastoDto
    {
        public int ViajeId { get; set; }
        public int ParticipanteId { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public string Fecha { get; set; }
        public static GastoDto Create(Gasto gasto)
        {
            return new GastoDto
            {
                ViajeId = gasto.ViajeId,
                ParticipanteId = gasto.ParticipanteId,
                Descripcion = gasto.Descripcion,
                Monto = gasto.Monto,
                Fecha = gasto.Fecha.ToString("dd/MM/yyyy HH:mm:ss")
            };
        }

        public static List<GastoDto> CreateList(List<Gasto> gastos)
        {
            var dtos = new List<GastoDto>();
            foreach (var gasto in gastos)
            {
                dtos.Add(Create(gasto));
            }
            return dtos;
        }
    }
}