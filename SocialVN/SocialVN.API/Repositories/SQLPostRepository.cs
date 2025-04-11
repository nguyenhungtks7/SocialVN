using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLPostRepository : IPostRepository
    {
        private readonly SocialVNDbContext dbContext;

        public SQLPostRepository(SocialVNDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Post> CreateAsync(Post post)
        {
            await dbContext.Posts.AddAsync(post);
            await dbContext.SaveChangesAsync();
            return post;
        }

        public async Task<Post> DeleteAsync(Guid id)
        {
            var existingPost = dbContext.Posts.FirstOrDefault(x => x.Id == id);
            if(existingPost == null)
            {
                return null;
            }
            dbContext.Posts.Remove(existingPost);
            await dbContext.SaveChangesAsync();
            return existingPost;
        }

        public async Task<List<Post>> GetAllAsync()
        {
            return await dbContext.Posts.ToListAsync();
        }

        public async Task<Post> GetByIdAsync(Guid id)
        {
            return await dbContext.Posts.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
          
        }

        public async Task<Post> UpdateAsync(Guid id, Post post)
        {
            var existingPost = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);
            if (existingPost == null)
            {
                return null;
            }
            existingPost.UserId = post.UserId;
            existingPost.Content = post.Content;
            existingPost.UpdatedAt = DateTime.Now;
            await dbContext.SaveChangesAsync();
            return existingPost;

        }
    }
}
