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
        public GastoDto CrearGasto(GastoRequest dto)
        {
            if (dto.Monto <= 0) throw new Exception("El monto del gasto debe ser mayor a cero.");
            if (string.IsNullOrEmpty(dto.Descripcion)) throw new Exception("La descripción es obligatoria.");

            var nuevoGasto = new Gasto(dto.ViajeId, dto.ParticipanteId, dto.Descripcion, dto.Monto);


            var creado = _gastoRepository.Add(nuevoGasto);

            return GastoDto.Create(creado);
        }

        // 2. CONSULTA: Obtener gastos de un viaje
        public List<GastoDto> ObtenerGastosPorViaje(int viajeId)
        {
            var lista = _gastoRepository.GetByViajeId(viajeId);

            return GastoDto.CreateList(lista);
        }

        // 3. BAJA: Eliminar un gasto
        public void EliminarGasto(int id)
        {
            var gasto = _gastoRepository.GetById(id);
            if (gasto == null) throw new Exception("El gasto no existe.");

            _gastoRepository.Delete(id);
        }
    }
}