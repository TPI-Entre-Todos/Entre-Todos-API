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
        /// Sube un archivo y devuelve la URL pública desde la que queda accesible.
        /// </summary>
        Task<string> SubirAsync(
            Stream contenido,
            string nombreArchivo,
            string contentType,
            string contenedor,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina un archivo previamente subido a partir de su URL.
        /// No falla si el archivo ya no existe.
        /// </summary>
        Task EliminarAsync(
            string url,
            string contenedor,
            CancellationToken cancellationToken = default);
    }
}
