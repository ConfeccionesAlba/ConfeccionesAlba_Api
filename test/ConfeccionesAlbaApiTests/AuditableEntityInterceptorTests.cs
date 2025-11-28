using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using AwesomeAssertions;
using ConfeccionesAlbaApiTests.Common;

namespace ConfeccionesAlbaApiTests
{
    [TestFixture]
    public class AuditableEntityInterceptorTests
    {
        private DbContextFactoryFixture _fixture;
        private ApplicationDbContext _context;

        [SetUp]
        public void Setup()
        {
            _fixture = new DbContextFactoryFixture();
            _context = _fixture.GetDbContext();
            _context.Categories.Add(new Category { Name = "test category", Description = "test description category" });
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database after each test
            _context.Dispose();
            _fixture.Dispose();
        }

        [Test]
        public async Task Category_CreatedOnAndUpdatedOn_SetWhenAdded()
        {
            // Arrange
            var category = new Category
            {
                Name = "Test Category",
                Description = "Test Description"
            };

            // Act
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Assert
            category.CreatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().NotBe(default);
            category.CreatedOn.Should().Be(category.UpdatedOn);
        }

        [Test]
        public async Task Category_UpdatedOn_SetWhenModified()
        {
            // Arrange
            var category = new Category
            {
                Name = "Test Category",
                Description = "Test Description"
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Act - Simulate a modification
            category.Description = "Updated Description";
            await _context.SaveChangesAsync();

            // Assert
            category.CreatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().BeAfter(category.CreatedOn);
        }

        [Test]
        public async Task Item_CreatedOn_SetWhenAdded()
        {
            // Arrange
            var item = new Product
            {
                Name = "Test Item",
                Description = "Test Description",
                CategoryId = 1,
                PriceReference = 10.99m,
                IsVisible = true,
                Image = new Image { Name = "Test name", Url = "Test url" }
            };

            // Act
            _context.Products.Add(item);
            await _context.SaveChangesAsync();

            // Assert
            item.CreatedOn.Should().NotBe(default);
            item.UpdatedOn.Should().NotBe(default);
            item.CreatedOn.Should().Be(item.UpdatedOn);
        }

        [Test]
        public async Task Item_UpdatedOn_SetWhenModified()
        {
            // Arrange
            var item = new Product
            {
                Name = "Test Item",
                Description = "Test Description",
                CategoryId = 1,
                PriceReference = 10.99m,
                IsVisible = true,
                Image = new Image { Name = "Test name", Url = "Test url" }
            };

            _context.Products.Add(item);
            await _context.SaveChangesAsync();

            // Act - Simulate a modification
            item.PriceReference = 15.99m;
            await _context.SaveChangesAsync();

            // Assert
            item.CreatedOn.Should().NotBe(default);
            item.UpdatedOn.Should().NotBe(default);
            item.UpdatedOn.Should().BeAfter(item.CreatedOn);
        }

        // [Test]
        // public void Test_Interceptor_Not_Affected_By_Non_Auditable_Entities()
        // {
        //     // Arrange
        //     using var context = new ApplicationDbContext(_options);
        //
        //     // Create a non-auditable entity (if any existed)
        //     // For this test, we'll just verify that the interceptor doesn't throw errors
        //     // when processing non-auditable entities
        //
        //     // Act & Assert - This should not throw any exceptions
        //     context.SaveChanges().Should().BeGreaterThanOrEqualTo(0);
        // }

        [Test]
        public async Task Multiple_Entities_Auditing()
        {
            // Arrange
            var category = new Category
            {
                Name = "Test Category",
                Description = "Test Description"
            };

            var item = new Product
            {
                Name = "Test Item",
                Description = "Test Description",
                PriceReference = 10.99m,
                IsVisible = true,
                Category = category,
                Image = new Image { Name = "Test name", Url = "Test url" }
            };

            // Act
            _context.Add(item);
            await _context.SaveChangesAsync();

            // Assert both entities have proper audit timestamps
            category.CreatedOn.Should().NotBe(default);
            item.CreatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().NotBe(default);
            item.UpdatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().Be(category.CreatedOn);
            item.UpdatedOn.Should().Be(item.CreatedOn);

            // Modify both entities
            category.Description = "Updated Category Description";
            item.PriceReference = 15.99m;

            await _context.SaveChangesAsync();

            // Assert both entities have updated timestamps
            category.UpdatedOn.Should().NotBe(default);
            item.UpdatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().BeAfter(category.CreatedOn);
            item.UpdatedOn.Should().BeAfter(item.CreatedOn);
        }
    }
}