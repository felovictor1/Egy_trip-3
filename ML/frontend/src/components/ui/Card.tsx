import React from 'react';
import { cn } from '@/lib/utils';

interface CardProps {
  children: React.ReactNode;
  className?: string;
  onClick?: () => void;
}

export function Card({ children, className, onClick }: CardProps) {
  return (
    <div
      onClick={onClick}
      className={cn(
        'glass-card p-6',
        onClick && 'cursor-pointer',
        className
      )}
    >
      {children}
    </div>
  );
}
