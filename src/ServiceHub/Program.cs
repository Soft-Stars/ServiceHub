using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using ServiceHub.Data;
using SignalRHub;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Additional Identity options can be configured here
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 4;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add NLog for logging
builder.Host.UseNLog();

// Configure SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<SignalHub>();

// Add Razor Pages and Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ensure Authentication & Authorization middleware is properly ordered
app.UseAuthentication(); // This is automatically configured by AddDefaultIdentity
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Map SignalR Hub
app.MapHub<SignalHub>("/Hub");

// Map Blazor and Razor Pages
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
