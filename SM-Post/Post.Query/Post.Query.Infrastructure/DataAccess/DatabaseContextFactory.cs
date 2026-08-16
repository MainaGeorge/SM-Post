using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.DataAccess;

public class DatabaseContextFactory(Action<DbContextOptionsBuilder> contextOptionsBuilder)
{
    public DatabaseContext CreateDatabaseContext()
    {
        DbContextOptionsBuilder<DatabaseContext> options = new();

        contextOptionsBuilder(options);

        return new DatabaseContext(options.Options);
    }
}
