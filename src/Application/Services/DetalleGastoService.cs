using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class DetalleGastoService : IDetalleGastoService
    {
        private readonly IDetalleGastoRepository _detalleRepository;
        private readonly IGastoRepository _gastoRepository;

        public DetalleGastoService(IDetalleGastoRepository detalleRepository, IGastoRepository gastoRepository)
        {
            _detalleRepository = detalleRepository;
            _gastoRepository = gastoRepository;
        }

        // 👈 Usamos el DTO real que tenés en pantalla
        public DetalleGasto RegistrarGastoConDetalles(DetalleGastoRequest dto)
        {

            var nuevoDetalle = new DetalleGasto
            {
                Id = dto.Id,
                GastoId = gastoCreado.Id,
                ParticipanteId = dto.ParticipanteId,
                MontoIndividual = dto.MontoIndividual
            };

            _detalleRepository.Add(nuevoDetalle);
            return nuevoDetalle;
        }

        public List<DetalleGastoDto> ObtenerDetallesPorGasto(int gastoId)
        {
            var detalles = _detalleRepository.GetByGastoId(gastoId);

            return detalles.Select(d => new DetalleGastoDto
            {
                Id = d.Id,
                GastoId = d.GastoId,
                ParticipanteId = d.ParticipanteId,
                MontoIndividual = d.MontoIndividual
            }).ToList();
        }
    }
}