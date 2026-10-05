Used Claude to plan the project and set up the API; learned what HTTP status codes are.

A 404 error doesn't always look like an error in the browser

In Chrome or Edge, you'll usually get a gray page that says "This localhost page can't be found" with HTTP ERROR 404 underneath. In some browsers the page is just blank, and that counts as a pass too.

Why it looks like that: Results.NotFound() doesn't send back a message. It only sends a status code, a number the server attaches to every response to say how things went. 200 means "OK, here's your data," and 404 means "that doesn't exist." Your code sends an empty 404, so the browser either shows its own error page or nothing at all.

////////////////////

Right now events live in a List in the code, so any change disappears when the server stops. We replace that with a PostgreSQL database, which stores data on disk so it sticks around.

dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design

Entity Framework Core (EF Core) is the .NET library that lets you work with the database using C# objects instead of writing raw SQL for everything. It's the same idea as Mongoose in NurseAssist, but for SQL databases. (That kind of tool is called an ORM)[Object-Relational Mapping (ORM) is a programming technique that connects object-oriented application code to relational databases by translating code objects into database table rows and columns.]
The first package teaches EF Core how to talk specifically to PostgreSQL.
The second package adds the design-time tools EF Core needs to create your tables from your C# classes.

/////////////

public class PriceAlert
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event? Event { get; set; }
    public string Email { get; set; } = "";
    public decimal TargetPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsTriggered { get; set; }
}
EventId is a foreign key: a column that points to a row in another table. It records which event this alert is watching. In MongoDB you may have stored a reference ID loosely. A SQL database enforces it, so it will refuse to save an alert for an event that doesn't exist.
Event? Event is a "navigation property." It lets your C# code jump from an alert to its full event (alert.Event.Name). EF Core sees EventId and Event together and works out the relationship on its own. The ? means it's allowed to be empty.
IsTriggered will track whether the price has dropped below the target. We'll use it next time.

Adding endpoints; whats going on in Program.cs:

MapPost: GET requests read data and POST requests create data. It's the same split as app.get() and app.post() in Express.
CreateAlertRequest describes the data a user is allowed to send. It has only three fields, on purpose. If the endpoint accepted a full PriceAlert, a user could set Id or IsTriggered themselves. A small class like this, which controls what comes in, is called a DTO (data transfer object).
The two if checks are validation. Never trust incoming data. A bad price gets a 400 Bad Request ("you sent something invalid"), and a nonexistent event gets a 404.
Add then SaveChangesAsync: Add only queues the new row in memory. Nothing reaches the database until SaveChangesAsync runs. It works like git add and git commit.
Results.Created returns status 201 Created, the correct code for "your new thing was saved," along with the address of the new alert and the alert itself.

/////////

Your events are fake, so their prices never change on their own. The job will therefore do two things each time it runs: nudge every price up or down a little to imitate a live market, then check whether any alerts have been hit.

New file, api/PriceCheckService.cs:

What each piece does:

BackgroundService is a built-in .NET class for code that runs alongside your API for as long as the server is up. Your endpoints only run when someone sends a request. This runs on its own.
PeriodicTimer and the while loop: the loop waits 30 seconds, runs the code inside, then waits again, until the server shuts down.
stoppingToken is how .NET tells the service "the server is shutting down, stop now." Passing it into each database call lets them cancel cleanly, so they don't hang.
scopeFactory.CreateScope(): in your endpoints, ASP.NET hands you a fresh database context for each request and throws it away afterwards. A background service has no requests, so it creates its own short-lived "scope" on each pass to get a fresh context. This lifetime mismatch between long-lived services and per-request ones is a common .NET interview topic.
Random.Shared.NextDouble() * 0.2 - 0.1: NextDouble() gives a random number from 0 to 1. Multiplying by 0.2 and subtracting 0.1 turns it into a number from -0.1 to +0.1, which is -10% to +10%.
The first SaveChangesAsync writes the new prices to the database before the alert check. The check runs in the database, so it would otherwise see the old prices.
Include(a => a.Event) tells EF Core to load each alert's event along with it. This is the "only loads related data when you ask" behavior from last time, and here we're asking. In SQL terms, it does a JOIN between the two tables.
Where(...) filters to alerts that haven't fired yet and whose event price is at or below the target. EF Core turns this into a SQL WHERE clause, so the database does the filtering.
logger.LogInformation prints a message to the terminal where your server is running. For now, that log line is the alert. Sending a real email can come later.

/// START OF NEXT.JS

what's going on in web/app/page.tsx:

type TicketEvent describes the shape of the data coming from your API. If you mistype a field name, like ev.lowestPrce, VS Code underlines it immediately. I named it TicketEvent because browsers already have a built-in type called Event.
The field names are lowercase (lowestPrice, not LowestPrice) because ASP.NET automatically converts C# property names to JavaScript's naming style when it sends JSON.
async function Home() is the big difference from NurseAssist. There, your React components ran in the browser and used useEffect to fetch data after the page loaded. This one is a server component: it runs on the Next.js server, fetches the data there, and sends the browser a finished page. That means less code and no loading flicker.
cache: "no-store" tells Next.js to fetch fresh data on every visit and never reuse an old copy. Your prices change every 30 seconds, so you want the latest.
process.env.API_URL reads the address from your .env.local file.
events.map(...) with key is the same list rendering you did in React before.
toFixed(2) always shows two decimal places, so $89.5 displays as $89.50.
timeZone: "UTC" displays the date as stored. Without it, the browser converts to your local time zone, which can shift the date back a day.
The className values are Tailwind. For example, p-8 is padding, font-bold is bold text, and flex justify-between pushes the name and the price to opposite sides.

whats going on in AlertForm.tsx:

"use client" on the first line marks this as a client component, meaning it runs in the browser. Anything interactive (typing, clicking, useState) has to run there. The page stays a server component and this form sits inside it. Knowing when to use each kind is a common Next.js interview question.
{ eventId }: { eventId: number } is how TypeScript describes a component's props. This form takes one prop, the ID of the event it belongs to.
useState works exactly as it did in NurseAssist. Each input's value lives in state and updates as the user types.
e.preventDefault() stops the browser's default form behavior, which is to reload the whole page. You want to send the data yourself with fetch.
fetch with method: "POST" is the same request you sent from PowerShell with Invoke-RestMethod, now sent by the browser. JSON.stringify turns your JavaScript object into JSON text.
Number(targetPrice): input boxes always give you text, even when someone types digits. This API expects a number, so this converts it.
res.ok is true for success codes like your 201 Created and false for errors like 400 or 404.
required, type="email", min="1" make the browser check the input before sending. The API still validates too. Browser checks are a convenience for the user, and server checks are the real protection, because anyone can skip the website and call the API directly.

I hit a CORS error, learned why browsers block cross-site requests, and fixed it with a named-origin policy
CORS (Cross-Origin Resource Sharing) is the way a server grants that permission.