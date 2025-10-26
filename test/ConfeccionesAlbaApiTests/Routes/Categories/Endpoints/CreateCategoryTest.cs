using System.Net;
using ConfeccionesAlba_Api.Models;
using ConfeccionesAlba_Api.Models.Dtos.Categories;
using ConfeccionesAlba_Api.Models.Dtos.Categories.Validators;
using ConfeccionesAlba_Api.Routes.Categories.Endpoints;
using ConfeccionesAlbaApiTests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;
using ConfeccionesAlba_Api.Data;

namespace ConfeccionesAlbaApiTests.Routes.Categories.Endpoints;

[TestFixture]
[TestOf(typeof(CreateCategory))]
public class CreateCategoryTest
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
    public async Task Handle_SuccessfulCategoryCreation_ReturnsCreatedAtRoute()
    {
        // Arrange
        var categoryDto = new CategoryCreateDto
        {
            Name = "Test Category",
            Description = "Test Description"
        };

        // Act
        var result = (await CreateCategory.Handle(_context, categoryDto)).Result;
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtRoute<ApiResponse>>();

        var createdResult = result as CreatedAtRoute<ApiResponse>;
        createdResult.Value.Result.Should().BeOfType<Category>();
        createdResult.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        createdResult.Value.IsSuccess.Should().BeTrue();

        // Verify the category was added to the database
        var categories = await _context.Categories.ToListAsync();
        categories.Should().HaveCount(1);
        categories[0].Name.Should().Be(categoryDto.Name);
        categories[0].Description.Should().Be(categoryDto.Description);
    }
    
    [Test]
    public async Task Handle_DatabaseError_ReturnsInternalServerError()
    {
        // Arrange
        var categoryDto = new CategoryCreateDto
        {
            Name = "Test Category",
            Description = "Test Description"
        };
    
        // Act - Simulate a database error by disposing the context
        _context.Dispose();
        var result = (await CreateCategory.Handle(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()), categoryDto)).Result;
    
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<InternalServerError<ApiResponse>>();
    
        var internalErrorResult = (InternalServerError<ApiResponse>)result;
        internalErrorResult.Value.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        internalErrorResult.Value.IsSuccess.Should().BeFalse();
        internalErrorResult.Value.ErrorMessages.Should().HaveCount(1);
    }
    
    [Test]
    public async Task Handle_ValidCategoryWithEmptyDescription_SuccessfullyCreatesCategory()
    {
        // Arrange
        var categoryDto = new CategoryCreateDto
        {
            Name = "Test Category",
            Description = "" // Empty description should be allowed
        };
    
        // Act
        var result = (await CreateCategory.Handle(_context, categoryDto)).Result;
    
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtRoute<ApiResponse>>();
    
        var createdResult = (CreatedAtRoute<ApiResponse>)result;
        createdResult.Value.Result.Should().BeOfType<Category>();
        createdResult.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        createdResult.Value.IsSuccess.Should().BeTrue();
    
        // Verify the category was added to the database
        var categories = await _context.Categories.ToListAsync();
        categories.Should().HaveCount(1);
        categories[0].Name.Should().Be(categoryDto.Name);
        categories[0].Description.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_MultipleValidCategories_CreatesAllSuccessfully()
    {
        // Arrange
        var categoryDto1 = new CategoryCreateDto
        {
            Name = "Test Category 1",
            Description = "Test Description 1"
        };
    
        var categoryDto2 = new CategoryCreateDto
        {
            Name = "Test Category 2",
            Description = "Test Description 2"
        };
    
        // Act
        var result1 = (await CreateCategory.Handle(_context, categoryDto1)).Result;
        var result2 = (await CreateCategory.Handle(_context, categoryDto2)).Result;
    
        // Assert
        result1.Should().NotBeNull();
        result1.Should().BeOfType<CreatedAtRoute<ApiResponse>>();
    
        result2.Should().NotBeNull();
        result2.Should().BeOfType<CreatedAtRoute<ApiResponse>>();
    
        // Verify both categories were added to the database
        var categories = await _context.Categories.ToListAsync();
        categories.Should().HaveCount(2);
    
        categories.Should().Contain(c => c.Name == categoryDto1.Name && c.Description == categoryDto1.Description);
        categories.Should().Contain(c => c.Name == categoryDto2.Name && c.Description == categoryDto2.Description);
    }
}