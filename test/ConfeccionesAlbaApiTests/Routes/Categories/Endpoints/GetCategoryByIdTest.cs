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
[TestOf(typeof(GetCategoryById))]
public class GetCategoryByIdTest
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
        _fixture.Dispose();
    }

    [Test]
    public async Task Handle_ValidCategoryId_ReturnsOkWithCategory()
    {
        // Arrange
        // Add a test category to the database
        var testCategory = new Category
        {
            Name = "Test Category",
            Description = "Test Description",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await _context.Categories.AddAsync(testCategory);
        await _context.SaveChangesAsync();

        // Act
        var result = await GetCategoryById.Handle(_context, testCategory.Id);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeOfType<Category>();

        var returnedCategory = okResult.Value.Result as Category;
        returnedCategory.Should().NotBeNull();
        returnedCategory.Id.Should().Be(testCategory.Id);
        returnedCategory.Name.Should().Be(testCategory.Name);
        returnedCategory.Description.Should().Be(testCategory.Description);
    }

    [Test]
    public async Task Handle_InvalidCategoryId_ReturnsBadRequest()
    {
        // Arrange
        int invalidId = 0; // Invalid ID (less than 1)

        // Act
        var result = await GetCategoryById.Handle(_context, invalidId);

        // Assert
        result.Should().NotBeNull();

        var badRequestResult = result.Result as BadRequest<ApiResponse>;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.ErrorMessages.Should().Contain("Invalid category Id");
    }

    [Test]
    public async Task Handle_NonExistentCategoryId_ReturnsNotFound()
    {
        // Arrange
        int nonExistentId = 999; // ID that doesn't exist in the database

        // Act
        var result = await GetCategoryById.Handle(_context, nonExistentId);

        // Assert
        result.Should().NotBeNull();

        var notFoundResult = result.Result as NotFound<ApiResponse>;
        notFoundResult.Should().NotBeNull();
        notFoundResult.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        notFoundResult.Value.IsSuccess.Should().BeFalse();
        notFoundResult.Value.ErrorMessages.Should().Contain("Category not found");
    }

    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        // Simulate a database error by disposing the context
        await _context.DisposeAsync();

        // Act
        var result = await GetCategoryById.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()), 1);

        // Assert
        result.Should().NotBeNull();

        var internalErrorResult = result.Result as InternalServerError<ApiResponse>;
        internalErrorResult.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }
}