using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Images.Endpoints;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using AwesomeAssertions;

namespace ConfeccionesAlbaApiTests.Routes.Images.Endpoints;

[TestFixture]
[TestOf(typeof(GetImageById))]
public class GetImageByIdTest
{
    private DbContextFactoryFixture _dbContextFactoryFixture;

    [SetUp]
    public void SetUp()
    {
        _dbContextFactoryFixture = new DbContextFactoryFixture();
    }

    [Test]
    public async Task Handle_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var dbContext = _dbContextFactoryFixture.GetDbContext();
        var invalidId = 0;

        // Act
        var result = await GetImageById.Handle(dbContext, invalidId);

        // Assert
        result.Result.Should().BeOfType<BadRequest<ApiResponse>>();
        var badRequestResult = result.Result.As<BadRequest<ApiResponse>>();
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.ErrorMessages.Should().Contain("Invalid image Id");
    }

    [Test]
    public async Task Handle_NonExistentImage_ReturnsNotFound()
    {
        // Arrange
        var dbContext = _dbContextFactoryFixture.GetDbContext();
        var nonExistentId = 999;

        // Act
        var result = await GetImageById.Handle(dbContext, nonExistentId);

        // Assert
        result.Result.Should().BeOfType<NotFound<ApiResponse>>();
        var notFoundResult = result.Result.As<NotFound<ApiResponse>>();
        notFoundResult.Value.IsSuccess.Should().BeFalse();
        notFoundResult.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        notFoundResult.Value.ErrorMessages.Should().Contain("image not found");
    }

    [Test]
    public async Task Handle_ValidId_ReturnsOkWithImage()
    {
        // Arrange
        var dbContext = _dbContextFactoryFixture.GetDbContext();
        var image = new Image { Id = 1, Name = "Test Image", Url = "http://test.com/image.jpg" };
        dbContext.Images.Add(image);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await GetImageById.Handle(dbContext, image.Id);

        // Assert
        result.Result.Should().BeOfType<Ok<ApiResponse>>();
        var okResult = result.Result.As<Ok<ApiResponse>>();
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.Result.Should().BeEquivalentTo(image);
    }

    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        var dbContext = _dbContextFactoryFixture.GetDbContext();
        var validId = 1;

        // Simulate a database error by disposing the context
        await dbContext.DisposeAsync();

        // Act
        var result = await GetImageById.Handle(dbContext, validId);

        // Assert
        result.Result.Should().BeOfType<InternalServerError<ApiResponse>>();
        var internalServerErrorResult = result.Result.As<InternalServerError<ApiResponse>>();
        internalServerErrorResult.Value.IsSuccess.Should().BeFalse();
        internalServerErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalServerErrorResult.Value.ErrorMessages.Should().NotBeEmpty();
    }
}