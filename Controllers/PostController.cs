﻿using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;

namespace Blog.Controllers
{
    [ApiController]
    public class PostController : Controller
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync(
            [FromServices]BlogDataContext context,
            [FromQuery]int page = 0,
            [FromQuery]int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();

                var posts = await context.Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .Select(x => new PostListViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("05X04 - Falha interna"));
            }
        }
    }
}
