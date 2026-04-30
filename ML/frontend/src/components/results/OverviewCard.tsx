'use client';

import { motion } from 'framer-motion';
import { MapPin } from 'lucide-react';

interface OverviewCardProps {
  description: string;
}

export function OverviewCard({ description }: OverviewCardProps) {
  return (
    <motion.div
      className="glass-card p-6 md:p-8 border-l-4 border-l-amber"
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      <div className="flex items-start gap-4">
        <MapPin className="w-6 h-6 text-amber flex-shrink-0 mt-1" />
        <div>
          <h2 className="text-2xl font-bold mb-3">Overview</h2>
          <p className="text-offwhite-muted leading-relaxed whitespace-pre-wrap">
            {description}
          </p>
        </div>
      </div>
    </motion.div>
  );
}
