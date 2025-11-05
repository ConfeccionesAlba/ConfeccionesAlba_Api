namespace ConfeccionesAlba_Api.Services.S3.Models;

public record FileUploadRequest(string FileName, Stream FileStream, string ContentType);