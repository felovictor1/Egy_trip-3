import React from 'react';
import { cn } from '@/lib/utils';

interface ChipProps {
  label: string;
  selected?: boolean;
  onClick?: () => void;
  icon?: React.ReactNode;
  className?: string;
}

export function Chip({ label, selected = false, onClick, icon, className }: ChipProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={cn(
        'inline-flex items-center gap-2 px-4 py-2 rounded-full text-sm font-medium transition-all duration-200 cursor-pointer select-none',
        selected
          ? 'bg-amber/25 border-amber text-amber-light'
          : 'bg-amber/10 border-amber/20 text-amber hover:bg-amber/20 hover:border-amber/40',
        'border',
        className
      )}
    >
      {icon}
      {label}
    </button>
  );
}
