using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using ConfeccionesAlba_Api.Services.S3.Exceptions;
using ConfeccionesAlba_Api.Services.S3.Interfaces;
using ConfeccionesAlba_Api.Services.S3.Models;
using Microsoft.Extensions.Options;

namespace ConfeccionesAlba_Api.Services.S3;

public class S3CloudflareClient : IS3Client
{
    private readonly R2Options _options;
    private readonly ILogger<S3CloudflareClient> _logger;
    private readonly AmazonS3Client _s3Client;

    public S3CloudflareClient(IOptions<R2Options> options, ILogger<S3CloudflareClient> logger)
    {
        _logger = logger;
        _options = options.Value;

        var amazonS3Config = new AmazonS3Config { ServiceURL = _options.EndpointUrl, ForcePathStyle = true, };
        _s3Client = new AmazonS3Client(_options.AccessKey, _options.SecretKey, amazonS3Config);
    }
    
    public async Task<string> UploadImage(FileUploadRequest file)
    {
        _logger.LogInformation("New file {FileName} to upload to S3 bucket {BucketName}", file.FileName, _options.BucketName);
        
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = file.FileName,
            InputStream = file.FileStream,
            ContentType = file.ContentType,
            DisablePayloadSigning = true
        };

        var response = await _s3Client.PutObjectAsync(request);

        if(response.HttpStatusCode != HttpStatusCode.OK)
        {
            _logger.LogError("Error uploading image: {HttpStatusCode}", response.HttpStatusCode);
            throw new S3Exception("Upload to Cloudflare R2 failed");
        }
        
        if (Uri.TryCreate(_options.PublicUrl, UriKind.Absolute, out var baseUri) &&
            Uri.TryCreate(baseUri, file.FileName, out var finalUri))
        {
            return finalUri.ToString();
        }
        
        _logger.LogError("Error formating public url: {PublicUri}, {ImageFile}", _options.PublicUrl, file.FileName);
        throw new S3Exception("Failed to format public url");
    }

    public async Task RemoveImage(string imageName)
    {
            _logger.LogInformation("Deleting file {Key} from R2 bucket {BucketName}", imageName, _options.BucketName);
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = imageName,
            };
            
            var response = await _s3Client.DeleteObjectAsync(deleteRequest);
            if (response.HttpStatusCode != HttpStatusCode.NoContent)
            {
                _logger.LogError("Failed to delete file {Key} from R2 bucket {BucketName}. Status code: {StatusCode}",
                    imageName, _options.BucketName, response.HttpStatusCode);

                throw new S3Exception("Failed to delete file");
            }

            _logger.LogInformation("File {Key} deleted successfully from R2 bucket {BucketName}", imageName, _options.BucketName);
    }
}