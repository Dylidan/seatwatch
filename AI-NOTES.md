# How I used AI on this project

I built SeatWatch with Claude as a pair programmer and tutor. I had never
used ASP.NET Core, PostgreSQL or Next.js before this project.

## How I worked
- Claude proposed the design and wrote the first version of most of the code.
- I asked for an explanation of every step, then ran, tested and debugged
  everything myself.
- I tested each endpoint by hand, including the failure cases (404 for a
  missing event, 400 for an invalid price).

## Things I had to work through
- **A scary-looking "fail" on the first database update.** EF Core checks a
  migrations history table that doesn't exist yet on a new database. I
  learned to read the whole output before assuming something broke.
- **A CORS error when the form called the API.** I learned why browsers
  block cross-origin requests and fixed it with a policy that names only
  my frontend's origin.
- **Sending alert emails through a third-party API.** I added email
  notifications using Resend's REST API. I stored the API key in .NET user
  secrets so it never reaches GitHub, and wrapped the call in try/catch so
  an outage at the email provider can't crash my app.
- **The whole API crashed because the database was off.** After a break
  from the project, the site showed a JSON parse error. The website's error
  only showed the symptom. The API's log showed the real cause: PostgreSQL
  wasn't running ("connection actively refused" on port 5432). I started
  the Windows service and set it to start automatically. I also learned
  that an unhandled exception in a .NET BackgroundService stops the entire
  app by default.
- **Alerts fired but no email arrived.** The terminal showed the alert, but
  the Resend dashboard showed nothing was sent. My code only logged
  failures, so a send that never happened looked the same as one that
  worked. I added a log line for successful sends, which surfaced the real
  error: EmailService was never registered for dependency injection. I
  found it by reading the stack trace down to the line in my own code.

