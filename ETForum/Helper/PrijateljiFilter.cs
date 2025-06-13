namespace ETForum.Helper
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using ETForum.Data;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using ETForum.Models;

    public class PrijateljiFilter : IAsyncActionFilter
    {
        private readonly ETForumDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public PrijateljiFilter(ETForumDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.Controller as Controller;
            var user = context.HttpContext.User;

            if (controller != null && user.Identity.IsAuthenticated)
            {
                var mojId = _userManager.GetUserId(user);
                var prijatelji = await _context.Prijateljstva
                    .Include(p => p.korisnik1)
                    .Include(p => p.korisnik2)
                    .Where(p => (p.korisnik1Id == mojId || p.korisnik2Id == mojId) && p.status == Status.PRIHVACENO)
                    .Select(p => p.korisnik1Id == mojId ? p.korisnik2 : p.korisnik1)
                    .ToListAsync();

                controller.ViewBag.SviPrijatelji = prijatelji;
            }
            await next();
        }
    }

}
