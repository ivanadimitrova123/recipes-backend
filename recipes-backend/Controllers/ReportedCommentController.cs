using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend.Models;

namespace recipes_backend.Controllers
{
    [Route("api/report")]
    [ApiController]
    [Authorize]
    public class ReportedCommentController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ReportedCommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        private class ReportedCommentDTO
        {
            public int commentId { get; set; }
            public Comment Comment { get; set; }
            public int numReports { get; set; } = 0;

            public ReportedCommentDTO(int commentId,Comment comment, int numReports)
            {
                this.commentId = commentId;
                this.Comment = comment;
                this.numReports = numReports;
            }
        }


        [HttpGet]
        public IActionResult GetReportedComments()
        {
            var allReportedComments = _context.ReportedComments.ToList();
            List<ReportedCommentDTO> reportedComments = new List<ReportedCommentDTO>();
            List<int> commentIds = new List<int>();
            foreach (var comment in allReportedComments)
            {
                if (commentIds.Contains(comment.CommentId))
                {
                    foreach (var reportedComment in reportedComments)
                    {
                        if(reportedComment.commentId == comment.CommentId)
                        {
                            reportedComment.numReports++;
                        }
                    }
                }
                else
                {
                    commentIds.Add(comment.CommentId);
                    var currComment = _context.Comments.FirstOrDefault(c =>  c.CommentId == comment.CommentId);
                    var rcdto = new ReportedCommentDTO(comment.CommentId,currComment, 1);
                    reportedComments.Add(rcdto);
                }
            }

            return Ok(reportedComments);
        }

        [HttpPost]
        public async Task<IActionResult> ReportComment([FromForm] long userId,[FromForm] int commentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
            if (comment == null)
            {
                return BadRequest("Comment does not exist");
            }

            var reportedComment = new ReportedComment
            {
                UserId = userId,
                CommentId = commentId
            };

            _context.ReportedComments.Add(reportedComment);
            _context.SaveChanges();

            return Ok();
        }
    }
}
