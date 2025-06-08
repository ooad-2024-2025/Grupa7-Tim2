using ETForum.Data;
using ETForum.DTO;
using ETForum.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ETForum.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ETForumDbContext _context;
        public ChatHub(ETForumDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string message)
        {
            var username = Context.User?.Identity?.Name ?? "Gost";

            string korisnikId = null;

            var korisnik = await _context.Korisnici.FirstOrDefaultAsync(k => k.UserName == Context.User.Identity.Name);
            if (korisnik != null)
            {
                korisnikId = korisnik.Id;
            }

            var novaPoruka = new LiveChat
            {
                korisnikId = korisnikId,
                username = username,
                poruka = message,
                vrijeme = DateTime.Now
            };

            _context.LiveChat.Add(novaPoruka);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", username , message);
        }


        public async Task<List<MessageDTO>> GetRecentMessages(int count = 100)
        {
            var messageDtos = await _context.LiveChat
            .OrderByDescending(m => m.vrijeme)
            .Take(count)
            .Select(m => new MessageDTO
            {
                userName = m.korisnikId == null || m.korisnik == null ? "Gost" : m.korisnik.UserName,
                poruka = m.poruka,
                vrijeme = m.vrijeme
            })
            .OrderBy(m => m.vrijeme) 
            .ToListAsync();

            return messageDtos;
        }


    }

}

