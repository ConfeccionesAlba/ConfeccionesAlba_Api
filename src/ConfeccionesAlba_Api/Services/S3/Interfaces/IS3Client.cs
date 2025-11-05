using ConfeccionesAlba_Api.Services.S3.Models;

namespace ConfeccionesAlba_Api.Services.S3.Interfaces;

public interface IS3Client
{
    Task<string> UploadImage(FileUploadRequest file);
    Task RemoveImage(string imageName);
}