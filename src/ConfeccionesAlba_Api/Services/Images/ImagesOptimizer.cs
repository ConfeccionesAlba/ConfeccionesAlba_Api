using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ConfeccionesAlba_Api.Services.Images;

public class ImagesOptimizer
{
    public async Task<byte[]> OptimizeAsync(Stream inputStream, int maxWidth = 1280, int quality = 85)
    {
        using var image = await Image.LoadAsync(inputStream);

        // Resize while keeping aspect ratio
        if (image.Width > maxWidth)
        {
            var newHeight = (int)(image.Height * (maxWidth / (double)image.Width));
            image.Mutate(x => x.Resize(maxWidth, newHeight));
        }

        using var outputStream = new MemoryStream();
        var encoder = new WebpEncoder { Quality = quality };
        await image.SaveAsync(outputStream, encoder);
        return outputStream.ToArray();
    }
}