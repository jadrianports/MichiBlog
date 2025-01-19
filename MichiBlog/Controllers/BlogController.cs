using AspNetCoreHero.ToastNotification.Abstractions;
using MichiBlog.Models;
using MichiBlog.WebApp.Data;
using MichiBlog.WebApp.Models;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MichiBlog.WebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notification { get; }
        public BlogController(ApplicationDbContext context,
                              INotyfService notification,
                              UserManager<ApplicationUser> userManager) {
            _context = context;
            _notification = notification;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Post(string slug)
        {
            if (string.IsNullOrEmpty(slug)) {
                _notification.Error("Post not found");
                return RedirectToAction("Index", "Home");
            }
            var post = _context.Posts!
                .Include(x => x.ApplicationUser)
                .Include(x => x.Comments)
                .ThenInclude(x => x.User)
                .Include(x => x.Reactions)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Slug == slug);

            if (post == null) {
                _notification.Error("Post not found");
                return RedirectToAction("Index", "Home");
            }
            
            var userId = _userManager.GetUserId(User);
            var currentReaction = post.Reactions.FirstOrDefault(x => x.UserId == userId);

            var likesCount = post.Reactions.Count(r => r.Liked);
            var dislikesCount = post.Reactions.Count(r => r.Disliked);

            var blogPostVM = new BlogPostVM() {
                Id = post.Id,   
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Content = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
                Author = post.ApplicationUser!.FirstName + " " + post.ApplicationUser.LastName,
                CreatedDate = post.CreatedDate,
                Comments = post.Comments,
                Reactions = post.Reactions,
                LikesCount = likesCount,
                DislikesCount = dislikesCount,
                CurrentLiked = currentReaction?.Liked ?? false,
                CurrentDisliked = currentReaction?.Disliked ?? false
            };
            return View(blogPostVM);
        }
        [HttpPost]
        public IActionResult AddComment(int postId, string content, int? parentCommentID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var post = _context.Posts!.FirstOrDefault(x => x.Id == postId);
            if (post == null)
            {
                _notification.Error("Post not found");
                return RedirectToAction("Index", "Home"); // Redirect if post doesn't exist
            }

            var comment = new Comment
            {
                Content = content,
                PostId = postId,
                UserId = userId,
                ParentCommentId = parentCommentID,
                CreatedDate = DateTime.Now
            };

            _context.Comments!.Add(comment);
            _context.SaveChanges();

            // Redirect to the Post page using the slug
            return RedirectToAction("Post", new { slug = post.Slug });
        }

        [HttpPost]
        public async Task<IActionResult> AddReaction(int postId, bool liked, bool disliked)
        {
            var userId = _userManager.GetUserId(User); // Get current user's ID
            if (userId == null)
            {
                return BadRequest("User is not authenticated.");
            }
            var existingReaction = await _context.Reactions
                                                  .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);

            if (existingReaction == null)
            {
                var reaction = new Reaction
                {
                    PostId = postId,
                    Liked = liked,
                    Disliked = disliked,
                    UserId = userId
                };
                _context.Reactions.Add(reaction);
            }
            else
            {
                // If the user clicked the same reaction again, remove it
                if ((existingReaction.Liked && liked) || (existingReaction.Disliked && disliked))
                {
                    _context.Reactions.Remove(existingReaction);
                }
                else
                {
                    // Otherwise, update the existing reaction with the new one
                    existingReaction.Liked = liked;
                    existingReaction.Disliked = disliked;
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Reaction added/updated/deleted successfully.");
        }
    }
}
