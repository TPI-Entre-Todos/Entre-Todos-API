using System.ComponentModel.DataAnnotations;

namespace Application.Models.Requests;

public class ViajeRequest
{

    public string? Nombre { get; set; }


    public string? Descripcion { get; set; }


    public string? Moneda { get; set; }
}
