using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entitites;
using Post.Query.Domain.Repository;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class CommentRepository(DatabaseContextFactory contextFactory) : ICommentRepository
{
    public async Task CreateAsync(CommentEntity comment)
    {
        using var context = contextFactory.CreateDatabaseContext();
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        using var context = contextFactory.CreateDatabaseContext();
        var comment = await GetByIdAsync(commentId);

        if (comment is null)
            return;

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
    }

    public async Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
        using var context = contextFactory.CreateDatabaseContext();
        return await context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using var context = contextFactory.CreateDatabaseContext();
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }
}
