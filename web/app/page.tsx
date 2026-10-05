import AlertForm from "./AlertForm";

type TicketEvent = {
  id: number;
  name: string;
  venue: string;
  date: string;
  lowestPrice: number;
};

export default async function Home() {
  const res = await fetch(`${process.env.API_URL}/events`, { cache: "no-store" });
  const events: TicketEvent[] = await res.json();

  return (
    <main className="mx-auto max-w-2xl p-8">
      <h1 className="text-3xl font-bold">SeatWatch</h1>
      <p className="mt-1 text-gray-500">
        Track ticket prices and get alerted when they drop.
      </p>

      <ul className="mt-6 space-y-3">
       {events.map((ev) => (
         <li key={ev.id} className="rounded-lg border p-4">
           <div className="flex items-center justify-between">
             <div>
              <p className="font-semibold">{ev.name}</p>
              <p className="text-sm text-gray-500">
               {ev.venue} · {new Date(ev.date).toLocaleDateString("en-US", { timeZone: "UTC" })}
              </p>
            </div>
            <p className="text-xl font-bold">${ev.lowestPrice.toFixed(2)}</p>
          </div>
          <AlertForm eventId={ev.id} />
        </li>
      ))}
      </ul>
    </main>
  );
}