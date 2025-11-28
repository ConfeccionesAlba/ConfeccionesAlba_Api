using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Products.Endpoints;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using AwesomeAssertions;

namespace ConfeccionesAlbaApiTests.Routes.Products.Endpoints;

[TestFixture]
[TestOf(typeof(UploadProductImage))]
public class UploadProductImageTest
{
    private DbContextFactoryFixture _dbContextFactoryFixture;
    private ApplicationDbContext _dbContext;
    private IImageProcessor _imageProcessor;
    private Product _testItem;

    [SetUp]
    public void SetUp()
    {
        _dbContextFactoryFixture = new DbContextFactoryFixture();
        _dbContext = _dbContextFactoryFixture.GetDbContext();
        _imageProcessor = Substitute.For<IImageProcessor>();
        
        // Create a test item in the database
        
        _dbContext.Categories.Add(new Category { Name = "category1", Description = "category1 desc" });
        _testItem = new Product
        {
            Name = "Test Item",
            Description = "Original description",
            CategoryId = 1,
            PriceReference = 10.99m,
            IsVisible = true,
            Image = new Image { Name = "test name", Url = "test url" }
        };

        _dbContext.Products.Add(_testItem);
        _dbContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
        _dbContextFactoryFixture.Dispose();
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenImageIsNull()
    {
        // Arrange
        const int id = 1;
        IFormFile image = null;
    
        // Act
        var result = await UploadProductImage.Handle(_dbContext, _imageProcessor, id, image);
    
        // Assert
        result.Result.Should().BeOfType<BadRequest<ApiResponse<UploadImageResponse>>>();
        var badRequestResult = result.Result.As<BadRequest<ApiResponse<UploadImageResponse>>>();
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.ErrorMessages.Should().Contain("Invalid image received");
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenProductDoesNotExist()
    {
        // Arrange
        const int id = 999;
        var image = Substitute.For<IFormFile>();
        image.Length.Returns(100);

        // Act
        var result = await UploadProductImage.Handle(_dbContext, _imageProcessor, id, image);

        // Assert
        result.Result.Should().BeOfType<BadRequest<ApiResponse<UploadImageResponse>>>();
        var badRequest = result.Result.As<BadRequest<ApiResponse<UploadImageResponse>>>();
        badRequest.Value.IsSuccess.Should().BeFalse();
        badRequest.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequest.Value.ErrorMessages.Should().Contain("Product Id not found");
    }

    [Test]
    public async Task Handle_ShouldReturnOk_WhenImageIsUploadedSuccessfully()
    {
        // Arrange
        const int id = 1;
        var image = Substitute.For<IFormFile>();
        image.Length.Returns(100);
        image.ContentType.Returns("image/webp");
        image.OpenReadStream().Returns(new MemoryStream());

        _imageProcessor.ProcessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>())
            .Returns(Task.FromResult("http://example.com/new-image.webp"));

        // Act
        var result = await UploadProductImage.Handle(_dbContext, _imageProcessor, id, image);

        // Assert
        result.Result.Should().BeOfType<Ok<ApiResponse<UploadImageResponse>>>();
        var okResult = result.Result.As<Ok<ApiResponse<UploadImageResponse>>>();
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Url.Should().Be("http://example.com/new-image.webp");
    }

    [Test]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        const int id = 1;
        var image = Substitute.For<IFormFile>();
        image.Length.Returns(100);
        image.ContentType.Returns("image/webp");
        image.OpenReadStream().Returns(new MemoryStream());

        await _dbContext.DisposeAsync(); // Dispose dbContext to simulate an internal error

        // Act
        var result = await UploadProductImage.Handle(_dbContext, _imageProcessor, id, image);

        // Assert
        result.Result.Should().BeOfType<InternalServerError<ApiResponse<UploadImageResponse>>>();
        var internalServerErrorResult = result.Result.As<InternalServerError<ApiResponse<UploadImageResponse>>>();
        internalServerErrorResult.Value.IsSuccess.Should().BeFalse();
        internalServerErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}