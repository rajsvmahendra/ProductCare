import React, { useState } from 'react';
import { Navbar } from './components/Navbar';
import { CinematicHeroVideo } from './components/CinematicHeroVideo';
import { HeroCardsBanner } from './components/HeroCardsBanner';
import { InteractiveModals } from './components/InteractiveModals';

const VIDEO_BG_URL =
  '/hero.mp4';

export default function App() {
  const [activeNav, setActiveNav] = useState('Explore');
  const [activeModal, setActiveModal] = useState<
    'buy' | 'get-started' | 'learn-more' | 'reviews' | null
  >(null);

  return (
    <div className="min-h-screen bg-[#eaf1f7] flex flex-col items-center justify-start selection:bg-cyan-500 selection:text-white pb-16">
      
      {/* ============================================================ */}
      {/* HEADER SECTION (NAVBAR AT TOP, VIDEO CLEANLY BELOW) */}
      {/* ============================================================ */}
      <header className="w-full max-w-[1440px] px-3 sm:px-6 lg:px-8 pt-3 sm:pt-4 flex flex-col gap-3 sm:gap-4">
        {/* Navigation Bar at Top - Dedicated space, no overlap with video */}
        <Navbar
          activeNav={activeNav}
          onNavClick={(nav) => setActiveNav(nav)}
          onGetStartedClick={() => setActiveModal('get-started')}
        />

        {/* 16:9 Aspect Ratio Cinematic Video Frame */}
        <div
          id="header-16-9-frame"
          className="relative w-full aspect-[16/9] min-h-[380px] sm:min-h-[460px] md:min-h-[520px] lg:min-h-[580px] max-h-[720px] rounded-2xl sm:rounded-3xl overflow-hidden shadow-2xl bg-black"
        >
          {/* 
            Layer 1: Background Video (100% Opacity)
            Layer 2: Oversized "FUTURE VISION" Typography flanking the character
          */}
          <CinematicHeroVideo videoUrl={VIDEO_BG_URL} />
        </div>
      </header>

      {/* ============================================================ */}
      {/* 3 HORIZONTAL ANGLED BOXES EXACTLY BELOW THE VIDEO (Image 2) */}
      {/* ============================================================ */}
      <section className="w-full">
        <HeroCardsBanner
          onBuyNowClick={() => setActiveModal('buy')}
          onLearnMoreClick={() => setActiveModal('learn-more')}
          onReviewsClick={() => setActiveModal('reviews')}
        />
      </section>

      {/* Interactive Feature Modals */}
      <InteractiveModals
        activeModal={activeModal}
        onClose={() => setActiveModal(null)}
      />
    </div>
  );
}
