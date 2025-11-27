using System.Net;
using AwesomeAssertions;
using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Routes.Products.Endpoints;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlbaApiTests.Routes.Products.Endpoints;

[TestFixture]
[TestOf(typeof(GetProductById))]
public class GetProductByIdTest
{
    private DbContextFactoryFixture _fixture;
    private ApplicationDbContext _context;

    [SetUp]
    public void SetUp()
    {
        _fixture = new DbContextFactoryFixture();
        _context = _fixture.GetDbContext();
        _context.Categories.Add(new Category { Name = "category1", Description = "category1 desc" });
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
        _fixture.Dispose();
    }

    [Test]
    public async Task Handle_InvalidItemId_ReturnsBadRequest()
    {
        // Arrange
        int invalidId = 0; // Invalid ID (less than 1)

        // Act
        var result = await GetProductById.Handle(_context, invalidId);

        // Assert
        result.Should().NotBeNull();

        var badRequestResult = result.Result as BadRequest<ApiResponse<Product>>;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        badRequestResult.Value.IsSuccess.Should().BeFalse();
        badRequestResult.Value.ErrorMessages.Should().Contain("Invalid Product Id");
    }

    [Test]
    public async Task Handle_NonExistentItemId_ReturnsNotFound()
    {
        // Arrange
        int nonExistentId = 999; // ID that doesn't exist in the database

        // Act
        var result = await GetProductById.Handle(_context, nonExistentId);

        // Assert
        result.Should().NotBeNull();

        var notFoundResult = result.Result as NotFound<ApiResponse<Product>>;
        notFoundResult.Should().NotBeNull();
        notFoundResult.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        notFoundResult.Value.IsSuccess.Should().BeFalse();
        notFoundResult.Value.ErrorMessages.Should().Contain("Product not found");
    }

    [Test]
    public async Task Handle_ValidItemId_ReturnsOkWithItem()
    {
        // Arrange
        // Add a test item to the database
        var testItem = new Product
        {
            Name = "Test Item",
            Description = "Test Description",
            CategoryId = 1,
            PriceReference = 10.99m,
            IsVisible = true,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            Image = new Image { Name = "test name", Url = "test url" }
        };

        await _context.Products.AddAsync(testItem);
        await _context.SaveChangesAsync();

        // Act
        var result = await GetProductById.Handle(_context, testItem.Id);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse<Product>>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeOfType<Product>();

        var returnedItem = okResult.Value.Result as Product;
        returnedItem.Should().NotBeNull();
        returnedItem.Id.Should().Be(testItem.Id);
        returnedItem.Name.Should().Be(testItem.Name);
        returnedItem.Description.Should().Be(testItem.Description);
        returnedItem.PriceReference.Should().Be(testItem.PriceReference);
        returnedItem.IsVisible.Should().Be(testItem.IsVisible);
    }

    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        // Simulate a database error by disposing the context
        await _context.DisposeAsync();

        // Act
        var result = await GetProductById.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()), 1);

        // Assert
        result.Should().NotBeNull();

        var internalErrorResult = result.Result as InternalServerError<ApiResponse<Product>>;
        internalErrorResult.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }
}