import React from 'react';
import { cn } from '@/lib/utils';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'ghost' | 'outline';
  size?: 'sm' | 'md' | 'lg';
  children: React.ReactNode;
}

export function Button({
  variant = 'primary',
  size = 'md',
  className,
  children,
  ...props
}: ButtonProps) {
  const variants = {
    primary: 'bg-gradient-to-r from-amber to-amber-dark text-charcoal font-semibold hover:shadow-lg hover:shadow-amber/30 hover:-translate-y-0.5',
    secondary: 'bg-transparent text-amber border border-amber/50 hover:bg-amber/10',
    ghost: 'bg-transparent text-offwhite hover:bg-charcoal-lighter',
    outline: 'bg-transparent text-offwhite border border-border hover:border-amber',
  };

  const sizes = {
    sm: 'px-4 py-2 text-sm rounded-lg',
    md: 'px-6 py-3 text-base rounded-xl',
    lg: 'px-8 py-4 text-lg rounded-xl',
  };

  return (
    <button
      className={cn(
        'transition-all duration-300 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:translate-y-0',
        variants[variant],
        sizes[size],
        className
      )}
      {...props}
    >
      {children}
    </button>
  );
}
