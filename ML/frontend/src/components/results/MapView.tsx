'use client';

import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { MapPin, ExternalLink } from 'lucide-react';
import { Place, Restaurant, Hotel } from '@/types';

interface MapViewProps {
  places: Place[];
  restaurants: Restaurant[];
  hotels: Hotel[];
}

export function MapView({ places, restaurants, hotels }: MapViewProps) {
  const [activeTab, setActiveTab] = useState<'places' | 'restaurants' | 'hotels'>('places');

  const allLocations = [
    ...places.map((p) => ({ ...p, type: 'place' as const })),
    ...restaurants.map((r) => ({ ...r, type: 'restaurant' as const })),
    ...hotels.map((h) => ({ ...h, type: 'hotel' as const })),
  ];

  const filteredLocations = allLocations.filter((loc) => {
    if (activeTab === 'places') return 'ticketPrice' in loc;
    if (activeTab === 'restaurants') return 'cuisines' in loc;
    return 'price' in loc;
  });

  return (
    <motion.div
      className="glass-card p-6"
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-2xl font-bold flex items-center gap-2">
          <MapPin className="w-6 h-6 text-amber" />
          Map View
        </h2>
      </div>

      <div className="flex gap-2 mb-4">
        {(['places', 'restaurants', 'hotels'] as const).map((tab) => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
              activeTab === tab
                ? 'bg-amber/20 text-amber border border-amber/40'
                : 'bg-charcoal-lighter text-offwhite-muted hover:text-offwhite'
            }`}
          >
            {tab.charAt(0).toUpperCase() + tab.slice(1)}
          </button>
        ))}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 max-h-96 overflow-y-auto pr-2">
        {filteredLocations.map((loc, index) => (
          <motion.div
            key={loc.name}
            className="flex items-center gap-3 p-3 rounded-lg bg-charcoal-lighter hover:bg-charcoal-lighter/80 transition-colors"
            initial={{ opacity: 0, x: -10 }}
            whileInView={{ opacity: 1, x: 0 }}
            viewport={{ once: true }}
            transition={{ delay: index * 0.05 }}
          >
            <MapPin className="w-5 h-5 text-amber flex-shrink-0" />
            <div className="flex-1 min-w-0">
              <p className="font-medium text-sm truncate">{loc.name}</p>
              {'address' in loc && loc.address && (
                <p className="text-xs text-offwhite-muted truncate">{loc.address}</p>
              )}
            </div>
            <a
              href={`https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(loc.name + ' Egypt')}`}
              target="_blank"
              rel="noopener noreferrer"
              className="p-2 rounded-lg hover:bg-charcoal-light transition-colors"
            >
              <ExternalLink className="w-4 h-4 text-amber" />
            </a>
          </motion.div>
        ))}
      </div>

      <div className="mt-4 p-4 rounded-lg bg-charcoal-lighter">
        <iframe
          width="100%"
          height="300"
          style={{ border: 0, borderRadius: '8px' }}
          loading="lazy"
          allowFullScreen
          referrerPolicy="no-referrer-when-downgrade"
          src={`https://www.google.com/maps/embed/v1/place?key=AIzaSyBFw0Qbyq9zTFTd-tUY6dZWTgaQzuU17R8&q=Egypt&zoom=6`}
        />
        <p className="text-xs text-offwhite-muted mt-2 text-center">
          Replace the API key with your own Google Maps API key for full functionality
        </p>
      </div>
    </motion.div>
  );
}
