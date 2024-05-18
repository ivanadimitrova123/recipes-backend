using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recipes_backend.Models;
using recipes_backend.Models.Dto;

namespace recipes_backend.Controllers
{
    [Route("api/report")]
    [ApiController]
    
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
            public UserDto User { get; set; }
            public int numReports { get; set; } = 0;

            public ReportedCommentDTO(int commentId,UserDto user,Comment comment, int numReports)
            {
                this.commentId = commentId;
                this.Comment = comment;
                this.numReports = numReports;
                this.User = user;
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
                    var currComment = _context.Comments.Include(c=>c.User).FirstOrDefault(c =>  c.CommentId == comment.CommentId);
                    string img = "";
                    if (currComment.User.ProfilePictureId != null)
                        img = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/image/{currComment.User.ProfilePictureId}";
                    UserDto userDto = new UserDto(
                                            currComment.User.Id,
                                            currComment.User.Username,
                                            currComment.User.FirstName,
                                            currComment.User.LastName,
                                            currComment.User.Email,
                                            img,
                                            currComment.User.Role
                                            );
                    var rcdto = new ReportedCommentDTO(comment.CommentId,userDto,currComment, 1);
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

            var reportedComment = _context.ReportedComments.FirstOrDefault(c=> c.CommentId==commentId && c.UserId == userId);
            var reportedCommentByOthers = _context.ReportedComments.FirstOrDefault(c => c.CommentId == commentId);
            if(reportedComment != null) {
                return Ok("Already reported");
            }
            else
            {
                if (reportedCommentByOthers != null)
                {
                    return Ok("Comment Reported");
                }
                else
                {
                    reportedComment = new ReportedComment
                    {
                        UserId = userId,
                        CommentId = commentId
                    };
                    _context.ReportedComments.Add(reportedComment);
                    _context.SaveChanges();
                }
                
            }

            

       

            return Ok("Comment Reported");
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> AllowReportedComment(int commentId)
        {
            var comment = await _context.ReportedComments.FirstOrDefaultAsync(c => c.CommentId == commentId);
            if (comment == null)
            {
                return BadRequest("Comment does not exist");
            }

            _context.Remove(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("delete/{commentId}")]
        public async Task<IActionResult> DeleteReportedComment(int commentId)
        {
            var reportedComment = await _context.ReportedComments.FirstOrDefaultAsync(c => c.CommentId == commentId);
            if (reportedComment == null)
            {
                return BadRequest("Comment does not exist");
            }

            _context.ReportedComments.Remove(reportedComment);
            await _context.SaveChangesAsync();

            var comment = _context.Comments.FirstOrDefault(c => c.CommentId == commentId);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
