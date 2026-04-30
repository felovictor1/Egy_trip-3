'use client';

import { useEffect, useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

const loadingMessages = [
  'Finding the best places...',
  'Crafting your itinerary...',
  'Checking prices...',
  'Adding local tips...',
  'Curating restaurants...',
  'Finding the best hotels...',
  'Building your budget plan...',
];

interface LoadingStateProps {
  tripSummary?: {
    destinations: string[];
    dates: string;
    groupSize: number;
    budget: string;
  };
}

export function LoadingState({ tripSummary }: LoadingStateProps) {
  const [messageIndex, setMessageIndex] = useState(0);

  useEffect(() => {
    const interval = setInterval(() => {
      setMessageIndex((prev) => (prev + 1) % loadingMessages.length);
    }, 2000);
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="min-h-screen bg-charcoal flex flex-col items-center justify-center px-4">
      {/* Egyptian Pyramid Animation */}
      <div className="relative mb-12">
        <svg
          width="200"
          height="200"
          viewBox="0 0 200 200"
          className="animate-float"
        >
          {/* Sun */}
          <motion.circle
            cx="100"
            cy="40"
            r="20"
            fill="#D4A853"
            initial={{ opacity: 0, scale: 0 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ duration: 1, delay: 0.5 }}
          />

          {/* Pyramids */}
          <motion.polygon
            points="30,160 100,60 170,160"
            fill="none"
            stroke="#D4A853"
            strokeWidth="2"
            initial={{ pathLength: 0, opacity: 0 }}
            animate={{ pathLength: 1, opacity: 1 }}
            transition={{ duration: 2, delay: 0.5 }}
            style={{ pathLength: 1 }}
          />
          <motion.polygon
            points="80,160 130,90 180,160"
            fill="none"
            stroke="#D4A853"
            strokeWidth="1.5"
            initial={{ pathLength: 0, opacity: 0 }}
            animate={{ pathLength: 1, opacity: 0.6 }}
            transition={{ duration: 2, delay: 1 }}
          />

          {/* Ground line */}
          <motion.line
            x1="10"
            y1="160"
            x2="190"
            y2="160"
            stroke="#D4A853"
            strokeWidth="1"
            initial={{ pathLength: 0 }}
            animate={{ pathLength: 1 }}
            transition={{ duration: 1, delay: 0.2 }}
          />

          {/* Stars */}
          {[...Array(8)].map((_, i) => (
            <motion.circle
              key={i}
              cx={20 + Math.random() * 160}
              cy={10 + Math.random() * 50}
              r="1"
              fill="#D4A853"
              initial={{ opacity: 0 }}
              animate={{ opacity: [0, 1, 0] }}
              transition={{
                duration: 2 + Math.random() * 2,
                repeat: Infinity,
                delay: Math.random() * 2,
              }}
            />
          ))}
        </svg>
      </div>

      {/* Loading Message */}
      <div className="h-8 mb-8">
        <AnimatePresence mode="wait">
          <motion.p
            key={messageIndex}
            className="text-xl text-offwhite text-center"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.3 }}
          >
            {loadingMessages[messageIndex]}
          </motion.p>
        </AnimatePresence>
      </div>

      {/* Progress Dots */}
      <div className="flex gap-2 mb-12">
        {loadingMessages.map((_, i) => (
          <motion.div
            key={i}
            className="w-2 h-2 rounded-full"
            animate={{
              backgroundColor: i <= messageIndex ? '#D4A853' : '#2A2A2A',
              scale: i === messageIndex ? 1.3 : 1,
            }}
            transition={{ duration: 0.3 }}
          />
        ))}
      </div>

      {/* Trip Summary Card */}
      {tripSummary && (
        <motion.div
          className="glass-card p-6 max-w-sm w-full"
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.5 }}
        >
          <h3 className="text-lg font-bold text-amber mb-4">Your Trip</h3>
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-offwhite-muted">Destinations</span>
              <span className="text-offwhite">{tripSummary.destinations.join(', ')}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-offwhite-muted">Dates</span>
              <span className="text-offwhite">{tripSummary.dates}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-offwhite-muted">Group</span>
              <span className="text-offwhite">{tripSummary.groupSize} people</span>
            </div>
            <div className="flex justify-between">
              <span className="text-offwhite-muted">Budget</span>
              <span className="text-amber">{tripSummary.budget}</span>
            </div>
          </div>
        </motion.div>
      )}
    </div>
  );
}
