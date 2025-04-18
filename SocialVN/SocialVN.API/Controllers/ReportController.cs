using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SocialVN.API.Repositories;
using System.Security.Claims;

namespace SocialVN.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IFriendshipRepository _friendRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ICommentRepository _commentRepository;

        public ReportController(IPostRepository postRepository, IFriendshipRepository friendRepository,
            ILikeRepository likeRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _friendRepository = friendRepository;
            _likeRepository = likeRepository;
            _commentRepository = commentRepository;
        }

        [HttpGet("generate-report")]
        public async Task<IActionResult> GenerateReport()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Lấy ID của người dùng hiện tại

            // Tính toán số bài viết tuần qua
            var postsLastWeek = await _postRepository.GetPostsCreatedInLastWeek(userId);
            int numberOfPostsLastWeek = postsLastWeek.Count();

            // Tính toán số bạn bè mới
            var newFriendsLastWeek = await _friendRepository.GetNewFriendsInLastWeek(userId);
            int numberOfNewFriends = newFriendsLastWeek.Count();

            // Tính toán số lượt like mới
            var likesLastWeek = await _likeRepository.GetLikesInLastWeek(userId);
            int numberOfLikesLastWeek = likesLastWeek.Count();

            // Tính toán số bình luận mới
            var commentsLastWeek = await _commentRepository.GetCommentsInLastWeek(userId);
            int numberOfCommentsLastWeek = commentsLastWeek.Count();

            // Tạo file Excel
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Report");

                // Thêm tiêu đề
                worksheet.Cells[1, 1].Value = "Tính năng báo cáo";
                worksheet.Cells[2, 1].Value = "Số bài viết tuần qua";
                worksheet.Cells[2, 2].Value = numberOfPostsLastWeek;

                worksheet.Cells[3, 1].Value = "Số bạn bè mới";
                worksheet.Cells[3, 2].Value = numberOfNewFriends;

                worksheet.Cells[4, 1].Value = "Số lượt like mới";
                worksheet.Cells[4, 2].Value = numberOfLikesLastWeek;

                worksheet.Cells[5, 1].Value = "Số bình luận mới";
                worksheet.Cells[5, 2].Value = numberOfCommentsLastWeek;

                // Lưu file
                package.Save();
            }

            stream.Position = 0;
            var fileName = $"Report_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

    }
}
