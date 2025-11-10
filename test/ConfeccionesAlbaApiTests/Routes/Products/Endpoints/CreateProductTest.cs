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
[TestOf(typeof(CreateProduct))]
public class CreateProductTest
{
    private DbContextFactoryFixture _fixture;
    private ApplicationDbContext _context;
    private Mock<IImageProcessor> _imageProcessorMock;
    private Mock<IFormFile> _formFileMock;

    [SetUp]
    public void SetUp()
    {
        _fixture = new DbContextFactoryFixture();
        _context = _fixture.GetDbContext();
        _context.Categories.Add(new Category { Name = "category1", Description = "category1 desc" });

        const string testImageUrl = "https://example.com/images/test-image.webp";
        _imageProcessorMock = new Mock<IImageProcessor>();
        _imageProcessorMock
            .Setup(x => x.ProcessAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync(testImageUrl);

        _formFileMock = new Mock<IFormFile>();
        _formFileMock.Setup(f => f.Length).Returns(100);
        _formFileMock.Setup(f => f.ContentType).Returns("image/webp");
        _formFileMock.Setup(f => f.OpenReadStream()).Returns(Stream.Null);
        _formFileMock.Setup(f => f.Name).Returns("file");
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
        
        var itemDto = new ProductCreateRequest("Test Item", 
            "Test Description",
            1, // Assuming category exists
            19.99m,
            IsVisible: true, _formFileMock.Object);

        // Act
        var result = await CreateProduct.Handle(_context, _imageProcessorMock.Object, itemDto);

        // Assert
        result.Should().NotBeNull();

        var createdResult = result.Result as CreatedAtRoute<ApiResponse>;
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
        var itemDto = new ProductCreateRequest("Test Item", "Test Description", 1, 19.99m, true, _formFileMock.Object);

        // Act - Simulate a database error by disposing the context
        await _context.DisposeAsync();
        var result = (await CreateProduct.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()),_imageProcessorMock.Object, itemDto)).Result;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<InternalServerError<ApiResponse>>();

        var internalErrorResult = (InternalServerError<ApiResponse>)result;
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }

    [Test]
    public async Task Handle_ValidItemWithMinimalData_SuccessfullyCreatesItem()
    {
        // Arrange
        var itemDto = new ProductCreateRequest("Test Item", 
            "", // Minimal description
            1, 
            0m, // Minimum price
            false, // Non-default visibility
            _formFileMock.Object
        );

        // Act
        var result = (await CreateProduct.Handle(_context, _imageProcessorMock.Object, itemDto)).Result;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtRoute<ApiResponse>>();

        var createdResult = (CreatedAtRoute<ApiResponse>)result;
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
        var itemDto1 = new ProductCreateRequest("Test Item 1", "Test Description 1", 1, 19.99m, true, _formFileMock.Object);

        var itemDto2 = new ProductCreateRequest("Test Item 2", "Test Description 2", 1, 29.99m, false, _formFileMock.Object);

        // Act
        var result1 = (await CreateProduct.Handle(_context, _imageProcessorMock.Object, itemDto1)).Result;
        var result2 = (await CreateProduct.Handle(_context, _imageProcessorMock.Object, itemDto2)).Result;

        // Assert
        result1.Should().NotBeNull();
        result1.Should().BeOfType<CreatedAtRoute<ApiResponse>>();

        result2.Should().NotBeNull();
        result2.Should().BeOfType<CreatedAtRoute<ApiResponse>>();

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
        var itemDto = new ProductCreateRequest("Test Item", longDescription, 1, 19.99m, true, _formFileMock.Object);
        
        // Act
        var result = (await CreateProduct.Handle(_context, _imageProcessorMock.Object, itemDto)).Result;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtRoute<ApiResponse>>();

        var createdResult = (CreatedAtRoute<ApiResponse>)result;
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