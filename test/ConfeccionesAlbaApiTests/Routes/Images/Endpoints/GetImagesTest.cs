using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Images.Endpoints;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using AwesomeAssertions;

namespace ConfeccionesAlbaApiTests.Routes.Images.Endpoints;

[TestFixture]
[TestOf(typeof(GetImages))]
public class GetImagesTest
{
    private DbContextFactoryFixture _dbContextFactoryFixture;

    [SetUp]
    public void Setup()
    {
        _dbContextFactoryFixture = new DbContextFactoryFixture();
    }

    [Test]
    public async Task GetImages_ReturnsOk_WhenImagesExist()
    {
        // Arrange
        var dbContext = _dbContextFactoryFixture.GetDbContext();
        var testImages = new List<Image>
        {
            new() { Id = 1, Name = "Test1", Url = "http://example.com/test1.jpg" },
            new() { Id = 2, Name = "Test2", Url = "http://example.com/test2.jpg" }
        };

        await dbContext.Images.AddRangeAsync(testImages);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await GetImages.Handle(dbContext);

        // Assert
        result.Result.Should().BeOfType<Ok<ApiResponse>>();
        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value!.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeEquivalentTo(testImages);
    }

    [Test]
    public async Task GetImages_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var dbContext = _dbContextFactoryFixture.GetDbContext();

        // Simulate a database error by disposing the context
        await dbContext.DisposeAsync();

        // Act
        var result = await GetImages.Handle(dbContext);

        // Assert
        result.Result.Should().BeOfType<InternalServerError<ApiResponse>>();
        var errorResult = result.Result as InternalServerError<ApiResponse>;
        errorResult.Should().NotBeNull();
        errorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        errorResult.Value.IsSuccess.Should().BeFalse();
        errorResult.Value.ErrorMessages.Should().NotBeEmpty();
    }
}