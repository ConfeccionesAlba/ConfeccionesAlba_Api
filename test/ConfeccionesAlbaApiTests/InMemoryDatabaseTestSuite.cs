using ConfeccionesAlba_Api.Data;
using Microsoft.EntityFrameworkCore;
using AwesomeAssertions;

namespace ConfeccionesAlbaApiTests
{
    [TestFixture]
    public class InMemoryDatabaseTestSuite
    {
        private string _testDatabaseName;

        [SetUp]
        public void Setup()
        {
            // Configure in-memory database options with a unique name for each test
            _testDatabaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";
        }

        private DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: _testDatabaseName)
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