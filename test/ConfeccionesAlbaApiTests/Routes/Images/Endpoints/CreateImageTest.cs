using System.Net;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Images.Endpoints;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;
using Moq;

namespace ConfeccionesAlbaApiTests.Routes.Images.Endpoints;

[TestFixture]
[TestOf(typeof(CreateImage))]
public class CreateImageTest
{
    private DbContextFactoryFixture _fixture;
    private ApplicationDbContext _context;
    private Mock<IImageProcessor> _imageProcessorMock;

    [SetUp]
    public void SetUp()
    {
        _fixture = new DbContextFactoryFixture();
        _context = _fixture.GetDbContext();
        _imageProcessorMock = new Mock<IImageProcessor>();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task Handle_SuccessfulImageCreation_ReturnsCreatedAtRoute()
    {
        // Arrange
        var testImageUrl = "https://example.com/images/test-image.webp";

        _imageProcessorMock
            .Setup(x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(testImageUrl);

        var formFile = new Mock<IFormFile>();
        formFile.Setup(f => f.Length).Returns(100);
        formFile.Setup(f => f.ContentType).Returns("image/webp");
        formFile.Setup(f => f.OpenReadStream()).Returns(Stream.Null);
        formFile.Setup(f => f.Name).Returns("file");

        var formCollection = new Mock<IFormCollection>();
        formCollection.Setup(f => f.Files).Returns(new FormFileCollection { formFile.Object });

        var request = new Mock<HttpRequest>();
        request.Setup(r => r.ReadFormAsync(CancellationToken.None))
            .ReturnsAsync(formCollection.Object);

        // Act
        var result = await CreateImage.Handle(
            request.Object,
            _context,
            _imageProcessorMock.Object);

        // Assert
        var createdResult = result.Result as CreatedAtRoute<ApiResponse>;
        createdResult.Should().NotBeNull();

        createdResult.Value.Should().NotBeNull();
        createdResult.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        createdResult.Value.IsSuccess.Should().BeTrue();
        createdResult.Value.Result.Should().BeOfType<Image>();

        var createdImage = createdResult.Value.Result as Image;
        createdImage.Should().NotBeNull();
        createdImage.Name.Should().NotBeEmpty();
        createdImage.Url.Should().Be(testImageUrl);

        // Verify the image was added to the database
        var images = await _context.Images.ToListAsync();
        images.Should().HaveCount(1);
        images[0].Name.Should().NotBeEmpty();
        images[0].Url.Should().Be(testImageUrl);

        // Verify image processor was called
        _imageProcessorMock.Verify(x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()), Times.Once());
    }

    [Test]
    public async Task Handle_NoImageFileProvided_ReturnsBadRequest()
    {
        // Arrange
        var formCollection = new Mock<IFormCollection>();
        formCollection.Setup(f => f.Files).Returns(new FormFileCollection());

        var request = new Mock<HttpRequest>();
        request.Setup(r => r.ReadFormAsync(CancellationToken.None))
            .ReturnsAsync(formCollection.Object);

        // Act
        var result = await CreateImage.Handle(
            request.Object,
            _context,
            _imageProcessorMock.Object);

        // Assert
        var badRequestResult = result.Result as BadRequest<ApiResponse>;
        badRequestResult.Should().NotBeNull();

        badRequestResult.Value.Should().NotBeNull();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.ErrorMessages.Should().Contain("No image file provided");

        // Verify no images were added to the database
        var images = await _context.Images.ToListAsync();
        images.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_EmptyImageFile_ReturnsBadRequest()
    {
        // Arrange
        var emptyFormFile = new Mock<IFormFile>();
        emptyFormFile.Setup(f => f.Length).Returns(0);
        emptyFormFile.Setup(f => f.ContentType).Returns("image/webp");
        emptyFormFile.Setup(f => f.OpenReadStream()).Returns(Stream.Null);

        var formCollection = new Mock<IFormCollection>();
        formCollection.Setup(f => f.Files).Returns(new FormFileCollection { emptyFormFile.Object });

        var request = new Mock<HttpRequest>();
        request.Setup(r => r.ReadFormAsync(CancellationToken.None))
            .ReturnsAsync(formCollection.Object);

        // Act
        var result = await CreateImage.Handle(
            request.Object,
            _context,
            _imageProcessorMock.Object);

        // Assert
        var badRequestResult = result.Result as BadRequest<ApiResponse>;
        badRequestResult.Should().NotBeNull();

        badRequestResult.Value.Should().NotBeNull();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.ErrorMessages.Should().Contain("No image file provided");

        // Verify no images were added to the database
        var images = await _context.Images.ToListAsync();
        images.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_ExceptionDuringProcessing_ReturnsInternalServerError()
    {
        // Arrange
        _imageProcessorMock
            .Setup(x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        var formFile = new Mock<IFormFile>();
        formFile.Setup(f => f.Length).Returns(100);
        formFile.Setup(f => f.ContentType).Returns("image/webp");
        formFile.Setup(f => f.OpenReadStream()).Returns(Stream.Null);
        formFile.Setup(f => f.Name).Returns("file");

        var formCollection = new Mock<IFormCollection>();
        formCollection.Setup(f => f.Files).Returns(new FormFileCollection { formFile.Object });

        var request = new Mock<HttpRequest>();
        request.Setup(r => r.ReadFormAsync(CancellationToken.None))
            .ReturnsAsync(formCollection.Object);

        // Act
        var result = await CreateImage.Handle(
            request.Object,
            _context,
            _imageProcessorMock.Object);

        // Assert
        var internalErrorResult = result.Result as InternalServerError<ApiResponse>;
        internalErrorResult.Should().NotBeNull();

        internalErrorResult.Value.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().Contain("Test exception");

        // Verify no images were added to the database
        var images = await _context.Images.ToListAsync();
        images.Should().BeEmpty();
    }
}