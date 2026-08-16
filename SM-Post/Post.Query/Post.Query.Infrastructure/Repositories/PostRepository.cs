using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entitites;
using Post.Query.Domain.Repository;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository(DatabaseContextFactory contextFactory) : IPostRepository
{
    public async Task CreateAsync(PostEntity entity)
    {
        using var context = contextFactory.CreateDatabaseContext();
        context.Post.Add(entity);

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using var context = contextFactory.CreateDatabaseContext();
        var post = await GetByIdAsync(postId);

        if (post is null)
            return;

        context.Post.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using var context = contextFactory.CreateDatabaseContext();
        return await context.Post.Include(p => p.Comments).FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using var context = contextFactory.CreateDatabaseContext();
        return await context.Post.AsNoTracking().Include(p => p.Comments).ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using var context = contextFactory.CreateDatabaseContext();
        return await context.Post.AsNoTracking().Include(p => p.Comments).Where(a => a.Author.Contains(author)).ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using var context = contextFactory.CreateDatabaseContext();
        return await context.Post.AsNoTracking().Include(p => p.Comments).Where(a => a.Comments != null && a.Comments.Any()).ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        using var context = contextFactory.CreateDatabaseContext();
        return await context.Post.AsNoTracking().Include(p => p.Comments).Where(a => a.Likes >= numberOfLikes).ToListAsync();
    }

    public async Task UpdateAsync(PostEntity entity)
    {
        using var context = contextFactory.CreateDatabaseContext();
        context.Post.Update(entity);
        await context.SaveChangesAsync();
    }
}
