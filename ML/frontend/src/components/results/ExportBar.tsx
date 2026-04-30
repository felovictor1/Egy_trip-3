'use client';

import { motion } from 'framer-motion';
import { Download, Share2, Save, MapPin } from 'lucide-react';
import { Button } from '@/components/ui/Button';

export function ExportBar() {
  const handleDownloadPDF = () => {
    window.print();
  };

  const handleShare = async () => {
    if (navigator.share) {
      try {
        await navigator.share({
          title: 'My Egypt Trip Plan',
          text: 'Check out my personalized Egypt trip itinerary!',
          url: window.location.href,
        });
      } catch (err) {
        // Fallback: copy URL
        navigator.clipboard.writeText(window.location.href);
      }
    } else {
      navigator.clipboard.writeText(window.location.href);
    }
  };

  return (
    <motion.div
      className="fixed bottom-0 left-0 right-0 z-50 bg-charcoal/90 backdrop-blur-lg border-t border-border no-print"
      initial={{ y: 100 }}
      animate={{ y: 0 }}
      transition={{ delay: 1 }}
    >
      <div className="max-w-6xl mx-auto px-4 py-3 flex items-center justify-between">
        <div className="hidden md:flex items-center gap-2 text-sm text-offwhite-muted">
          <MapPin className="w-4 h-4 text-amber" />
          <span>Trip plan ready</span>
        </div>

        <div className="flex items-center gap-3 w-full md:w-auto justify-center">
          <Button
            variant="ghost"
            size="sm"
            onClick={handleDownloadPDF}
            className="flex items-center gap-2"
          >
            <Download className="w-4 h-4" />
            <span className="hidden sm:inline">Download PDF</span>
          </Button>

          <Button
            variant="ghost"
            size="sm"
            onClick={handleShare}
            className="flex items-center gap-2"
          >
            <Share2 className="w-4 h-4" />
            <span className="hidden sm:inline">Share Link</span>
          </Button>

          <Button
            variant="ghost"
            size="sm"
            className="flex items-center gap-2"
          >
            <Save className="w-4 h-4" />
            <span className="hidden sm:inline">Save to My Trips</span>
          </Button>
        </div>
      </div>
    </motion.div>
  );
}
