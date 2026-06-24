using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application
{
    public class GastoService : IGastoService
    {
        private readonly IGastoRepository _gastoRepository;

        public GastoService(IGastoRepository gastoRepository)
        {
            _gastoRepository = gastoRepository;
        }

        // 1. ALTA: Crear un gasto
        public async Task<GastoDto> CrearGastoAsync(GastoCreateDto dto)
        {
            if (dto.Monto <= 0) throw new Exception("El monto del gasto debe ser mayor a cero.");
            if (string.IsNullOrEmpty(dto.Descripcion)) throw new Exception("La descripción es obligatoria.");

            var nuevoGasto = new Gasto
            {
                ViajeId = dto.ViajeId,
                ParticipanteId = dto.ParticipanteId,
                Descripcion = dto.Descripcion,
                Monto = dto.Monto,
                Fecha = DateTime.Now // Se registra con la fecha actual
            };

            var creado = await _gastoRepository.AddAsync(nuevoGasto);

            return new GastoDto
            {
                Id = creado.Id,
                ViajeId = creado.ViajeId,
                ParticipanteId = creado.ParticipanteId,
                Descripcion = creado.Descripcion,
                Monto = creado.Monto,
                Fecha = creado.Fecha
            };
        }

        // 2. CONSULTA: Obtener gastos de un viaje
        public async Task<List<GastoDto>> ObtenerGastosPorViajeAsync(int viajeId)
        {
            var lista = await _gastoRepository.GetByViajeIdAsync(viajeId);

            return lista.Select(g => new GastoDto
            {
                Id = g.Id,
                ViajeId = g.ViajeId,
                ParticipanteId = g.ParticipanteId,
                Descripcion = g.Descripcion,
                Monto = g.Monto,
                Fecha = g.Fecha
            }).ToList();
        }

        // 3. BAJA: Eliminar un gasto
        public async Task EliminarGastoAsync(int id)
        {
            var gasto = await _gastoRepository.GetByIdAsync(id);
            if (gasto == null) throw new Exception("El gasto no existe.");

            await _gastoRepository.DeleteAsync(id);
        }
    }
}