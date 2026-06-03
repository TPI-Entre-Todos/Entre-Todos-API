using System.ComponentModel.DataAnnotations;

namespace Application.Models.Requests;

public class ViajeRequest
{
    [Required]
    public string? Nombre { get; set; }

    [Required]
    public string? Descripcion { get; set; }

    [Required]
    public string? Moneda { get; set; }
}
