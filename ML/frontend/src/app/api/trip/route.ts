import type { NextRequest, NextResponse } from 'next/server';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();

    const response = await fetch(`${process.env.BACKEND_URL || 'http://localhost:5000'}/api/trip`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(body),
    });

    if (!response.ok) {
      const errorText = await response.text();
      return new Response(errorText, {
        status: response.status,
        statusText: response.statusText,
      });
    }

    const data = await response.json();
    return Response.json(data);
  } catch (error: any) {
    return new Response(
      JSON.stringify({ error: 'Failed to connect to backend', details: error.message }),
      { status: 500 }
    );
  }
}
