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
