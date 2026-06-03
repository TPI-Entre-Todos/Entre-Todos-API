using Domain.Entities;

namespace Application.Models;

public class ViajeDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Moneda { get; set; }

    public DateTime FechaCreacion { get; set; }
    public static ViajeDto Create(Viaje viaje)
    {
        return new ViajeDto
        {
            Id = viaje.Id,
            Nombre = viaje.Nombre,
            Descripcion = viaje.Descripcion,
            Moneda = viaje.Moneda,
            FechaCreacion = viaje.FechaCreacion
        };
    }

    public static List<ViajeDto> CreateList(List<Viaje> viajes)
    {
        var dtos = new List<ViajeDto>();
        foreach (var viaje in viajes)
        {
            dtos.Add(Create(viaje));
        }
        return dtos;
    }
}
