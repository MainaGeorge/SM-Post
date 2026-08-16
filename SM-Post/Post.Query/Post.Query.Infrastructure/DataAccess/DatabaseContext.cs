using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entitites;

namespace Post.Query.Infrastructure.DataAccess;

public class DatabaseContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<PostEntity> Post { get; set; }
}
