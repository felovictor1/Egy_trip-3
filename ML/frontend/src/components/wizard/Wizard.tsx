'use client';

import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { ChevronRight, ChevronLeft } from 'lucide-react';
import { Step1 } from './Step1';
import { Step2 } from './Step2';
import { Step3 } from './Step3';
import { useTripStore } from '@/store/useTripStore';

interface WizardProps {
  onGenerate: () => void;
}

const steps = ['Where & When', 'Your Vibe', 'Logistics'];

export function Wizard({ onGenerate }: WizardProps) {
  const { currentStep, setCurrentStep } = useTripStore();

  const handleNext = () => {
    if (currentStep < 2) {
      setCurrentStep(currentStep + 1);
    }
  };

  const handleBack = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleGenerate = () => {
    onGenerate();
  };

  const slideVariants = {
    enter: (direction: number) => ({
      x: direction > 0 ? 300 : -300,
      opacity: 0,
    }),
    center: {
      x: 0,
      opacity: 1,
    },
    exit: (direction: number) => ({
      x: direction < 0 ? 300 : -300,
      opacity: 0,
    }),
  };

  return (
    <div className="min-h-screen bg-charcoal py-12 px-4">
      <div className="max-w-3xl mx-auto">
        {/* Progress Indicator */}
        <div className="mb-12">
          <div className="flex items-center justify-between mb-4">
            {steps.map((step, index) => (
              <div key={step} className="flex items-center">
                <div className="flex flex-col items-center">
                  <div
                    className={`w-10 h-10 rounded-full flex items-center justify-center text-sm font-bold transition-all duration-300 ${
                      index <= currentStep
                        ? 'bg-amber text-charcoal'
                        : 'bg-charcoal-lighter text-offwhite-muted border border-border'
                    }`}
                  >
                    {index + 1}
                  </div>
                  <span
                    className={`text-xs mt-2 hidden sm:block ${
                      index <= currentStep ? 'text-amber' : 'text-offwhite-muted'
                    }`}
                  >
                    {step}
                  </span>
                </div>
                {index < steps.length - 1 && (
                  <div
                    className={`w-16 sm:w-24 h-0.5 mx-2 sm:mx-4 transition-all duration-300 ${
                      index < currentStep ? 'bg-amber' : 'bg-border'
                    }`}
                  />
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Step Content */}
        <AnimatePresence mode="wait" custom={currentStep}>
          <motion.div
            key={currentStep}
            custom={currentStep}
            variants={slideVariants}
            initial="enter"
            animate="center"
            exit="exit"
            transition={{ duration: 0.3, ease: 'easeInOut' }}
          >
            {currentStep === 0 && <Step1 onNext={handleNext} />}
            {currentStep === 1 && <Step2 onNext={handleNext} onBack={handleBack} />}
            {currentStep === 2 && <Step3 onBack={handleBack} onGenerate={handleGenerate} />}
          </motion.div>
        </AnimatePresence>
      </div>
    </div>
  );
}
