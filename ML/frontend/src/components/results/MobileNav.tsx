'use client';

import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { MapPin, Utensils, Bed, Calendar, PieChart } from 'lucide-react';
import { cn } from '@/lib/utils';

const tabs = [
  { id: 'places', label: 'Places', icon: MapPin },
  { id: 'restaurants', label: 'Food', icon: Utensils },
  { id: 'hotels', label: 'Hotels', icon: Bed },
  { id: 'itinerary', label: 'Plan', icon: Calendar },
  { id: 'budget', label: 'Budget', icon: PieChart },
];

interface MobileNavProps {
  activeSection: string;
  onNavigate: (section: string) => void;
}

export function MobileNav({ activeSection, onNavigate }: MobileNavProps) {
  return (
    <motion.div
      className="fixed bottom-16 left-0 right-0 z-40 bg-charcoal/95 backdrop-blur-lg border-t border-border md:hidden no-print"
      initial={{ y: 100 }}
      animate={{ y: 0 }}
      transition={{ delay: 0.5 }}
    >
      <div className="flex items-center justify-around py-2">
        {tabs.map((tab) => {
          const Icon = tab.icon;
          const isActive = activeSection === tab.id;
          return (
            <button
              key={tab.id}
              onClick={() => onNavigate(tab.id)}
              className={cn(
                'flex flex-col items-center gap-1 px-3 py-2 rounded-lg transition-colors',
                isActive ? 'text-amber' : 'text-offwhite-muted'
              )}
            >
              <Icon className="w-5 h-5" />
              <span className="text-xs">{tab.label}</span>
            </button>
          );
        })}
      </div>
    </motion.div>
  );
}
