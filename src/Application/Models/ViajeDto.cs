using Domain.Entities;

namespace Application.Models;

public class ViajeDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Moneda { get; set; }
    public string? PortadaUrl { get; set; }

    public DateTime FechaCreacion { get; set; }

    // La URL no sale de la entidad: el bucket es privado y se firma con vencimiento
    // recién acá, después de que el service ya validó que quien pide el viaje puede verlo.
    public static ViajeDto Create(Viaje viaje, string? portadaUrl = null)
    {
        return new ViajeDto
        {
            Id = viaje.Id,
            Nombre = viaje.Nombre,
            Descripcion = viaje.Descripcion,
            Moneda = viaje.Moneda,
            PortadaUrl = portadaUrl,
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
