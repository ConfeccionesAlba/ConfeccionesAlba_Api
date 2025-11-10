using ConfeccionesAlba_Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ConfeccionesAlbaApiTests.Common;

public sealed class DbContextFactoryFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _fixture;

    public DbContextFactoryFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        _fixture = new ApplicationDbContext(options);
        _fixture.Database.EnsureCreated();
    }

    public ApplicationDbContext GetDbContext()
    {
        return _fixture;
    }
    
    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
        _fixture.Dispose();
    }
}
