"use client";

import { useState, type FormEvent } from "react";

export default function AlertForm({ eventId }: { eventId: number }) {
  const [email, setEmail] = useState("");
  const [targetPrice, setTargetPrice] = useState("");
  const [message, setMessage] = useState("");

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();

    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/alerts`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ eventId, email, targetPrice: Number(targetPrice) }),
    });

    setMessage(res.ok ? "Alert saved!" : "Something went wrong. Check the price and try again.");
  }

  return (
    <form onSubmit={handleSubmit} className="mt-3 flex flex-wrap items-center gap-2">
      <input
        type="email"
        required
        placeholder="you@example.com"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        className="rounded border px-2 py-1 text-sm"
      />
      <input
        type="number"
        required
        min="1"
        step="0.01"
        placeholder="Target price"
        value={targetPrice}
        onChange={(e) => setTargetPrice(e.target.value)}
        className="w-32 rounded border px-2 py-1 text-sm"
      />
      <button type="submit" className="rounded bg-blue-600 px-3 py-1 text-sm text-white">
        Alert me
      </button>
      {message && <p className="w-full text-sm text-gray-600">{message}</p>}
    </form>
  );
}