using ConfeccionesAlba_Api.Services.Images.Interfaces;
using ConfeccionesAlba_Api.Services.S3.Interfaces;
using ConfeccionesAlba_Api.Services.S3.Models;

namespace ConfeccionesAlba_Api.Services.Images;

public class ImageProcessor(IS3Client s3Client, IImageOptimizer webpImageOptimizer) : IImageProcessor
{
    public async Task<string> ProcessAsync(string name, string contentType, Stream imageStream)
    {
        await using var optimizedImageStream = await webpImageOptimizer.OptimizeAsync(imageStream);

        var fileRequest = new FileUploadRequest(name, optimizedImageStream, contentType);

        return await s3Client.UploadImage(fileRequest);
    }

    public Task RemoveAsync(string name)
    {
        return s3Client.RemoveImage(name);
    }
}