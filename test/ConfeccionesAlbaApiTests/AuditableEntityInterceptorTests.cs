using ConfeccionesAlba_Api.Data;
using ConfeccionesAlba_Api.Models;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;

namespace ConfeccionesAlbaApiTests
{
    [TestFixture]
    public class AuditableEntityInterceptorTests
    {
        private string _testDatabaseName;
        private DbContextOptions<ApplicationDbContext> _options;

        [SetUp]
        public void Setup()
        {
            // Configure in-memory database options with a unique name for each test
            _testDatabaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: _testDatabaseName)
                .Options;
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database after each test
            using var context = new ApplicationDbContext(_options);
            context.Database.EnsureDeleted();
        }

        [Test]
        public void Category_CreatedOnAndUpdatedOn_SetWhenAdded()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);

            var category = new Category
            {
                Name = "Test Category",
                Slug = "test-category",
                Description = "Test Description"
            };

            // Act
            context.Categories.Add(category);
            context.SaveChanges();

            // Assert
            category.CreatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().NotBe(default);
            category.CreatedOn.Should().Be(category.UpdatedOn);
        }

        [Test]
        public void Category_UpdatedOn_SetWhenModified()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);

            var category = new Category
            {
                Name = "Test Category",
                Slug = "test-category",
                Description = "Test Description"
            };

            context.Categories.Add(category);
            context.SaveChanges();

            // Act - Simulate a modification
            category.Description = "Updated Description";
            context.SaveChanges();

            // Assert
            category.CreatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().BeAfter(category.CreatedOn);
        }

        [Test]
        public void Item_CreatedOn_SetWhenAdded()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);

            var item = new Item
            {
                Name = "Test Item",
                Description = "Test Description",
                PriceReference = 10.99m,
                IsVisible = true
            };

            // Act
            context.Items.Add(item);
            context.SaveChanges();

            // Assert
            item.CreatedOn.Should().NotBe(default);
            item.UpdatedOn.Should().NotBe(default);
            item.CreatedOn.Should().Be(item.UpdatedOn);
        }

        [Test]
        public void Item_UpdatedOn_SetWhenModified()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);

            var item = new Item
            {
                Name = "Test Item",
                Description = "Test Description",
                PriceReference = 10.99m,
                IsVisible = true
            };

            context.Items.Add(item);
            context.SaveChanges();

            // Act - Simulate a modification
            item.PriceReference = 15.99m;
            context.SaveChanges();

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
        public void Multiple_Entities_Auditing()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);

            var category = new Category
            {
                Name = "Test Category",
                Slug = "test-category",
                Description = "Test Description"
            };

            var item = new Item
            {
                Name = "Test Item",
                Description = "Test Description",
                PriceReference = 10.99m,
                IsVisible = true,
                Category = category
            };

            // Act
            context.Add(item);
            context.SaveChanges();

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

            context.SaveChanges();

            // Assert both entities have updated timestamps
            category.UpdatedOn.Should().NotBe(default);
            item.UpdatedOn.Should().NotBe(default);
            category.UpdatedOn.Should().BeAfter(category.CreatedOn);
            item.UpdatedOn.Should().BeAfter(item.CreatedOn);
        }
    }
}