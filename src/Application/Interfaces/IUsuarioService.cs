
using Application.Models.Requests;
using Domain.Entities;
using Application.Models;

namespace Application.Interfaces
{
    public interface IUsuarioService
    {
        List<UsuarioDto> GetAll();
        UsuarioDto GetById(int id);
        UsuarioDto Add(UsuarioRequest request);
        UsuarioDto Update(int id, UsuarioRequest request);
        void Delete(int id);
    }
}