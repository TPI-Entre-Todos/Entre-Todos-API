using Domain.Exceptions;

namespace Application.Services
{
    /// <summary>
    /// Valida que un archivo subido sea realmente una imagen.
    /// </summary>
    public static class ValidadorImagen
    {
        public const long TamanioMaximoBytes = 2 * 1024 * 1024; // 2 MB

        /// <summary>
        /// Verifica tamaño y contenido, y devuelve la extensión que corresponde al formato
        /// real del archivo.
        /// </summary>
        /// <remarks>
        /// El formato se deduce de los primeros bytes del archivo y no del Content-Type ni
        /// de la extensión: los dos los controla el cliente y se pueden falsear para subir
        /// un ejecutable diciendo que es un JPEG.
        /// </remarks>
        public static string ValidarYObtenerExtension(Stream contenido, long tamanio)
        {
            if (tamanio <= 0)
                throw new BadRequestException("El archivo está vacío.");

            if (tamanio > TamanioMaximoBytes)
                throw new BadRequestException(
                    $"La imagen supera el tamaño máximo de {TamanioMaximoBytes / 1024 / 1024} MB.");

            var cabecera = LeerCabecera(contenido);

            if (EsJpeg(cabecera)) return ".jpg";
            if (EsPng(cabecera)) return ".png";
            if (EsWebp(cabecera)) return ".webp";

            throw new BadRequestException("El archivo no es una imagen válida. Formatos aceptados: JPEG, PNG y WebP.");
        }

        private static byte[] LeerCabecera(Stream contenido)
        {
            if (!contenido.CanSeek)
                throw new BadRequestException("No se pudo leer el archivo.");

            var buffer = new byte[12];
            contenido.Position = 0;
            var leidos = contenido.Read(buffer, 0, buffer.Length);
            contenido.Position = 0; // Se devuelve al inicio para que la subida lo lea completo.

            return leidos < buffer.Length ? buffer[..leidos] : buffer;
        }

        // FF D8 FF
        private static bool EsJpeg(byte[] b) =>
            b.Length >= 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF;

        // 89 P N G \r \n 1A \n
        private static bool EsPng(byte[] b) =>
            b.Length >= 8 && b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47
            && b[4] == 0x0D && b[5] == 0x0A && b[6] == 0x1A && b[7] == 0x0A;

        // "RIFF" .... "WEBP"
        private static bool EsWebp(byte[] b) =>
            b.Length >= 12
            && b[0] == 0x52 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x46
            && b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50;

        /// <summary>Content-Type que corresponde a cada extensión validada.</summary>
        public static string ContentTypePara(string extension) => extension switch
        {
            ".jpg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
