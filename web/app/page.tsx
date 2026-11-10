import AlertForm from "./AlertForm";

type TicketEvent = {
  id: number;
  name: string;
  venue: string;
  date: string;
  lowestPrice: number;
};

async function getEvents(): Promise<TicketEvent[] | null> {
  try {
    const res = await fetch(`${process.env.API_URL}/events`, { cache: "no-store" });
    if (!res.ok) return null;
    return await res.json();
  } catch {
    return null;
  }
}

export default async function Home() {
  const events = await getEvents();

  return (
    <main className="mx-auto max-w-2xl p-8">
      <h1 className="text-3xl font-bold">SeatWatch</h1>
      <p className="mt-1 text-gray-500">
        Track ticket prices and get alerted when they drop.
      </p>

      {events === null ? (
        <p className="mt-6 rounded-lg border border-red-200 bg-red-50 p-4 text-red-700">
          Prices are unavailable right now. Please try again in a minute.
        </p>
      ) : (
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
      )}
    </main>
  );
}