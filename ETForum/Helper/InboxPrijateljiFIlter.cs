using ETForum.Data;
using ETForum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class InboxPrijateljiFilter : IActionFilter
{
    private readonly ETForumDbContext _context;
    private readonly UserManager<Korisnik> _userManager;

    public InboxPrijateljiFilter(ETForumDbContext context, UserManager<Korisnik> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller as Microsoft.AspNetCore.Mvc.Controller;
        if (controller != null && controller.User.Identity.IsAuthenticated)
        {
            var userId = _userManager.GetUserId(controller.User);
            if (!string.IsNullOrEmpty(userId))
            {
                var inboxPrijatelji = _context.Prijateljstva
                    .Include(p => p.korisnik1)
                    .Include(p => p.korisnik2)
                    .Where(p => (p.korisnik1Id == userId || p.korisnik2Id == userId) && p.status == Status.PRIHVACENO)
                    .Select(p => p.korisnik1Id == userId ? p.korisnik2 : p.korisnik1)
                    .ToList();
                controller.ViewBag.InboxPrijatelji = inboxPrijatelji;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
