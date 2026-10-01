var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Fake data for now. This moves to a real database later.
var events = new List<Event>
{
    new(1, "Yankees vs. Red Sox", "Yankee Stadium", new DateTime(2026, 10, 18), 89.00m),
    new(2, "Knicks vs. Celtics", "Madison Square Garden", new DateTime(2026, 11, 14), 185.00m),
    new(3, "Hamilton", "Richard Rodgers Theatre", new DateTime(2026, 12, 5), 249.00m),
};

app.MapGet("/events", () => events);

app.MapGet("/events/{id}", (int id) =>
{
    var ev = events.FirstOrDefault(e => e.Id == id);
    return ev is null ? Results.NotFound() : Results.Ok(ev);
});

app.Run();

record Event(int Id, string Name, string Venue, DateTime Date, decimal LowestPrice);