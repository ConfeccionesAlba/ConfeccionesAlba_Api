using ConfeccionesAlba_Api.Data;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;

namespace ConfeccionesAlbaApiTests
{
    [TestFixture]
    public class InMemoryDatabaseTestSuite
    {
        [SetUp]
        public void Setup()
        {
        }

        private DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
        }

        [Test]
        public void Test_Database_Creation_Success()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            var context = new ApplicationDbContext(options);

            // Act
            var result = context.Database.EnsureCreated();

            // Assert
            result.Should().BeTrue();
        }
    }
}