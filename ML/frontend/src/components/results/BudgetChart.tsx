'use client';

import { motion } from 'framer-motion';
import {
  PieChart,
  Pie,
  Cell,
  ResponsiveContainer,
  Tooltip,
  Legend,
} from 'recharts';
import { BudgetBreakdown } from '@/types';

interface BudgetChartProps {
  budget: BudgetBreakdown;
  totalBudget?: number;
}

const COLORS = ['#D4A853', '#C4603A', '#E8C97A', '#8B7355'];

export function BudgetChart({ budget, totalBudget }: BudgetChartProps) {
  const data = [
    { name: 'Accommodation', value: budget.accommodation },
    { name: 'Food', value: budget.food },
    { name: 'Activities', value: budget.activities },
    { name: 'Transportation', value: budget.transportation },
  ];

  const total = budget.total || data.reduce((sum, d) => sum + d.value, 0);
  const isWithinBudget = totalBudget ? total <= totalBudget : true;

  const CustomTooltip = ({ active, payload }: any) => {
    if (active && payload && payload.length) {
      const data = payload[0].payload;
      const percentage = ((data.value / total) * 100).toFixed(1);
      return (
        <div className="bg-charcoal-light border border-border rounded-lg p-3 shadow-xl">
          <p className="text-sm font-medium text-offwhite">{data.name}</p>
          <p className="text-amber font-bold">{data.value} EGP</p>
          <p className="text-xs text-offwhite-muted">{percentage}%</p>
        </div>
      );
    }
    return null;
  };

  return (
    <motion.div
      className="glass-card p-6 md:p-8"
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
    >
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8 items-center">
        <div className="relative h-64 md:h-80">
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie
                data={data}
                cx="50%"
                cy="50%"
                innerRadius={60}
                outerRadius={90}
                paddingAngle={4}
                dataKey="value"
                stroke="none"
              >
                {data.map((_, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip content={<CustomTooltip />} />
            </PieChart>
          </ResponsiveContainer>

          <div className="absolute inset-0 flex flex-col items-center justify-center pointer-events-none">
            <span className="text-2xl font-bold text-offwhite">{total}</span>
            <span className="text-xs text-offwhite-muted">EGP Total</span>
          </div>
        </div>

        <div className="space-y-4">
          <h3 className="text-xl font-bold mb-4">Budget Breakdown</h3>

          {data.map((item, index) => {
            const percentage = ((item.value / total) * 100).toFixed(1);
            return (
              <div key={item.name}>
                <div className="flex items-center justify-between mb-1">
                  <div className="flex items-center gap-2">
                    <div
                      className="w-3 h-3 rounded-full"
                      style={{ backgroundColor: COLORS[index] }}
                    />
                    <span className="text-sm text-offwhite">{item.name}</span>
                  </div>
                  <span className="text-sm font-medium text-amber">
                    {item.value} EGP ({percentage}%)
                  </span>
                </div>
                <div className="w-full h-2 bg-charcoal-lighter rounded-full overflow-hidden">
                  <motion.div
                    className="h-full rounded-full"
                    style={{ backgroundColor: COLORS[index] }}
                    initial={{ width: 0 }}
                    whileInView={{ width: `${percentage}%` }}
                    viewport={{ once: true }}
                    transition={{ duration: 0.8, delay: index * 0.1 }}
                  />
                </div>
              </div>
            );
          })}

          {totalBudget && (
            <div className="pt-4 border-t border-border">
              <div className="flex items-center justify-between">
                <span className="text-sm text-offwhite-muted">Budget Status</span>
                <span
                  className={`px-3 py-1 rounded-full text-sm font-medium ${
                    isWithinBudget
                      ? 'bg-success/10 text-success'
                      : 'bg-error/10 text-error'
                  }`}
                >
                  {isWithinBudget ? 'Within Budget' : 'Over Budget'}
                </span>
              </div>
            </div>
          )}
        </div>
      </div>
    </motion.div>
  );
}
