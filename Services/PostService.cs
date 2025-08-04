using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Models;

public class PostService
{
  private readonly IMongoCollection<Post> _posts;

  public PostService(IOptions<MongoDbSettings> settings)
  {
    var client = new MongoClient(settings.Value.ConnectionString);
    var database = client.GetDatabase(settings.Value.DatabaseName);
    _posts = database.GetCollection<Post>("posts");
  }

  public async Task<Post> CreateAsync(Post post)
  {
    await _posts.InsertOneAsync(post);
    return post;
  }

  public async Task<Post> GetByIdAsync(string id)
  {
    return await _posts.Find(p => p.Id.ToString() == id).FirstOrDefaultAsync();
  }

  public async Task<List<Post>> GetPostsByUserIdAsync(string userId)
  {
    return await _posts.Find(p => p.UserId == userId && !p.IsDeleted).SortByDescending(p => p.CreatedAt).ToListAsync();
  }
}