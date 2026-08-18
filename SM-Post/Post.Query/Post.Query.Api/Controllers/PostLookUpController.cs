using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entitites;

namespace Post.Query.Api.Controllers;

[Route("api/v1/posts")]
[ApiController]
public class PostLookUpController(ILogger<PostLookUpController> logger, IQueryDispatcher<PostEntity> queryDispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAllPost()
    {
        try
        {
            var posts = await queryDispatcher.SendAsync(new FindAllPostQuery());

            if (posts == null || posts.Count == 0)
                return NoContent();

            return Ok(new PostLookUpResponse
            {
                Posts = posts,
                Message = $"successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}",
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR = "Error while processing request to retrieve all posts!";

            logger.LogError(ex, SAFE_ERROR);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR });
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult> GetPostById(Guid id)
    {
        try
        {
            var post = await queryDispatcher.SendAsync(new FindPostByIdQuery { Id = id });

            if (post == null)
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponse { Message = $"could not find post with id {id}!" });

            return Ok(new PostLookUpResponse { Message = "successfully returned post", Posts = post });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR = "Error while processing request to retrieve a post by id!";

            logger.LogError(ex, SAFE_ERROR);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR });
        }
    }

    [HttpGet]
    [Route("byauthor/{author}")]
    public async Task<ActionResult> GetPostsByAuthor(string author)
    {
        try
        {
            var posts = await queryDispatcher.SendAsync(new FindPostByAuthorQuery { Author = author });

            if (posts == null)
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponse { Message = $"could not find any posts by author '{author}'!" });

            return Ok(new PostLookUpResponse { Posts = posts, Message = $"successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}" });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR = "Error while processing request to retrieve posts by author!";

            logger.LogError(ex, SAFE_ERROR);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR });
        }
    }

    [HttpGet]
    [Route("withlikes/{numberOfLikes}")]
    public async Task<ActionResult> GetPostsWithLikes(int numberOfLikes)
    {
        try
        {
            var posts = await queryDispatcher.SendAsync(new FindPostsWithLikesQuery { MinimumNumberOfLikes = numberOfLikes });

            if (posts == null)
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponse { Message = $"could not find any posts at least '{numberOfLikes}' likes!" });

            return Ok(new PostLookUpResponse { Posts = posts, Message = $"successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}" });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR = "Error while processing request to retrieve posts with likes!";

            logger.LogError(ex, SAFE_ERROR);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR });
        }
    }

    [HttpGet]
    [Route("withcomments")]
    public async Task<ActionResult> GetPostsWithComments()
    {
        try
        {
            var posts = await queryDispatcher.SendAsync(new FindPostWithCommentsQuery());

            if (posts == null || posts.Count == 0)
                return StatusCode(StatusCodes.Status404NotFound, new BaseResponse { Message = $"could not find any posts with comments!" });

            return Ok(new PostLookUpResponse { Posts = posts, Message = $"successfully returned {posts.Count} post{(posts.Count > 1 ? "s" : string.Empty)}" });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR = "Error while processing request to retrieve posts with comments!";

            logger.LogError(ex, SAFE_ERROR);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse { Message = SAFE_ERROR });
        }
    }
}
