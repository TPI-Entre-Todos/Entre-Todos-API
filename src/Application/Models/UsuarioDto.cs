using Domain.Entities;
using Domain.Enums;

namespace Application.Models;

public class UsuarioDto
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public Rol Rol { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime FechaRegistro { get; set; }

    public static UsuarioDto Create(Usuario usuario)
    {
        var dto = new UsuarioDto();
        dto.Id = usuario.Id;
        dto.Nombre = usuario.Nombre;
        dto.Email = usuario.Email;
        dto.Rol = usuario.Rol;
        dto.AvatarUrl = usuario.AvatarUrl;
        dto.FechaRegistro = usuario.FechaRegistro;
        return dto;
    }

    public static List<UsuarioDto> CreateList(List<Usuario> usuarios)
    {
        var dtos = new List<UsuarioDto>();
        foreach (var usuario in usuarios)
        {
            dtos.Add(Create(usuario));
        }
        return dtos;
    }

}