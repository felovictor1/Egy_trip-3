'use client';

import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Sun,
  Utensils,
  Sunset,
  Moon,
  ChevronDown,
  ChevronUp,
  Clock,
  Ticket,
} from 'lucide-react';
import { DayItinerary } from '@/types';

interface ItineraryTimelineProps {
  days: DayItinerary[];
}

const blockIcons: Record<string, React.ElementType> = {
  morning: Sun,
  lunch: Utensils,
  afternoon: Sunset,
  dinner: Moon,
};

const blockLabels: Record<string, string> = {
  morning: 'Morning',
  lunch: 'Lunch',
  afternoon: 'Afternoon',
  dinner: 'Dinner',
};

export function ItineraryTimeline({ days }: ItineraryTimelineProps) {
  const [openDays, setOpenDays] = useState<Set<number>>(new Set([1]));

  const toggleDay = (day: number) => {
    const next = new Set(openDays);
    if (next.has(day)) {
      next.delete(day);
    } else {
      next.add(day);
    }
    setOpenDays(next);
  };

  return (
    <div className="space-y-4">
      {days.map((day, index) => (
        <motion.div
          key={day.day}
          className="glass-card overflow-hidden"
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          transition={{ delay: index * 0.1 }}
        >
          <button
            onClick={() => toggleDay(day.day)}
            className="w-full flex items-center justify-between p-4 md:p-6 cursor-pointer hover:bg-charcoal-lighter/50 transition-colors"
          >
            <div className="flex items-center gap-4">
              <div className="w-12 h-12 rounded-xl bg-amber/10 flex items-center justify-center">
                <span className="text-xl font-bold text-amber">{day.day}</span>
              </div>
              <div className="text-left">
                <h3 className="font-bold text-lg">Day {day.day}</h3>
                {day.date && (
                  <p className="text-sm text-offwhite-muted">{day.date}</p>
                )}
              </div>
            </div>
            <div className="flex items-center gap-3">
              {day.dayCost && (
                <span className="px-3 py-1 rounded-full bg-amber/10 text-amber text-sm font-medium">
                  ~{day.dayCost} EGP/day
                </span>
              )}
              {openDays.has(day.day) ? (
                <ChevronUp className="w-5 h-5 text-offwhite-muted" />
              ) : (
                <ChevronDown className="w-5 h-5 text-offwhite-muted" />
              )}
            </div>
          </button>

          <AnimatePresence>
            {openDays.has(day.day) && (
              <motion.div
                initial={{ height: 0, opacity: 0 }}
                animate={{ height: 'auto', opacity: 1 }}
                exit={{ height: 0, opacity: 0 }}
                transition={{ duration: 0.3 }}
              >
                <div className="px-4 md:px-6 pb-6 space-y-4">
                  {(['morning', 'lunch', 'afternoon', 'dinner'] as const).map(
                    (block) => {
                      const activity = day[block];
                      if (!activity) return null;
                      const Icon = blockIcons[block];

                      return (
                        <div key={block} className="flex gap-4">
                          <div className="flex flex-col items-center">
                            <div className="w-10 h-10 rounded-full bg-charcoal-lighter flex items-center justify-center">
                              <Icon className="w-5 h-5 text-amber" />
                            </div>
                            <div className="w-0.5 h-full bg-border mt-2" />
                          </div>

                          <div className="flex-1 pb-4">
                            <div className="flex items-center gap-2 mb-1">
                              <span className="text-xs font-medium text-amber uppercase tracking-wider">
                                {blockLabels[block]}
                              </span>
                              {activity.time && (
                                <span className="inline-flex items-center gap-1 text-xs text-offwhite-muted">
                                  <Clock className="w-3 h-3" />
                                  {activity.time}
                                </span>
                              )}
                            </div>

                            <h4 className="font-semibold text-offwhite">
                              {activity.place}
                            </h4>
                            <p className="text-sm text-offwhite-muted mt-1">
                              {activity.description}
                            </p>

                            {activity.price && (
                              <span className="inline-flex items-center gap-1 mt-2 text-xs text-amber">
                                <Ticket className="w-3 h-3" />
                                {activity.price} EGP
                              </span>
                            )}
                          </div>
                        </div>
                      );
                    }
                  )}
                </div>
              </motion.div>
            )}
          </AnimatePresence>
        </motion.div>
      ))}
    </div>
  );
}
