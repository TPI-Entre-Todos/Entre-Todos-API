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
        public async Task RegistrarGastoConDetallesAsync(DetalleGastoCreateDto dto)
        {
            // 1. Guardamos el Gasto Maestro usando las propiedades de tu DTO
            var nuevoGasto = new Gasto
            {
                // Si tu DetalleGastoCreateDto maneja la creación completa del gasto, mapeás sus propiedades acá:
                ParticipanteId = dto.ParticipanteId,
                Monto = dto.MontoIndividual,
                Fecha = DateTime.Now
                // Agregá acá ViajeId o Descripcion si tu DetalleGastoCreateDto los tiene adentro
            };
            var gastoCreado = await _gastoRepository.AddAsync(nuevoGasto);

            // 2. Mapeamos el detalle individual
            var nuevoDetalle = new DetalleGasto
            {
                GastoId = gastoCreado.Id,
                ParticipanteId = dto.ParticipanteId,
                MontoIndividual = dto.MontoIndividual
            };

            await _detalleRepository.AddAsync(nuevoDetalle);
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