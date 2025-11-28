using System.Net;
using AwesomeAssertions;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Products.Endpoints;
using ConfeccionesAlba_Api.Services.Images.Interfaces;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ConfeccionesAlbaApiTests.Routes.Products.Endpoints;

[TestFixture]
[TestOf(typeof(UpdateProductById))]
public class UpdateProductByIdTest
{
    private DbContextFactoryFixture _fixture;
    private ApplicationDbContext _context;
    private Product _testItem;

    [SetUp]
    public void SetUp()
    {
        _fixture = new DbContextFactoryFixture();
        _context = _fixture.GetDbContext();
        _context.Categories.Add(new Category { Name = "category1", Description = "category1 desc" });
        _context.Categories.Add(new Category { Name = "category2", Description = "category2 desc" });
        
        // Create a test item in the database
        _testItem = new Product
        {
            Name = "Test Item",
            Description = "Original description",
            CategoryId = 1,
            PriceReference = 10.99m,
            IsVisible = true,
            Image = new Image { Name = "test name", Url = "test url" }
        };

        _context.Products.Add(_testItem);
        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _fixture.Dispose();
    }

    [Test]
    public async Task Handle_SuccessfulUpdate_ReturnsOk()
    {
        // Arrange

        // Create update DTO with updated values
        var updateDto = new ProductUpdateRequest
        {
            Id = _testItem.Id,
            Description = "Updated description",
            CategoryId = 2,
            PriceReference = 15.99m,
            IsVisible = false,
        };

        // Act
        var result = await UpdateProductById.Handle(_context, updateDto, _testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse<Product>>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        okResult.Value.IsSuccess.Should().BeTrue();

        // Verify the item was updated in the database
        var updatedItem = await _context.Products.FindAsync(_testItem.Id);
        updatedItem.Should().NotBeNull();
        updatedItem.Description.Should().Be("Updated description");
        updatedItem.CategoryId.Should().Be(2);
        updatedItem.PriceReference.Should().Be(15.99m);
        updatedItem.IsVisible.Should().BeFalse();
    }

    [Test]
    public async Task Handle_MismatchedId_ReturnsBadRequest()
    {
        // Arrange

        // Create update DTO with mismatched ID
        var updateDto = new ProductUpdateRequest { Id = _testItem.Id + 1 };

        // Act
        var result = await UpdateProductById.Handle(_context, updateDto, _testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var badRequestResult = result.Result as BadRequest<ApiResponse<Product>>;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.ErrorMessages.Should().Contain("Invalid Product id");
    }

    [Test]
    public async Task Handle_ItemNotFound_ReturnsNotFound()
    {
        // Arrange
        // Create update DTO for non-existent item
        var updateDto = new ProductUpdateRequest { Id = 999 }; // Non-existent ID

        // Act
        var result = await UpdateProductById.Handle(_context, updateDto, 999);

        // Assert
        result.Should().NotBeNull();

        var notFoundResult = result.Result as NotFound<ApiResponse<Product>>;
        notFoundResult.Should().NotBeNull();
        notFoundResult.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        notFoundResult.Value.IsSuccess.Should().BeFalse();
        notFoundResult.Value.ErrorMessages.Should().Contain("Product not found");
    }

    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        // Create update DTO
        var updateDto = new ProductUpdateRequest()
        {
            Id = _testItem.Id,
            Description = "Updated description",
            CategoryId = 2,
            PriceReference = 15.99m,
            IsVisible = false
        };

        // Simulate database error by disposing the context
        await _context.DisposeAsync();

        // Act
        var result = await UpdateProductById.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()), updateDto, _testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var internalErrorResult = result.Result as InternalServerError<ApiResponse<Product>>;
        internalErrorResult.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }

    [Test]
    public async Task Handle_PartialUpdate_ReturnsOk()
    {
        // Arrange
        // Create update DTO with only some fields updated
        var updateDto = new ProductUpdateRequest()
        {
            Id = _testItem.Id,
            Description = "Updated description", // Only updating description
            CategoryId = _testItem.CategoryId, // Keeping category same
            PriceReference = _testItem.PriceReference, // Keeping price same
            IsVisible = _testItem.IsVisible // Keeping visibility same
        };

    // Act
        var result = await UpdateProductById.Handle(_context, updateDto, _testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse<Product>>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        okResult.Value.IsSuccess.Should().BeTrue();

        // Verify only the description was updated
        var updatedItem = await _context.Products.FindAsync(_testItem.Id);
        updatedItem.Should().NotBeNull();
        updatedItem.Description.Should().Be("Updated description");
        updatedItem.CategoryId.Should().Be(1); // Should remain unchanged
        updatedItem.PriceReference.Should().Be(10.99m); // Should remain unchanged
        updatedItem.IsVisible.Should().BeTrue(); // Should remain unchanged
    }
    
    [Test]
    public async Task Handle_NoChangesDetected_ReturnsBadRequest()
    {
        // Arrange
        
        // Create update DTO with same values (no changes)
        var updateDto = new ProductUpdateRequest
        {
            Id = _testItem.Id,
            Description = _testItem.Description, // Same description
            CategoryId = _testItem.CategoryId, // Same category
            PriceReference = _testItem.PriceReference, // Same price
            IsVisible = _testItem.IsVisible // Same visibility
        };

        // Act
        var result = await UpdateProductById.Handle(_context, updateDto, _testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var badRequestResult = result.Result as BadRequest<ApiResponse<Product>>;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.ErrorMessages.Should().Contain("No changes detected in submitted data");
    }
}