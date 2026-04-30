'use client';

import { motion } from 'framer-motion';
import { Landmark, UtensilsCrossed, Mountain, Palmtree, ShoppingBag, Waves, Users, GlassWater } from 'lucide-react';
import { useTripStore } from '@/store/useTripStore';
import { Chip } from '@/components/ui/Chip';
import { Button } from '@/components/ui/Button';
import { cn } from '@/lib/utils';

const travelStyles = [
  { id: 'Historical', label: 'Historical', icon: Landmark },
  { id: 'Food & Dining', label: 'Food & Dining', icon: UtensilsCrossed },
  { id: 'Adventure', label: 'Adventure', icon: Mountain },
  { id: 'Relaxation', label: 'Relaxation', icon: Palmtree },
  { id: 'Shopping', label: 'Shopping', icon: ShoppingBag },
  { id: 'Nature & Wildlife', label: 'Nature & Wildlife', icon: Waves },
  { id: 'Cultural', label: 'Cultural', icon: Users },
  { id: 'Nightlife', label: 'Nightlife', icon: GlassWater },
];

const timePeriods = [
  { label: 'Pharaonic', emoji: '🏛️' },
  { label: 'Islamic', emoji: '🕌' },
  { label: 'Coptic', emoji: '⛪' },
  { label: 'Roman', emoji: '🏛️' },
  { label: 'Modern', emoji: '🌆' },
];

interface Step2Props {
  onNext: () => void;
  onBack: () => void;
}

export function Step2({ onNext, onBack }: Step2Props) {
  const { input, setInput } = useTripStore();

  const toggleTravelStyle = (style: string) => {
    const current = input.travelStyles || [];
    const updated = current.includes(style)
      ? current.filter((s) => s !== style)
      : [...current, style];
    setInput({ travelStyles: updated });
  };

  const toggleTimePeriod = (period: string) => {
    const current = input.preferredTimePeriods || [];
    const updated = current.includes(period)
      ? current.filter((p) => p !== period)
      : [...current, period];
    setInput({ preferredTimePeriods: updated });
  };

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-2xl font-bold mb-2">Your Travel Style</h2>
        <p className="text-offwhite-muted mb-4">What kind of experiences are you looking for?</p>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {travelStyles.map((style) => {
            const Icon = style.icon;
            const isSelected = (input.travelStyles || []).includes(style.id);
            return (
              <motion.div
                key={style.id}
                whileHover={{ scale: 1.03 }}
                whileTap={{ scale: 0.97 }}
                onClick={() => toggleTravelStyle(style.id)}
                className={cn(
                  'p-4 rounded-xl border cursor-pointer transition-all duration-200 text-center',
                  isSelected
                    ? 'border-amber bg-amber/10'
                    : 'border-border bg-charcoal-light hover:border-amber/30'
                )}
              >
                <Icon className={cn(
                  'w-8 h-8 mx-auto mb-2',
                  isSelected ? 'text-amber' : 'text-offwhite-muted'
                )} />
                <span className="text-sm font-medium">{style.label}</span>
              </motion.div>
            );
          })}
        </div>
      </div>

      <div>
        <h2 className="text-2xl font-bold mb-2">Historical Knowledge</h2>
        <p className="text-offwhite-muted mb-4">How familiar are you with Egyptian history?</p>
        <div className="flex gap-3">
          {['Beginner', 'Intermediate', 'Expert'].map((level) => (
            <Chip
              key={level}
              label={level}
              selected={input.historicalKnowledge === level}
              onClick={() => setInput({ historicalKnowledge: level })}
            />
          ))}
        </div>
      </div>

      <div>
        <h2 className="text-2xl font-bold mb-2">Preferred Time Periods</h2>
        <p className="text-offwhite-muted mb-4">Which eras interest you most?</p>
        <div className="flex flex-wrap gap-3">
          {timePeriods.map((period) => (
            <Chip
              key={period.label}
              label={`${period.emoji} ${period.label}`}
              selected={(input.preferredTimePeriods || []).includes(period.label)}
              onClick={() => toggleTimePeriod(period.label)}
            />
          ))}
        </div>
      </div>

      <div className="flex gap-4 justify-between">
        <Button variant="secondary" onClick={onBack} size="lg">
          Back
        </Button>
        <Button onClick={onNext} size="lg">
          Next: Logistics
        </Button>
      </div>
    </div>
  );
}
