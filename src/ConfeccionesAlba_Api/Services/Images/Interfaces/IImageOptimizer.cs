namespace ConfeccionesAlba_Api.Services.Images.Interfaces;

public interface IImageOptimizer
{
    Task<byte[]> OptimizeAsync(Stream inputStream, int maxWidth = 1280, int quality = 85);
}