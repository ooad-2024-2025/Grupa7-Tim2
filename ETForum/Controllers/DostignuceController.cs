using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ETForum.Data;
using ETForum.Models;

namespace ETForum.Controllers
{
    public class DostignuceController : Controller
    {
        private readonly ETForumDbContext _context;

        public DostignuceController(ETForumDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Leaderboard()
        {
          
            return View();
        }
    }
}
