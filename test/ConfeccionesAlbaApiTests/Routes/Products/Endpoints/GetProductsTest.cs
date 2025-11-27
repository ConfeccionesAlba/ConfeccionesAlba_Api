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
[TestOf(typeof(GetProducts))]
public class GetProductsTest
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
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var result = await GetProducts.Handle(_context);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse<Product[]>>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeOfType<Product[]>();
        var items = okResult.Value.Result;
        items.Should().NotBeNull();
        items.Length.Should().Be(0);
    }

    [Test]
    public async Task Handle_WithItems_ReturnsAllItems()
    {
        // Arrange
        // Add test items to the database
        var testItems = new List<Product>
        {
            new Product
            {
                Name = "Test Item 1",
                Description = "Test Description 1",
                CategoryId = 1,
                PriceReference = 10.99m,
                IsVisible = true,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Image = new Image {Name="test name", Url = "test url"}
            },
            new Product
            {
                Name = "Test Item 2",
                Description = "Test Description 2",
                CategoryId = 1,
                PriceReference = 20.99m,
                IsVisible = true,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Image = new Image {Name="test name", Url = "test url"}
            }
        };

        await _context.Products.AddRangeAsync(testItems);
        await _context.SaveChangesAsync();

        // Act
        var result = await GetProducts.Handle(_context);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse<Product[]>>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeOfType<Product[]>();
        var items = okResult.Value.Result;
        items.Should().NotBeNull();
        items.Length.Should().Be(2);

        // Verify the first item
        var firstItem = items[0];
        firstItem.Name.Should().Be("Test Item 1");
        firstItem.Description.Should().Be("Test Description 1");
        firstItem.PriceReference.Should().Be(10.99m);

        // Verify the second item
        var secondItem = items[1];
        secondItem.Name.Should().Be("Test Item 2");
        secondItem.Description.Should().Be("Test Description 2");
        secondItem.PriceReference.Should().Be(20.99m);
    }

    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        // Simulate a database error by disposing the context
        await _context.DisposeAsync();

        // Act
        var result = await GetProducts.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()));

        // Assert
        result.Should().NotBeNull();

        var internalErrorResult = result.Result as InternalServerError<ApiResponse<Product[]>>;
        internalErrorResult.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }
}