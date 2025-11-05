using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Images.Endpoints;
using ConfeccionesAlba_Api.Services.S3.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using AwesomeAssertions;
using ConfeccionesAlbaApiTests.Common;

namespace ConfeccionesAlbaApiTests.Routes.Images.Endpoints;

[TestFixture]
public class DeleteImageByIdTest
{
    private DbContextFactoryFixture _dbContextFactory;
    private Mock<IS3Client> _mockS3Client;

    [SetUp]
    public void Setup()
    {
        _dbContextFactory = new DbContextFactoryFixture();
        _mockS3Client = new Mock<IS3Client>();
    }

    [Test]
    public async Task Handle_ShouldReturnOk_WhenImageIsDeletedSuccessfully()
    {
        // Arrange
        var dbContext = _dbContextFactory.GetDbContext();
        var image = new Image { Id = 1, Name = "test.jpg", Url = "http://test.com/test.jpg" };
        dbContext.Images.Add(image);
        await dbContext.SaveChangesAsync();

        _mockS3Client.Setup(s => s.RemoveImage(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await DeleteImageById.Handle(dbContext, _mockS3Client.Object, image.Id);

        // Assert
        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();

        var deletedImage = await dbContext.Images.FindAsync(image.Id);
        deletedImage.Should().BeNull();
    }

    [Test]
    public async Task Handle_ShouldReturnNotFound_WhenImageDoesNotExist()
    {
        // Arrange
        var dbContext = _dbContextFactory.GetDbContext();
        var nonExistentId = 999;

        // Act
        var result = await DeleteImageById.Handle(dbContext, _mockS3Client.Object, nonExistentId);

        // Assert
        var notFoundResult = result.Result as NotFound<ApiResponse>;
        notFoundResult.Should().NotBeNull();
        notFoundResult.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        notFoundResult.Value.IsSuccess.Should().BeFalse();
        notFoundResult.Value.ErrorMessages.Should().ContainSingle().Which.Should().Be("Image not found");
    }

    [Test]
    public async Task Handle_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var dbContext = _dbContextFactory.GetDbContext();
        var image = new Image { Id = 1, Name = "test.jpg", Url = "http://test.com/test.jpg" };
        dbContext.Images.Add(image);
        await dbContext.SaveChangesAsync();

        _mockS3Client.Setup(s => s.RemoveImage(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await DeleteImageById.Handle(dbContext, _mockS3Client.Object, image.Id);

        // Assert
        var errorResult = result.Result as InternalServerError<ApiResponse>;
        errorResult.Should().NotBeNull();
        errorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        errorResult.Value.IsSuccess.Should().BeFalse();
        errorResult.Value.ErrorMessages.Should().ContainSingle().Which.Should().Be("Test exception");
    }
}