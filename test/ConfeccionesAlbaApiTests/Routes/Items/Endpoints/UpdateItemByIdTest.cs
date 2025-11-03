using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Items.Endpoints;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;
using ConfeccionesAlba_Api.Data;

namespace ConfeccionesAlbaApiTests.Routes.Items.Endpoints;

[TestFixture]
[TestOf(typeof(UpdateItemById))]
public class UpdateItemByIdTest
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
    public async Task TearDown()
    {
        await _context.DisposeAsync();
    }

    [Test]
    public async Task Handle_SuccessfulUpdate_ReturnsOk()
    {
        // Arrange
        // Create a test item in the database
        var testItem = new Item
        {
            Name = "Test Item",
            Description = "Original description",
            CategoryId = 1,
            PriceReference = 10.99m,
            ImageUrl = "test.jpg",
            IsVisible = true,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await _context.Items.AddAsync(testItem);
        await _context.SaveChangesAsync();

        // Create update DTO with updated values
        var updateDto = new ItemUpdateRequest(testItem.Id, "Updated description", 2, 15.99m, false);

        // Act
        var result = await UpdateItemById.Handle(_context, updateDto, testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        okResult.Value.IsSuccess.Should().BeTrue();

        // Verify the item was updated in the database
        var updatedItem = await _context.Items.FindAsync(testItem.Id);
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
        // Create a test item in the database
        var testItem = new Item
        {
            Name = "Test Item",
            Description = "Original description",
            CategoryId = 1,
            PriceReference = 10.99m,
            ImageUrl = "test.jpg",
            IsVisible = true,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await _context.Items.AddAsync(testItem);
        await _context.SaveChangesAsync();

        // Create update DTO with mismatched ID
        var updateDto = new ItemUpdateRequest(testItem.Id + 1, // Mismatched ID
            "Updated description", 
            2, 
            15.99m,
            false);

        // Act
        var result = await UpdateItemById.Handle(_context, updateDto, testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var badRequestResult = result.Result as BadRequest<ApiResponse>;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.ErrorMessages.Should().Contain("Invalid item id");
    }

    [Test]
    public async Task Handle_ItemNotFound_ReturnsNotFound()
    {
        // Arrange
        // Create update DTO for non-existent item
        var updateDto = new ItemUpdateRequest(999, // Non-existent ID
            "Updated description", 
            2, 
            15.99m, 
            false);

        // Act
        var result = await UpdateItemById.Handle(_context, updateDto, 999);

        // Assert
        result.Should().NotBeNull();

        var notFoundResult = result.Result as NotFound<ApiResponse>;
        notFoundResult.Should().NotBeNull();
        notFoundResult.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        notFoundResult.Value.IsSuccess.Should().BeFalse();
        notFoundResult.Value.ErrorMessages.Should().Contain("Item not found");
    }

    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        // Create a test item in the database
        var testItem = new Item
        {
            Name = "Test Item",
            Description = "Original description",
            CategoryId = 1,
            PriceReference = 10.99m,
            ImageUrl = "test.jpg",
            IsVisible = true,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await _context.Items.AddAsync(testItem);
        await _context.SaveChangesAsync();

        // Create update DTO
        var updateDto = new ItemUpdateRequest(testItem.Id, "Updated description", 2, 15.99m, false);

        // Simulate database error by disposing the context
        await _context.DisposeAsync();

        // Act
        var result = await UpdateItemById.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()), updateDto, testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var internalErrorResult = result.Result as InternalServerError<ApiResponse>;
        internalErrorResult.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }

    [Test]
    public async Task Handle_PartialUpdate_ReturnsOk()
    {
        // Arrange
        // Create a test item in the database
        var testItem = new Item
        {
            Name = "Test Item",
            Description = "Original description",
            CategoryId = 1,
            PriceReference = 10.99m,
            ImageUrl = "test.jpg",
            IsVisible = true,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await _context.Items.AddAsync(testItem);
        await _context.SaveChangesAsync();

        // Create update DTO with only some fields updated
        var updateDto = new ItemUpdateRequest(testItem.Id,
            "Updated description", // Only updating description
            testItem.CategoryId, // Keeping category same
            testItem.PriceReference, // Keeping price same
            testItem.IsVisible // Keeping visibility same
        );

        // Act
        var result = await UpdateItemById.Handle(_context, updateDto, testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        okResult.Value.IsSuccess.Should().BeTrue();

        // Verify only the description was updated
        var updatedItem = await _context.Items.FindAsync(testItem.Id);
        updatedItem.Should().NotBeNull();
        updatedItem.Description.Should().Be("Updated description");
        updatedItem.CategoryId.Should().Be(1); // Should remain unchanged
        updatedItem.PriceReference.Should().Be(10.99m); // Should remain unchanged
        updatedItem.IsVisible.Should().BeTrue(); // Should remain unchanged
    }
}