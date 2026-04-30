'use client';

import { useState } from 'react';
import { motion } from 'framer-motion';
import { Bed, Car, Utensils, Zap, MapPin } from 'lucide-react';
import { useTripStore } from '@/store/useTripStore';
import { Chip } from '@/components/ui/Chip';
import { Button } from '@/components/ui/Button';
import { cn } from '@/lib/utils';

const accommodationTypes = [
  { id: 'Budget', label: 'Budget', icon: '💰', desc: 'Affordable stays' },
  { id: 'Medium', label: 'Mid-Range', icon: '🏨', desc: 'Comfortable & balanced' },
  { id: 'Luxury', label: 'Luxury', icon: '✨', desc: 'Premium experiences' },
];

const transportOptions = [
  { id: 'Private Car', label: 'Private Car', icon: Car },
  { id: 'Public Transport', label: 'Public Transport', icon: '🚌' as any },
  { id: 'Walking', label: 'Walking', icon: '🚶' as any },
  { id: 'Taxi/Rideshare', label: 'Taxi/Rideshare', icon: '🚕' as any },
];

const foodOptions = ['Vegetarian', 'Non-Vegetarian', 'Vegan', 'Halal', 'No Preference'];

const paceOptions = [
  { id: 'Relaxed', label: 'Relaxed', desc: '2-3 activities/day', icon: '🌅' },
  { id: 'Moderate', label: 'Moderate', desc: '4-5 activities/day', icon: '⚡' },
  { id: 'Fast', label: 'Packed', desc: '6+ activities/day', icon: '🚀' },
];

interface Step3Props {
  onBack: () => void;
  onGenerate: () => void;
}

export function Step3({ onBack, onGenerate }: Step3Props) {
  const { input, setInput } = useTripStore();
  const [mustVisitTags, setMustVisitTags] = useState<string[]>(
    (input.mustVisit || '').split(',').map((s) => s.trim()).filter(Boolean)
  );
  const [mustVisitInput, setMustVisitInput] = useState('');

  const addMustVisitTag = () => {
    if (mustVisitInput.trim()) {
      const newTags = [...mustVisitTags, mustVisitInput.trim()];
      setMustVisitTags(newTags);
      setInput({ mustVisit: newTags.join(', ') });
      setMustVisitInput('');
    }
  };

  const removeMustVisitTag = (tag: string) => {
    const newTags = mustVisitTags.filter((t) => t !== tag);
    setMustVisitTags(newTags);
    setInput({ mustVisit: newTags.join(', ') });
  };

  return (
    <div className="space-y-8">
      <div>
        <h2 className="text-2xl font-bold mb-2">Accommodation</h2>
        <p className="text-offwhite-muted mb-4">What type of stay do you prefer?</p>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          {accommodationTypes.map((type) => (
            <motion.div
              key={type.id}
              whileHover={{ scale: 1.03 }}
              whileTap={{ scale: 0.97 }}
              onClick={() => setInput({ accommodationType: type.id })}
              className={cn(
                'p-4 rounded-xl border cursor-pointer transition-all duration-200 text-center',
                input.accommodationType === type.id
                  ? 'border-amber bg-amber/10'
                  : 'border-border bg-charcoal-light hover:border-amber/30'
              )}
            >
              <span className="text-2xl">{type.icon}</span>
              <h3 className="font-semibold mt-2">{type.label}</h3>
              <p className="text-xs text-offwhite-muted mt-1">{type.desc}</p>
            </motion.div>
          ))}
        </div>
      </div>

      <div>
        <h2 className="text-2xl font-bold mb-2">Transportation</h2>
        <p className="text-offwhite-muted mb-4">How do you want to get around?</p>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
          {transportOptions.map((option) => (
            <motion.div
              key={option.id}
              whileHover={{ scale: 1.03 }}
              whileTap={{ scale: 0.97 }}
              onClick={() => setInput({ transportation: option.id })}
              className={cn(
                'p-4 rounded-xl border cursor-pointer transition-all duration-200 text-center',
                input.transportation === option.id
                  ? 'border-amber bg-amber/10'
                  : 'border-border bg-charcoal-light hover:border-amber/30'
              )}
            >
              <span className="text-2xl">{typeof option.icon === 'string' ? option.icon : <option.icon className="w-8 h-8 mx-auto" />}</span>
              <p className="text-sm font-medium mt-2">{option.label}</p>
            </motion.div>
          ))}
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div>
          <h2 className="text-2xl font-bold mb-2 flex items-center gap-2">
            <Utensils className="w-6 h-6 text-amber" />
            Food Preferences
          </h2>
          <div className="flex flex-wrap gap-2 mt-4">
            {foodOptions.map((food) => (
              <Chip
                key={food}
                label={food}
                selected={input.foodPreferences === food}
                onClick={() => setInput({ foodPreferences: food })}
              />
            ))}
          </div>
        </div>

        <div>
          <h2 className="text-2xl font-bold mb-2 flex items-center gap-2">
            <Zap className="w-6 h-6 text-amber" />
            Trip Pace
          </h2>
          <div className="flex flex-col sm:flex-row gap-3 mt-4">
            {paceOptions.map((pace) => (
              <motion.div
                key={pace.id}
                whileHover={{ scale: 1.03 }}
                whileTap={{ scale: 0.97 }}
                onClick={() => setInput({ tripPace: pace.id })}
                className={cn(
                  'flex-1 p-3 rounded-xl border cursor-pointer transition-all duration-200 text-center',
                  input.tripPace === pace.id
                    ? 'border-amber bg-amber/10'
                    : 'border-border bg-charcoal-light hover:border-amber/30'
                )}
              >
                <span className="text-xl">{pace.icon}</span>
                <p className="text-sm font-medium mt-1">{pace.label}</p>
                <p className="text-xs text-offwhite-muted">{pace.desc}</p>
              </motion.div>
            ))}
          </div>
        </div>
      </div>

      <div>
        <h2 className="text-2xl font-bold mb-2 flex items-center gap-2">
          <MapPin className="w-6 h-6 text-amber" />
          Must-Visit Places
        </h2>
        <p className="text-offwhite-muted mb-4">Any specific places you don't want to miss?</p>
        <div className="flex flex-wrap gap-2 mb-3">
          {mustVisitTags.map((tag) => (
            <span
              key={tag}
              className="inline-flex items-center gap-1 px-3 py-1.5 rounded-full bg-amber/20 border border-amber/40 text-amber-light text-sm"
            >
              {tag}
              <button
                onClick={() => removeMustVisitTag(tag)}
                className="ml-1 text-amber hover:text-offwhite"
              >
                ×
              </button>
            </span>
          ))}
        </div>
        <div className="flex flex-col sm:flex-row gap-2">
          <input
            type="text"
            value={mustVisitInput}
            onChange={(e) => setMustVisitInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), addMustVisitTag())}
            placeholder="Add a place..."
            className="flex-1 bg-charcoal-lighter border border-border rounded-xl px-4 py-3 text-offwhite placeholder-offwhite-muted/50 focus:border-amber focus:outline-none transition-colors"
          />
          <Button onClick={addMustVisitTag} variant="outline" size="sm" className="w-full sm:w-auto">
            Add
          </Button>
        </div>
      </div>

      <div className="flex gap-4 justify-between">
        <Button variant="secondary" onClick={onBack} size="lg">
          Back
        </Button>
        <Button onClick={onGenerate} size="lg" className="animate-pulse-glow">
          Generate My Trip
        </Button>
      </div>
    </div>
  );
}
