
using Application.Models.Requests;
using Domain.Entities;
using Application.Models;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IUsuarioService
    {
        List<UsuarioDto> GetAll();
        UsuarioDto GetById(int id);
        UsuarioDto Add(UsuarioRequest request);
        UsuarioDto Update(int id, UsuarioRequest request);
        UsuarioDto CambiarRol(int id, Rol rol);
        void Delete(int id);
    }
}