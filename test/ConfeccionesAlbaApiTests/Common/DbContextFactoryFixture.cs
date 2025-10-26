using ConfeccionesAlba_Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlbaApiTests.Common;

public sealed class DbContextFactoryFixture
{
    private readonly ApplicationDbContext _fixture;
    private readonly string _testDatabaseName
        ;
    public DbContextFactoryFixture()
    {
        // Configure in-memory database options with a unique name for each test
        _testDatabaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";

        _fixture = new ApplicationDbContext(CreateInMemoryOptions());
    }

    private DbContextOptions<ApplicationDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _testDatabaseName)
            .Options;
    }

    public ApplicationDbContext GetDbContext()
    {
        return _fixture;
    }
}
