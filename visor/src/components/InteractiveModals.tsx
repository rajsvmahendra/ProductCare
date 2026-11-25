import React from 'react';
import { motion, AnimatePresence } from 'motion/react';
import { X, Check, Star, Shield, Cpu, Glasses, Zap, ArrowRight } from 'lucide-react';

interface InteractiveModalsProps {
  activeModal: 'buy' | 'get-started' | 'learn-more' | 'reviews' | null;
  onClose: () => void;
}

export const InteractiveModals: React.FC<InteractiveModalsProps> = ({
  activeModal,
  onClose,
}) => {
  return (
    <AnimatePresence>
      {activeModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          {/* Backdrop */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            onClick={onClose}
            className="absolute inset-0 bg-slate-950/70 backdrop-blur-md"
          />

          {/* Modal Card */}
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: 15 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: 15 }}
            className="relative w-full max-w-lg bg-white rounded-3xl p-6 sm:p-8 shadow-2xl border border-slate-100 z-10 overflow-hidden"
          >
            {/* Close Button */}
            <button
              id="modal-close-btn"
              onClick={onClose}
              className="absolute top-5 right-5 p-2 rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors"
            >
              <X className="w-5 h-5" />
            </button>

            {/* Modal Content: BUY NOW $600 */}
            {activeModal === 'buy' && (
              <div className="space-y-6">
                <div className="flex items-center gap-3">
                  <div className="w-12 h-12 rounded-2xl bg-slate-900 text-white flex items-center justify-center font-bold text-lg">
                    VR
                  </div>
                  <div>
                    <span className="text-xs uppercase tracking-wider font-bold text-cyan-700 bg-cyan-50 px-2.5 py-0.5 rounded-full">
                      Next-Gen System
                    </span>
                    <h3 className="text-2xl font-bold text-slate-900 mt-1">
                      Visor Vision X1
                    </h3>
                  </div>
                </div>

                <div className="bg-slate-50 p-4 rounded-2xl border border-slate-200/70 space-y-2">
                  <div className="flex justify-between items-center text-sm">
                    <span className="text-slate-600">Spatial Optics</span>
                    <span className="font-semibold text-slate-900">4K Micro-OLED / Eye</span>
                  </div>
                  <div className="flex justify-between items-center text-sm">
                    <span className="text-slate-600">Tracking Engine</span>
                    <span className="font-semibold text-slate-900">6DoF Sub-Millimeter</span>
                  </div>
                  <div className="flex justify-between items-center text-sm">
                    <span className="text-slate-600">Battery Life</span>
                    <span className="font-semibold text-slate-900">Up to 4.5 Hours</span>
                  </div>
                  <div className="border-t border-slate-200 pt-2 mt-2 flex justify-between items-center">
                    <span className="font-medium text-slate-700">Special Launch Price</span>
                    <span className="text-2xl font-extrabold text-slate-900">$600 <span className="text-xs text-slate-400 line-through">$899</span></span>
                  </div>
                </div>

                <div className="space-y-3">
                  <button
                    onClick={() => {
                      alert('Thank you for ordering Visor VR! Order confirmed.');
                      onClose();
                    }}
                    className="w-full py-3.5 px-6 rounded-xl bg-slate-900 hover:bg-slate-800 text-white font-semibold text-center transition-all shadow-md active:scale-98"
                  >
                    Complete Pre-Order ($600)
                  </button>
                  <p className="text-center text-xs text-slate-400">
                    Includes 2-year warranty & free worldwide expedited shipping.
                  </p>
                </div>
              </div>
            )}

            {/* Modal Content: GET STARTED */}
            {activeModal === 'get-started' && (
              <div className="space-y-5">
                <div className="space-y-1">
                  <span className="text-xs uppercase tracking-wider font-bold text-cyan-700 bg-cyan-50 px-2.5 py-0.5 rounded-full">
                    Welcome to Visor
                  </span>
                  <h3 className="text-2xl font-bold text-slate-900 pt-2">
                    Get Started With Visor VR
                  </h3>
                  <p className="text-sm text-slate-600">
                    Experience the future of enterprise spatial computing and collaboration.
                  </p>
                </div>

                <form
                  onSubmit={(e) => {
                    e.preventDefault();
                    alert('Request received! A Visor specialist will contact you shortly.');
                    onClose();
                  }}
                  className="space-y-4"
                >
                  <div>
                    <label className="block text-xs font-semibold text-slate-700 uppercase mb-1">
                      Full Name
                    </label>
                    <input
                      type="text"
                      required
                      placeholder="e.g. Alex Morgan"
                      className="w-full px-4 py-2.5 rounded-xl border border-slate-200 focus:outline-none focus:ring-2 focus:ring-slate-900 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-semibold text-slate-700 uppercase mb-1">
                      Business Email
                    </label>
                    <input
                      type="email"
                      required
                      placeholder="alex@company.com"
                      className="w-full px-4 py-2.5 rounded-xl border border-slate-200 focus:outline-none focus:ring-2 focus:ring-slate-900 text-sm"
                    />
                  </div>
                  <div>
                    <label className="block text-xs font-semibold text-slate-700 uppercase mb-1">
                      Interest Area
                    </label>
                    <select className="w-full px-4 py-2.5 rounded-xl border border-slate-200 focus:outline-none focus:ring-2 focus:ring-slate-900 text-sm bg-white">
                      <option>Enterprise Simulation & Training</option>
                      <option>Industrial 3D Design & CAD</option>
                      <option>Medical & Healthcare VR</option>
                      <option>Visor Hardware Purchase</option>
                    </select>
                  </div>
                  <button
                    type="submit"
                    className="w-full py-3 rounded-xl bg-slate-900 hover:bg-slate-800 text-white font-semibold text-sm transition-all"
                  >
                    Request Guided Demo
                  </button>
                </form>
              </div>
            )}

            {/* Modal Content: LEARN MORE */}
            {activeModal === 'learn-more' && (
              <div className="space-y-5">
                <div>
                  <span className="text-xs uppercase tracking-wider font-bold text-cyan-700 bg-cyan-50 px-2.5 py-0.5 rounded-full">
                    Technology Overview
                  </span>
                  <h3 className="text-2xl font-bold text-slate-900 mt-2">
                    Empowering Businesses With Next-Gen VR
                  </h3>
                  <p className="text-sm text-slate-600 mt-1">
                    Visor blends neural optics, zero-latency spatial audio, and haptic feedback.
                  </p>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 pt-2">
                  <div className="p-3.5 rounded-2xl bg-slate-50 border border-slate-100 flex items-start gap-3">
                    <Cpu className="w-5 h-5 text-cyan-600 shrink-0 mt-0.5" />
                    <div>
                      <h4 className="text-sm font-semibold text-slate-900">Neural Co-Processor</h4>
                      <p className="text-xs text-slate-500 mt-0.5">Real-time eye-tracking & foveated rendering</p>
                    </div>
                  </div>
                  <div className="p-3.5 rounded-2xl bg-slate-50 border border-slate-100 flex items-start gap-3">
                    <Shield className="w-5 h-5 text-cyan-600 shrink-0 mt-0.5" />
                    <div>
                      <h4 className="text-sm font-semibold text-slate-900">Enterprise Security</h4>
                      <p className="text-xs text-slate-500 mt-0.5">SOC2 & end-to-end encrypted spatial streams</p>
                    </div>
                  </div>
                  <div className="p-3.5 rounded-2xl bg-slate-50 border border-slate-100 flex items-start gap-3">
                    <Zap className="w-5 h-5 text-cyan-600 shrink-0 mt-0.5" />
                    <div>
                      <h4 className="text-sm font-semibold text-slate-900">Ultra-low Latency</h4>
                      <p className="text-xs text-slate-500 mt-0.5">Sub-5ms photon-to-motion latency response</p>
                    </div>
                  </div>
                  <div className="p-3.5 rounded-2xl bg-slate-50 border border-slate-100 flex items-start gap-3">
                    <Glasses className="w-5 h-5 text-cyan-600 shrink-0 mt-0.5" />
                    <div>
                      <h4 className="text-sm font-semibold text-slate-900">Ultralight Chassis</h4>
                      <p className="text-xs text-slate-500 mt-0.5">Only 290g balanced ergonomic fit</p>
                    </div>
                  </div>
                </div>

                <button
                  onClick={onClose}
                  className="w-full py-3 rounded-xl bg-slate-900 hover:bg-slate-800 text-white font-semibold text-sm transition-all"
                >
                  Close Overview
                </button>
              </div>
            )}

            {/* Modal Content: REVIEWS */}
            {activeModal === 'reviews' && (
              <div className="space-y-5">
                <div className="flex items-center justify-between">
                  <div>
                    <h3 className="text-2xl font-bold text-slate-900">Customer Reviews</h3>
                    <p className="text-sm text-slate-500">Over 750+ verified enterprise teams</p>
                  </div>
                  <div className="flex items-center gap-1.5 bg-amber-50 px-3 py-1.5 rounded-2xl border border-amber-200">
                    <Star className="w-5 h-5 text-amber-500 fill-amber-500" />
                    <span className="font-bold text-lg text-slate-900">4.9 / 5</span>
                  </div>
                </div>

                <div className="space-y-3 max-h-60 overflow-y-auto pr-1">
                  <div className="p-3 rounded-2xl bg-slate-50 border border-slate-100 space-y-1">
                    <div className="flex items-center justify-between">
                      <span className="text-sm font-semibold text-slate-900">Elena Rostova</span>
                      <span className="text-xs text-slate-400">CTO, HoloDesign</span>
                    </div>
                    <p className="text-xs text-slate-600">
                      "Visor reduced our collaborative prototyping cycle by 70%. The clarity and tracking precision are completely unmatched."
                    </p>
                  </div>
                  <div className="p-3 rounded-2xl bg-slate-50 border border-slate-100 space-y-1">
                    <div className="flex items-center justify-between">
                      <span className="text-sm font-semibold text-slate-900">Marcus Chen</span>
                      <span className="text-xs text-slate-400">Head of VR, AeroSim</span>
                    </div>
                    <p className="text-xs text-slate-600">
                      "The lightweight form factor allows our engineers to work in virtual spaces for hours with zero fatigue."
                    </p>
                  </div>
                </div>

                <button
                  onClick={onClose}
                  className="w-full py-3 rounded-xl bg-slate-900 hover:bg-slate-800 text-white font-semibold text-sm transition-all"
                >
                  Close Reviews
                </button>
              </div>
            )}

          </motion.div>
        </div>
      )}
    </AnimatePresence>
  );
};
