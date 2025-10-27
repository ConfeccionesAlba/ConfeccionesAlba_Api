using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Categories;
using ConfeccionesAlba_Api.Routes.Categories.Endpoints;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;
using ConfeccionesAlba_Api.Data;

namespace ConfeccionesAlbaApiTests.Routes.Categories.Endpoints;

[TestFixture]
[TestOf(typeof(UpdateCategoryById))]
public class UpdateCategoryByIdTest
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
    public async Task Handle_ValidCategoryIdAndDto_ReturnsOk()
    {
        // Arrange
        // Add a test category to the database
        var testCategory = new Category
        {
            Name = "Test Category",
            Description = "Original Description",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await _context.Categories.AddAsync(testCategory);
        await _context.SaveChangesAsync();

        var updateDto = new CategoryUpdateDto
        {
            Id = testCategory.Id,
            Description = "Updated Description"
        };

        // Act
        var result = await UpdateCategoryById.Handle(_context, updateDto, testCategory.Id);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.ErrorMessages.Should().BeEmpty();

        // Verify the category was updated in the database
        var updatedCategory = await _context.Categories.FindAsync(testCategory.Id);
        updatedCategory.Should().NotBeNull();
        updatedCategory.Description.Should().Be("Updated Description");
    }

    [Test]
    public async Task Handle_InvalidCategoryIdMismatch_ReturnsBadRequest()
    {
        // Arrange
        // Add a test category to the database
        var testCategory = new Category
        {
            Name = "Test Category",
            Description = "Original Description",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await _context.Categories.AddAsync(testCategory);
        await _context.SaveChangesAsync();

        var updateDto = new CategoryUpdateDto
        {
            Id = testCategory.Id + 1, // Mismatched ID
            Description = "Updated Description"
        };

        // Act
        var result = await UpdateCategoryById.Handle(_context, updateDto, testCategory.Id);

        // Assert
        result.Should().NotBeNull();

        var badRequestResult = result.Result as BadRequest<ApiResponse>;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.ErrorMessages.Should().Contain("Invalid category id");
    }

    [Test]
    public async Task Handle_NonExistentCategoryId_ReturnsNotFound()
    {
        // Arrange
        int nonExistentId = 999; // ID that doesn't exist in the database
        var updateDto = new CategoryUpdateDto
        {
            Id = nonExistentId,
            Description = "Updated Description"
        };

        // Act
        var result = await UpdateCategoryById.Handle(_context, updateDto, nonExistentId);

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

        var updateDto = new CategoryUpdateDto
        {
            Id = 1,
            Description = "Updated Description"
        };

        // Act
        var result = await UpdateCategoryById.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()), updateDto, 1);

        // Assert
        result.Should().NotBeNull();

        var internalErrorResult = result.Result as InternalServerError<ApiResponse>;
        internalErrorResult.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }

    [Test]
    public async Task Handle_EmptyDescription_ReturnsOkWithoutUpdatingDescription()
    {
        // Arrange
        // Add a test category to the database
        var testCategory = new Category
        {
            Name = "Test Category",
            Description = "Original Description",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await _context.Categories.AddAsync(testCategory);
        await _context.SaveChangesAsync();

        var updateDto = new CategoryUpdateDto
        {
            Id = testCategory.Id,
            Description = "" // Empty description
        };

        // Act
        var result = await UpdateCategoryById.Handle(_context, updateDto, testCategory.Id);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.ErrorMessages.Should().BeEmpty();

        // Verify the category description was not updated
        var updatedCategory = await _context.Categories.FindAsync(testCategory.Id);
        updatedCategory.Should().NotBeNull();
        updatedCategory.Description.Should().Be("Original Description");
    }
}