using Application.Interfaces;
using Application.Models;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class DetalleGastoService : IDetalleGastoService
    {
        private readonly IDetalleGastoRepository _detalleRepository;
        private readonly IGastoRepository _gastoRepository;
        private readonly IParticipanteViajeRepository _participanteViajeRepository;

        public DetalleGastoService(
            IDetalleGastoRepository detalleRepository,
            IGastoRepository gastoRepository,
            IParticipanteViajeRepository participanteViajeRepository)
        {
            _detalleRepository = detalleRepository;
            _gastoRepository = gastoRepository;
            _participanteViajeRepository = participanteViajeRepository;
        }

        public List<DetalleGastoDto> ObtenerDetallesPorGasto(int gastoId, int userId, bool esAdmin)
        {
            var gasto = _gastoRepository.GetById(gastoId)
                ?? throw new NotFoundException("El gasto no existe.");
            if (!esAdmin)
            {
                var participante = _participanteViajeRepository.GetByIds(userId, gasto.ViajeId);
                if (participante == null)
                    throw new Domain.Exceptions.UnauthorizedAccessException("No estás autorizado para ver los detalles de este gasto.");
            }
            var detalles = _detalleRepository.GetByGastoId(gastoId);
            return DetalleGastoDto.CreateList(detalles);
        }
    }
}
