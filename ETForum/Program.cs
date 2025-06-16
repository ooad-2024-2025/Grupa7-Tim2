using ETForum.Data;
using ETForum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ETForum.Hubs;
using ETForum.Helper;
using ETForum.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<InboxPrijateljiFilter>();
builder.Services.AddDbContext<ETForumDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<PrijateljiFilter>();
    options.Filters.AddService<NeprocitaneNotifikacijeFilter>();
    options.Filters.AddService<InboxPrijateljiFilter>();
});
builder.Services.AddScoped<NeprocitaneNotifikacijeFilter>();

builder.Services.AddScoped<PrijateljiFilter>();

builder.Services.AddIdentity<Korisnik, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ETForumDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Korisnik/Login"; 
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "ETForum.Auth";
    options.SlidingExpiration = true;
    options.Events.OnSigningIn = context =>
    {
        var identity = (System.Security.Claims.ClaimsIdentity)context.Principal.Identity;
        var userId = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
           
            identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, userId));
        }

        return Task.CompletedTask;
    };

});


builder.Services.AddScoped<DostignucaHelper>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Korisnik/Login";
});

//za notifikacije
builder.Services.AddTransient<EmailSender>();






var app = builder.Build();

//chat mi reko da ovo promjenim
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] uloge = { "Student", "Profesor", "Asistent", "Administrator" };

    foreach (var uloga in uloge)
    {
        if (!roleManager.RoleExistsAsync(uloga).Result) //  KORISTI .Result UMJESTO await
        {
            roleManager.CreateAsync(new IdentityRole(uloga)).Wait(); //  KORISTI .Wait()
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeed.SeedRolesAndAdministrator(services);
}





// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.MapHub<ChatHub>("/livechat");
app.MapHub<NotificationHub>("/notificationHub");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Korisnik}/{action=Login}/{id?}")
    .WithStaticAssets();



app.Run();
