using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SeatWatch")));

builder.Services.AddHostedService<PriceCheckService>();

var app = builder.Build();

app.MapGet("/events", async (AppDbContext db) => await db.Events.ToListAsync());

app.MapGet("/events/{id}", async (int id, AppDbContext db) =>
{
    var ev = await db.Events.FindAsync(id);
    return ev is null ? Results.NotFound() : Results.Ok(ev);
});

app.MapGet("/alerts", async (AppDbContext db) => await db.PriceAlerts.ToListAsync());

app.MapPost("/alerts", async (CreateAlertRequest request, AppDbContext db) =>
{
    if (request.TargetPrice <= 0)
        return Results.BadRequest("Target price must be greater than zero.");

    var ev = await db.Events.FindAsync(request.EventId);
    if (ev is null)
        return Results.NotFound("Event not found.");

    var alert = new PriceAlert
    {
        EventId = request.EventId,
        Email = request.Email,
        TargetPrice = request.TargetPrice,
        CreatedAt = DateTime.UtcNow
    };

    db.PriceAlerts.Add(alert);
    await db.SaveChangesAsync();

    return Results.Created($"/alerts/{alert.Id}", alert);
});

app.Run();

record CreateAlertRequest(int EventId, string Email, decimal TargetPrice);