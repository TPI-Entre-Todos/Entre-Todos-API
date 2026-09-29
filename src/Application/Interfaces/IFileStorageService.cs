namespace Application.Interfaces
{
    /// <summary>
    /// Almacenamiento de archivos. La capa Application no conoce el proveedor concreto:
    /// hoy es S3, y el contenedor se resuelve por configuración, así que agregar un
    /// segundo destino (por ejemplo, los comprobantes en un bucket privado) no obliga
    /// a cambiar los servicios que la consumen.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Sube un archivo a un contenedor público y devuelve la URL desde la que queda
        /// accesible sin autenticación.
        /// </summary>
        Task<string> SubirAsync(
            Stream contenido,
            string nombreArchivo,
            string contentType,
            string contenedor,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina un archivo previamente subido con <see cref="SubirAsync"/>, a partir de
        /// su URL pública. No falla si el archivo ya no existe.
        /// </summary>
        Task EliminarAsync(
            string url,
            string contenedor,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sube un archivo a un contenedor privado y devuelve la key del objeto (no una URL:
        /// el bucket no tiene acceso público, así que hay que firmar una URL para leerlo).
        /// </summary>
        Task<string> SubirPrivadoAsync(
            Stream contenido,
            string nombreArchivo,
            string contentType,
            string contenedor,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Genera una URL temporal para leer un objeto de un contenedor privado. No hace
        /// ninguna llamada de red: la firma se calcula localmente con las credenciales del SDK.
        /// </summary>
        string ObtenerUrlFirmada(string key, string contenedor, TimeSpan vigencia);

        /// <summary>
        /// Elimina un archivo previamente subido con <see cref="SubirPrivadoAsync"/>, a partir
        /// de su key. No falla si el archivo ya no existe.
        /// </summary>
        Task EliminarPorKeyAsync(
            string key,
            string contenedor,
            CancellationToken cancellationToken = default);
    }
}
