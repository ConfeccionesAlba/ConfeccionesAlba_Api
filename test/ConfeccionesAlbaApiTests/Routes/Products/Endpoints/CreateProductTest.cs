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
[TestOf(typeof(CreateProduct))]
public class CreateProductTest
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
    public async Task Handle_SuccessfulItemCreation_ReturnsCreatedAtRoute()
    {
        // Arrange
        var itemDto = new ProductCreateRequest
        {
            Name = "Test Item",
            Description = "Test Description",
            CategoryId = 1,
            PriceReference = 19.99m,
            IsVisible = true
        };

        // Act
        var result = await CreateProduct.Handle(_context, itemDto);

        // Assert
        result.Should().NotBeNull();

        var createdResult = result.Result as CreatedAtRoute<ApiResponse<Product>>;
        createdResult.Value.Result.Should().BeOfType<Product>();
        createdResult.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        createdResult.Value.IsSuccess.Should().BeTrue();

        // Verify the item was added to the database
        var items = await _context.Products.ToListAsync();
        items.Should().HaveCount(1);
        items[0].Name.Should().Be(itemDto.Name);
        items[0].Description.Should().Be(itemDto.Description);
        items[0].CategoryId.Should().Be(itemDto.CategoryId);
        items[0].PriceReference.Should().Be(itemDto.PriceReference);
        items[0].IsVisible.Should().Be(itemDto.IsVisible);
    }

    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        var itemDto = new ProductCreateRequest
        {
            Name = "Test Item",
            Description = "Test Description",
            CategoryId = 1,
            PriceReference = 19.99m,
            IsVisible = true
        };

        // Act - Simulate a database error by disposing the context
        await _context.DisposeAsync();
        var result = (await CreateProduct.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()), itemDto)).Result;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<InternalServerError<ApiResponse<Product>>>();

        var internalErrorResult = (InternalServerError<ApiResponse<Product>>)result;
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }

    [Test]
    public async Task Handle_ValidItemWithMinimalData_SuccessfullyCreatesItem()
    {
        // Arrange
        var itemDto = new ProductCreateRequest
        {
            Name = "Test Item",
            Description = "",
            CategoryId = 1,
            PriceReference = 0m,
            IsVisible = false
        };
        
        // Act
        var result = (await CreateProduct.Handle(_context, itemDto)).Result;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtRoute<ApiResponse<Product>>>();

        var createdResult = (CreatedAtRoute<ApiResponse<Product>>)result;
        createdResult.Value.Result.Should().BeOfType<Product>();
        createdResult.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        createdResult.Value.IsSuccess.Should().BeTrue();

        // Verify the item was added to the database
        var items = await _context.Products.ToListAsync();
        items.Should().HaveCount(1);
        items[0].Name.Should().Be(itemDto.Name);
        items[0].Description.Should().BeEmpty();
        items[0].CategoryId.Should().Be(itemDto.CategoryId);
        items[0].PriceReference.Should().Be(itemDto.PriceReference);
        items[0].IsVisible.Should().Be(itemDto.IsVisible);
    }

    [Test]
    public async Task Handle_MultipleValidItems_CreatesAllSuccessfully()
    {
        // Arrange
        
        var itemDto1 = new ProductCreateRequest
        {
            Name = "Test Item 1",
            Description = "Test Description 1",
            CategoryId = 1,
            PriceReference = 19.99m,
            IsVisible = true
        };
        
        var itemDto2 = new ProductCreateRequest
        {
            Name = "Test Item 2",
            Description = "Test Description 2",
            CategoryId = 1,
            PriceReference = 29.99m,
            IsVisible = false
        };

        // Act
        var result1 = (await CreateProduct.Handle(_context, itemDto1)).Result;
        var result2 = (await CreateProduct.Handle(_context, itemDto2)).Result;

        // Assert
        result1.Should().NotBeNull();
        result1.Should().BeOfType<CreatedAtRoute<ApiResponse<Product>>>();

        result2.Should().NotBeNull();
        result2.Should().BeOfType<CreatedAtRoute<ApiResponse<Product>>>();

        // Verify both items were added to the database
        var items = await _context.Products.ToListAsync();
        items.Should().HaveCount(2);

        items.Should().Contain(i => i.Name == itemDto1.Name &&
                                   i.Description == itemDto1.Description &&
                                   i.CategoryId == itemDto1.CategoryId &&
                                   i.PriceReference == itemDto1.PriceReference &&
                                   i.IsVisible == itemDto1.IsVisible);

        items.Should().Contain(i => i.Name == itemDto2.Name &&
                                   i.Description == itemDto2.Description &&
                                   i.CategoryId == itemDto2.CategoryId &&
                                   i.PriceReference == itemDto2.PriceReference &&
                                   i.IsVisible == itemDto2.IsVisible);
    }

    [Test]
    public async Task Handle_ItemWithLongDescription_SuccessfullyCreatesItem()
    {
        // Arrange
        var longDescription = new string('a', 5000); // Max length
        var itemDto = new ProductCreateRequest
        {
            Name = "Test Item",
            Description = longDescription,
            CategoryId = 1,
            PriceReference = 19.99m,
            IsVisible = true
        };
        
        // Act
        var result = (await CreateProduct.Handle(_context, itemDto)).Result;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtRoute<ApiResponse<Product>>>();

        var createdResult = (CreatedAtRoute<ApiResponse<Product>>)result;
        createdResult.Value.Result.Should().BeOfType<Product>();
        createdResult.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        createdResult.Value.IsSuccess.Should().BeTrue();

        // Verify the item was added to the database
        var items = await _context.Products.ToListAsync();
        items.Should().HaveCount(1);
        items[0].Name.Should().Be(itemDto.Name);
        items[0].Description.Should().Be(longDescription);
        items[0].Description.Length.Should().Be(5000); // Verify max length
    }
}