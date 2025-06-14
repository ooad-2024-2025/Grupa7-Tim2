using ETForum.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETForum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly ETForumDbContext _context;
        public UsersApiController(ETForumDbContext context) => _context = context;

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var korisnici = await _context.Korisnici
                .Select(k => new {
                    id = k.Id,
                    nickname = k.nickname ?? (k.ime + " " + k.prezime),
                    isBannedLiveChat = k.BanDo != null && k.BanDo > DateTime.Now,
                    banDo = k.BanDo,
                    banRazlog = k.BanRazlog
                }).ToListAsync();
            return Ok(korisnici);
        }

    }
}
