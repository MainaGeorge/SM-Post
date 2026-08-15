using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, (string Comment, string Username)> _comments = [];

    public bool Active { get => _active; set => _active = value; }

    public PostAggregate()
    {  
    }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.UtcNow
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        _active = true;
        _author = @event.Author;
        _id = @event.Id;
    }

    public void EditMessage(string message) 
    {
        if (!_active)
            throw new InvalidOperationException("You can not edit the message of an inactive post!");

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException($"the value of {nameof(message)} can not be null or empty. Please provide a valid value for {nameof(message)}!");

        RaiseEvent(new MessageUpdatedEvent { Id = _id, Message = message });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void LikePost()
    {
        if(!_active)
            throw new InvalidOperationException("You can not like an inactive post!");

        RaiseEvent(new PostLikedEvent { Id = _id});
    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }

    public void AddComment(string comment, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You can not add a comment to an inactive post!");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentNullException($"the value of {nameof(comment)} can not be null or empty. Please provide a valid value for {nameof(comment)}!");

        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            Comment = comment,
            CommentId = Guid.NewGuid(),
            CommentDate = DateTime.UtcNow,
            Username = username,
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        _id = @event.Id;
        _comments.Add(@event.CommentId, (@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You can not edit a comment of an inactive post!");

        if (string.IsNullOrWhiteSpace(comment))
            throw new ArgumentNullException($"the value of {nameof(comment)} can not be null or empty. Please provide a valid value for {nameof(comment)}!");

        if (!_comments[commentId].Username.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You can not edit a comment made by another user!");

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.UtcNow
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = (@event.Comment, @event.Username);  
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You can not remove a comment of an inactive post!");

        if (!_comments[commentId].Username.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You can not remove a comment made by another user!");

        RaiseEvent(new CommentRemovedEvent { Id = _id, CommentId = commentId });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active)
            throw new InvalidOperationException("Post has already been removed!");

        if (!_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You are not allowed to delete a post made by someone else");

        RaiseEvent(new PostRemovedEvent { Id = _id });
    }

    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }
}
