'use client';

import { motion } from 'framer-motion';
import { Calendar, Users, Wallet, Zap } from 'lucide-react';

interface ResultsHeroProps {
  destinations: string[];
  startDate: string;
  endDate: string;
  groupSize: number;
  budget: string;
  tripPace: string;
}

export function ResultsHero({
  destinations,
  startDate,
  endDate,
  groupSize,
  budget,
  tripPace,
}: ResultsHeroProps) {
  const heroImage = 'https://images.unsplash.com/photo-1572252009286-268acec5ca0a?w=1920&q=80';

  const formatDate = (dateStr: string) => {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  };

  return (
    <div className="relative h-80 md:h-96 w-full overflow-hidden">
      <div className="absolute inset-0 bg-charcoal/50 z-10" />
      <div className="absolute inset-0 bg-gradient-to-t from-charcoal via-charcoal/60 to-transparent z-20" />

      <img
        src={heroImage}
        alt="Egypt"
        className="w-full h-full object-cover"
      />

      <div className="relative z-30 flex flex-col justify-end h-full px-4 md:px-8 pb-8 max-w-6xl mx-auto">
        <motion.h1
          className="text-4xl md:text-5xl font-bold mb-4"
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2 }}
        >
          <span className="gold-gradient-text">
            Trip Plan: {destinations.join(' & ')}
          </span>
        </motion.h1>

        <motion.div
          className="flex flex-wrap gap-3"
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.4 }}
        >
          <span className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-charcoal/60 backdrop-blur-sm text-sm">
            <Calendar className="w-4 h-4 text-amber" />
            {formatDate(startDate)} - {formatDate(endDate)}
          </span>
          <span className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-charcoal/60 backdrop-blur-sm text-sm">
            <Users className="w-4 h-4 text-amber" />
            {groupSize} {groupSize === 1 ? 'person' : 'people'}
          </span>
          <span className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-charcoal/60 backdrop-blur-sm text-sm">
            <Wallet className="w-4 h-4 text-amber" />
            {budget}
          </span>
          <span className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-charcoal/60 backdrop-blur-sm text-sm">
            <Zap className="w-4 h-4 text-amber" />
            {tripPace} pace
          </span>
        </motion.div>
      </div>
    </div>
  );
}
