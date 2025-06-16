using ETForum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using ETForum.Models;

public class NeprocitaneNotifikacijeFilter : IActionFilter
{
    private readonly ETForumDbContext _context;
    private readonly UserManager<Korisnik> _userManager;

    public NeprocitaneNotifikacijeFilter(ETForumDbContext context, UserManager<Korisnik> userManager)
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
                var broj = _context.Notifikacije
                    .Where(n => n.KorisnikId == userId && !n.Procitano)
                    .Count();
                controller.ViewBag.NeprocitaneNotifikacije = broj;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
