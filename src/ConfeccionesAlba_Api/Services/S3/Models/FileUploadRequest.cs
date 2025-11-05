namespace ConfeccionesAlba_Api.Services.S3.Models;

public record FileUploadRequest(string Key, Stream FileStream, string ContentType);