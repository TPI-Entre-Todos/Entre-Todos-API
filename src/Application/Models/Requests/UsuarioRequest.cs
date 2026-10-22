namespace Application.Models.Requests
{
    /// <summary>
    /// El email y la contraseña son administrados por Cognito: desde la API sólo se
    /// edita el nombre para mostrar.
    /// </summary>
    public class UsuarioRequest
    {
        public string? Nombre { get; set; }
    }
}
