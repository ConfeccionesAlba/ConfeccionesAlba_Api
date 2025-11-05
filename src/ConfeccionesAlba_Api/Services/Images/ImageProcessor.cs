using ConfeccionesAlba_Api.Services.Images.Interfaces;
using ConfeccionesAlba_Api.Services.S3.Interfaces;
using ConfeccionesAlba_Api.Services.S3.Models;

namespace ConfeccionesAlba_Api.Services.Images;

public class ImageProcessor(IS3Client s3Client, IImageOptimizer webpImageOptimizer) : IImageProcessor
{
    public async Task<string> ProcessAsync(string name, string contentType, Stream imageStream)
    {
        var optimizedImage = await webpImageOptimizer.OptimizeAsync(imageStream);
        using var optimizeMemoryStream = new MemoryStream(optimizedImage);

        var fileRequest = new FileUploadRequest(name, optimizeMemoryStream, contentType);

        return await s3Client.UploadImage(fileRequest);
    }
}