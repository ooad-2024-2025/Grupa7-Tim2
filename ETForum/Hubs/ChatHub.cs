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
        private static Dictionary<string, string> OnlineUsers = new Dictionary<string, string>();

        public ChatHub(ETForumDbContext context)
        {
            _context = context;
        }
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier ?? Context.User?.Identity?.Name ?? "Gost";
            OnlineUsers[Context.ConnectionId] = userId;

            await NotifyOnlineUsers();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            OnlineUsers.Remove(Context.ConnectionId);
            await NotifyOnlineUsers();
            await base.OnDisconnectedAsync(exception);
        }

        private async Task NotifyOnlineUsers()
        {
            // Skupljamo samo unique ID-eve korisnika koji su online
            var onlineUserIds = OnlineUsers.Values.Where(u => u != "Gost").Distinct().ToList();
            await Clients.All.SendAsync("UpdateOnlineUsers", onlineUserIds);
        }
        public async Task SendMessage(string message)
        {
            var username = Context.User?.Identity?.Name ?? "Gost";
            string korisnikId = null;

            var korisnik = await _context.Korisnici.FirstOrDefaultAsync(k => k.UserName == Context.User.Identity.Name);
            if (korisnik != null)
            {
                korisnikId = korisnik.Id;

                // PROVJERA BANA
                if (korisnik.BanDo != null && korisnik.BanDo > DateTime.Now)
                {
                    // Obavijesti korisnika da je banovan
                    await Clients.Caller.SendAsync("BannedInfo", $"Zabranjeno pisanje u Live Chat do {korisnik.BanDo.Value:dd.MM.yyyy. HH:mm} (Razlog: {korisnik.BanRazlog ?? "nema"}).");
                    return;
                }
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

            await Clients.All.SendAsync("ReceiveMessage", username, message);
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

