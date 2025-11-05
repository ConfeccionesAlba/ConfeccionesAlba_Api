namespace ConfeccionesAlba_Api.Services.Images.Interfaces;

public interface IImageProcessor
{
    Task<string> ProcessAsync(string name, string contentType, Stream imageStream);
}