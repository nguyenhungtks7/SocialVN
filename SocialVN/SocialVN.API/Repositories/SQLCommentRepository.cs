﻿using Microsoft.EntityFrameworkCore;
using SocialVN.API.Data;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public class SQLCommentRepository : ICommentRepository
    {
        private readonly SocialVNDbContext dbContext;
        public SQLCommentRepository(SocialVNDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await dbContext.Comments.AddAsync(comment);
            await dbContext.SaveChangesAsync();
            return comment;
        }
        public async Task<Comment> DeleteCommentAsync(Guid id)
        {
            var existingComment = await dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (existingComment != null)
            {
                dbContext.Comments.Remove(existingComment);
                await dbContext.SaveChangesAsync();
                return existingComment;
            }
            return null;
        }
        public async Task<List<Comment>> GetAllCommentsAsync(string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000)
        {
            var coments = dbContext.Comments.Include(x=>x.Post).Include(x=>x.User).AsQueryable();
            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    coments = isAscending ? coments.OrderBy(x => x.CreatedAt) : coments.OrderByDescending(x => x.CreatedAt);
                }
            }
            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;
            return await coments.Skip(skipResults).Take(pageSize).ToListAsync();
        }
        public Task<Comment?> GetCommentByIdAsync(Guid id)
        {
             return dbContext.Comments.Include(x=>x.User).Include(x => x.Post).FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Comment> UpdateCommentAsync(Guid id, Comment comment)
        {
            var existingComment = await dbContext.Comments.FirstOrDefaultAsync(x => x.Id == id);
            if (existingComment != null)
            {
                existingComment.PostId = comment.PostId;
                existingComment.UserId = comment.UserId;
                existingComment.Content = comment.Content;
                existingComment.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return existingComment;
            }
            return null;
        }
        public async Task<IEnumerable<Comment>> GetCommentsInLastWeek(string userId)
        {
            var lastWeek = DateTime.Now.AddDays(-7);
            return await dbContext.Comments
                .Where(c => c.UserId == userId && c.CreatedAt >= lastWeek)
                .ToListAsync();
        }
    }
}
