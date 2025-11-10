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
[TestOf(typeof(GetItems))]
public class GetItemsTest
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
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var result = await GetItems.Handle(_context);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeOfType<List<Product>>();
        var items = okResult.Value.Result as List<Product>;
        items.Should().NotBeNull();
        items.Count.Should().Be(0);
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
                UpdatedOn = DateTime.UtcNow
            },
            new Product
            {
                Name = "Test Item 2",
                Description = "Test Description 2",
                CategoryId = 2,
                PriceReference = 20.99m,
                IsVisible = true,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            }
        };

        await _context.Products.AddRangeAsync(testItems);
        await _context.SaveChangesAsync();

        // Act
        var result = await GetItems.Handle(_context);

        // Assert
        result.Should().NotBeNull();

        var okResult = result.Result as Ok<ApiResponse>;
        okResult.Should().NotBeNull();
        okResult.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        okResult.Value.IsSuccess.Should().BeTrue();
        okResult.Value.Result.Should().BeOfType<List<Product>>();
        var items = okResult.Value.Result as List<Product>;
        items.Should().NotBeNull();
        items.Count.Should().Be(2);

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
        var result = await GetItems.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()));

        // Assert
        result.Should().NotBeNull();

        var internalErrorResult = result.Result as InternalServerError<ApiResponse>;
        internalErrorResult.Should().NotBeNull();
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }
}