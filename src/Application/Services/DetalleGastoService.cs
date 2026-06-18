using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application
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

        public async Task RegistrarGastoConDetallesAsync(GastoConDetallesCreateDto dto)
        {
            if (dto.MontoTotal <= 0) throw new Exception("El monto total debe ser mayor a cero.");

            // 1. Guardamos el Gasto Maestro
            var nuevoGasto = new Gasto
            {
                ViajeId = dto.ViajeId,
                ParticipanteId = dto.PagadorParticipanteId,
                Descripcion = dto.Descripcion,
                Monto = dto.MontoTotal,
                Fecha = DateTime.Now
            };
            var gastoCreado = await _gastoRepository.AddAsync(nuevoGasto);

            // 2. Mapeamos los detalles (Corregido sin firmas de propiedades)
            var detallesEntities = dto.Divisiones.Select(d => new DetalleGasto
            {
                GastoId = gastoCreado.Id,
                ParticipanteId = d.ParticipanteId,
                MontoIndividual = d.MontoIndividual
            }).ToList();

            await _detalleRepository.AddRangeAsync(detallesEntities);
        }

        public async Task<List<DetalleGastoDto>> ObtenerDetallesPorGastoAsync(int gastoId)
        {
            var detalles = await _detalleRepository.GetByGastoIdAsync(gastoId);
            
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