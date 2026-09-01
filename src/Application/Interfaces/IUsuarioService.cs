
using Application.Models.Requests;
using Domain.Entities;
using Application.Models;
using Domain.Enums;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IUsuarioService
    {
        List<UsuarioDto> GetAll();
        UsuarioDto GetById(int id);
        UsuarioDto Update(int id, UsuarioRequest request, int usuarioAutenticadoId, bool esAdmin);
        UsuarioDto CambiarRol(int id, Rol rol);
        void Delete(int id, int usuarioAutenticadoId, bool esAdmin);

        /// <summary>
        /// Resuelve el usuario local a partir de un id token de Cognito ya validado.
        /// Si todavía no existe, lo crea (JIT provisioning).
        /// </summary>
        UsuarioDto GetOrCreateFromToken(ClaimsPrincipal principal);
    }
}
