using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
  private readonly PostService _postService;

  public PostController(PostService postService)
  {
    _postService = postService;
  }

  [HttpPost("create")]
  public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
  {
    if (string.IsNullOrWhiteSpace(dto.Content) || dto.Content.Length > 280)
      return BadRequest("Content must be between 1 and 280 characters.");

    var newPost = new Post
    {
      UserId = dto.UserId,
      Content = dto.Content,
      CreatedAt = DateTime.UtcNow,
      LikesCount = 0,
      IsDeleted = false
    };

    await _postService.CreateAsync(newPost);

    return Ok(new
    {
      newPost.Id,
      newPost.UserId,
      newPost.Content,
      newPost.CreatedAt
    });
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(string id)
  {
    var post = await _postService.GetByIdAsync(id);
    if (post == null || post.IsDeleted)
      return NotFound("Post not found.");

    return Ok(post);
  }
}