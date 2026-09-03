using Amazon.S3;
using Amazon.S3.Model;

using Application.Interfaces;

using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _configuration;

    public S3FileStorageService(IAmazonS3 s3, IConfiguration configuration)
    {
        _s3 = s3;
        _configuration = configuration;
    }

    public async Task<string> SubirAsync(
        Stream contenido,
        string nombreArchivo,
        string contentType,
        string contenedor,
        CancellationToken cancellationToken = default)
    {
        var bucket = ResolverBucket(contenedor);

        // El contenedor se usa también como prefijo de la clave para que coincida con el
        // alcance de la bucket policy (arn:...:bucket/avatars/*): así lo que se sube fuera
        // de ese prefijo no queda público por arrastre.
        var key = $"{contenedor}/{nombreArchivo}";

        var request = new PutObjectRequest
        {
            BucketName = bucket,
            Key = key,
            InputStream = contenido,
            ContentType = contentType
        };

        await _s3.PutObjectAsync(request, cancellationToken);

        return $"https://{bucket}.s3.{ResolverRegion()}.amazonaws.com/{key}";
    }

    public async Task EliminarAsync(
        string url,
        string contenedor,
        CancellationToken cancellationToken = default)
    {
        var bucket = ResolverBucket(contenedor);

        var key = ExtraerKey(url, bucket);
        if (key == null)
            return;

        // S3 no distingue borrar un objeto inexistente de uno que sí estaba: en ambos casos
        // responde OK. Eso hace que reemplazar un avatar sea idempotente sin chequeos previos.
        await _s3.DeleteObjectAsync(bucket, key, cancellationToken);
    }

    private string ResolverBucket(string contenedor)
    {
        return _configuration[$"S3:Buckets:{contenedor}"]
            ?? throw new InvalidOperationException(
                $"No hay bucket configurado para '{contenedor}'. Definí S3:Buckets:{contenedor}.");
    }

    private string ResolverRegion()
    {
        return _configuration["S3:Region"]
            ?? throw new InvalidOperationException("S3:Region no está configurado.");
    }

    /// <summary>
    /// Recupera la clave del objeto a partir de la URL pública. Devuelve null si la URL
    /// no pertenece a este bucket, para no borrar por accidente algo de otro origen.
    /// </summary>
    private static string? ExtraerKey(string url, string bucket)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return null;

        if (!uri.Host.StartsWith($"{bucket}.", StringComparison.OrdinalIgnoreCase))
            return null;

        var key = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));
        return string.IsNullOrEmpty(key) ? null : key;
    }
}
