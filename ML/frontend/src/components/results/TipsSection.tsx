'use client';

import { motion } from 'framer-motion';
import {
  Lightbulb,
  Shield,
  CreditCard,
  Camera,
  Clock,
  Droplets,
  Shirt,
  Languages,
  AlertTriangle,
  Heart,
} from 'lucide-react';

interface TipsSectionProps {
  tips: string[];
}

const tipIcons = [Lightbulb, Shield, CreditCard, Camera, Clock, Droplets, Shirt, Languages, AlertTriangle, Heart];

const tipColors = [
  'bg-amber/10 border-amber/20',
  'bg-blue-500/10 border-blue-500/20',
  'bg-green-500/10 border-green-500/20',
  'bg-purple-500/10 border-purple-500/20',
  'bg-orange-500/10 border-orange-500/20',
  'bg-cyan-500/10 border-cyan-500/20',
  'bg-pink-500/10 border-pink-500/20',
  'bg-indigo-500/10 border-indigo-500/20',
  'bg-red-500/10 border-red-500/20',
  'bg-rose-500/10 border-rose-500/20',
];

export function TipsSection({ tips }: TipsSectionProps) {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      {tips.map((tip, index) => {
        const Icon = tipIcons[index % tipIcons.length];
        return (
          <motion.div
            key={index}
            className={`glass-card p-5 border ${tipColors[index % tipColors.length]}`}
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: index * 0.05 }}
          >
            <div className="flex items-start gap-3">
              <div className="p-2 rounded-lg bg-charcoal-lighter">
                <Icon className="w-5 h-5 text-amber" />
              </div>
              <p className="text-sm text-offwhite-muted leading-relaxed">{tip}</p>
            </div>
          </motion.div>
        );
      })}
    </div>
  );
}
