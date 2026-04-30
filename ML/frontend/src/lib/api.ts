import { TripInput, TripResult } from '@/types';

const BACKEND_URL = process.env.NEXT_PUBLIC_BACKEND_URL || 'http://localhost:5000';

export async function generateTrip(input: TripInput): Promise<TripResult> {
  const response = await fetch(`${BACKEND_URL}/api/trip`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(input),
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(`Failed to generate trip: ${error}`);
  }

  return response.json();
}
