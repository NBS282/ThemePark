using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ThemePark.DataAccess.Utils;

public sealed class DbContextBuilder
{
    private static readonly SqliteConnection _connection = new("Data Source=:memory:");

    public static ThemeParkDbContext BuildTestDbContext()
    {
        var options = new DbContextOptionsBuilder<ThemeParkDbContext>()
            .UseSqlite(_connection)
            .Options;

        _connection.Open();

        var context = new ThemeParkDbContext(options);

        return context;
    }
}
