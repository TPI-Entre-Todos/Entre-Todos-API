using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IDetalleGastoService
    {
        DetalleGasto RegistrarGastoConDetalles(DetalleGastoRequest dto);
        List<DetalleGastoDto> ObtenerDetallesPorGasto(int gastoId);
    }
}