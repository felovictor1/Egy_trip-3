'use client';

import { useRef, useEffect, useState } from 'react';
import { motion, useScroll, useTransform } from 'framer-motion';
import { ArrowDown, Sparkles } from 'lucide-react';
import { Button } from '@/components/ui/Button';
import { useTripStore } from '@/store/useTripStore';

interface HeroProps {
  onStartPlanning: () => void;
}

export function Hero({ onStartPlanning }: HeroProps) {
  const ref = useRef<HTMLDivElement>(null);
  const { scrollY } = useScroll();
  const y = useTransform(scrollY, [0, 500], [0, 200]);
  const opacity = useTransform(scrollY, [0, 300], [1, 0]);
  const [imageLoaded, setImageLoaded] = useState(false);

  const heroImage = 'https://images.unsplash.com/photo-1539650116574-8efeb43e2750?w=1920&q=80';

  return (
    <div ref={ref} className="relative h-[100dvh] w-full overflow-hidden">
      <motion.div
        style={{ y, opacity }}
        className="absolute inset-0"
      >
        <div className="absolute inset-0 bg-charcoal/40 z-10" />
        <div className="absolute inset-0 hero-overlay z-20" />

        <motion.img
          src={heroImage}
          alt="Egypt Pyramids"
          className={`w-full h-full object-cover transition-opacity duration-1000 ${
            imageLoaded ? 'opacity-100' : 'opacity-0'
          }`}
          style={{
            animation: imageLoaded ? 'kenBurns 20s ease-in-out infinite alternate' : 'none',
          }}
          onLoad={() => setImageLoaded(true)}
        />

        {!imageLoaded && (
          <div className="absolute inset-0 bg-charcoal animate-pulse" />
        )}
      </motion.div>

      <div className="relative z-30 flex flex-col items-center justify-center h-full text-center px-4">
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.2 }}
        >
          <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-amber/10 border border-amber/30 text-amber mb-8">
            <Sparkles className="w-4 h-4" />
            <span className="text-sm font-medium">Powered by AI + Real-time Data</span>
          </div>
        </motion.div>

        <motion.h1
          className="text-4xl sm:text-5xl md:text-7xl lg:text-8xl font-bold mb-6 leading-tight mt-16 sm:mt-0"
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.4 }}
        >
          <span className="gold-gradient-text">Plan your perfect</span>
          <br />
          <span className="text-offwhite">Egypt trip with AI</span>
        </motion.h1>

        <motion.p
          className="text-lg md:text-xl text-offwhite-muted max-w-2xl mb-12"
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.6 }}
        >
          Personalized itineraries, real-time prices, curated restaurants & hotels
        </motion.p>

        <motion.div
          initial={{ opacity: 0, y: 30 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8, delay: 0.8 }}
        >
          <Button
            size="lg"
            onClick={onStartPlanning}
            className="animate-pulse-glow"
          >
            Start Planning
          </Button>
        </motion.div>

        <motion.div
          className="absolute bottom-8 left-1/2 -translate-x-1/2"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 1.2, duration: 0.5 }}
        >
          <ArrowDown className="w-6 h-6 text-amber/50 animate-bounce" />
        </motion.div>
      </div>

      <div className="absolute inset-0 z-25 pointer-events-none">
        {[...Array(20)].map((_, i) => (
          <motion.div
            key={i}
            className="absolute w-1 h-1 bg-amber/30 rounded-full"
            style={{
              left: `${Math.random() * 100}%`,
              top: `${Math.random() * 100}%`,
            }}
            animate={{
              y: [0, -20, 0],
              opacity: [0.2, 0.8, 0.2],
            }}
            transition={{
              duration: 2 + Math.random() * 3,
              repeat: Infinity,
              delay: Math.random() * 2,
            }}
          />
        ))}
      </div>
    </div>
  );
}
