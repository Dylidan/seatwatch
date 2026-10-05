using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SeatWatch")));

var app = builder.Build();

app.MapGet("/events", async (AppDbContext db) => await db.Events.ToListAsync());

app.MapGet("/events/{id}", async (int id, AppDbContext db) =>
{
    var ev = await db.Events.FindAsync(id);
    return ev is null ? Results.NotFound() : Results.Ok(ev);
});

app.Run();