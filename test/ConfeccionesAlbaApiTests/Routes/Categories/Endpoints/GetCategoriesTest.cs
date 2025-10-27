using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Categories.Endpoints;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;
using ConfeccionesAlba_Api.Data;

namespace ConfeccionesAlbaApiTests.Routes.Categories.Endpoints;

[TestFixture]
[TestOf(typeof(GetCategories))]
public class GetCategoriesTest
{
    private DbContextFactoryFixture _fixture;
    private ApplicationDbContext _context;

    [SetUp]
    public void SetUp()
    {
        _fixture = new DbContextFactoryFixture();
        _context = _fixture.GetDbContext();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task Handle_SuccessfulRetrievalOfCategories_ReturnsOk()
    {
        // Arrange
        // Add test categories to the database
        var testCategories = new List<Category>
        {
            new Category
            {
                Name = "Test Category 1",
                Description = "Test Description 1",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            },
            new Category
            {
                Name = "Test Category 2",
                Description = "Test Description 2",
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            }
        };

        await _context.Categories.AddRangeAsync(testCategories);
        await _context.SaveChangesAsync();

        // Act
        var result = await GetCategories.Handle(_context);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeOfType<List<Category>>();

        var returnedCategories = okResult.Value.Result as List<Category>;
        returnedCategories.Should().HaveCount(2);
        returnedCategories.Should().Contain(c => c.Name == "Test Category 1");
        returnedCategories.Should().Contain(c => c.Name == "Test Category 2");
    }

    [Test]
    public async Task Handle_EmptyCategoryList_ReturnsOkWithEmptyList()
    {
        // Arrange
        // Database is empty by default

        // Act
        var result = await GetCategories.Handle(_context);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeOfType<List<Category>>();

        var returnedCategories = okResult.Value.Result as List<Category>;
        returnedCategories.Should().HaveCount(0);
    }

    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        // Simulate a database error by disposing the context
        await _context.DisposeAsync();

        // Act
        var result = await GetCategories.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()));

        // Assert
        result.Should().NotBeNull();

        var internalErrorResult = result.Result as InternalServerError<ApiResponse>;
        internalErrorResult.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }
}