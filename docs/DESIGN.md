# PRODUCTCARE — DESIGN SPECIFICATION & INTERACTION BRIEF
**Version:** 3.0 (Midnight Precision)  
**Status:** Authoritative Design Specification  
**Architecture:** ASP.NET Core 10 MVC & REST APIs, Razor Views, Bootstrap 5, Custom Midnight CSS  
**Target Viewports:** 390&times;844 (Mobile), 430&times;932 (Large Mobile), 768&times;1024 (Tablet), 1024&times;768 (Desktop Small), 1440&times;900 (Desktop Standard), 1920&times;1080 (Wide Display)

---

## 1. Product Identity & Philosophy

**Product Name:** ProductCare  
**Product Category:** Product ownership, warranty intelligence, purchase-proof management, and after-sales service tracking.  
**Core Product Promise:** *Everything you own. Every warranty. Every service journey. One place.*

### Emotional & Brand Attributes
- **Trust:** Authoritative, cryptographic proof storage and per-customer tenant isolation.
- **Protection:** Vigilant, proactive warranty monitoring with early-warning radars.
- **Clarity:** High-contrast, unambiguous typography, tabular data, and transparent lifecycle progress.
- **Control:** Full ownership lifecycle management from unboxing to maintenance and warranty claims.
- **Technical Sophistication:** Precision-engineered dark visual atmosphere ("Midnight Precision") with subtle ambient lighting, hairline borders, and purposeful motion.

---

## 2. Visual Art Direction: Midnight Precision

ProductCare adopts **Midnight Precision**—a sophisticated dark visual environment designed to look like a high-end, studio-crafted engineering product rather than a generic SaaS template.

### 2.1 Color Token Matrix

| Token Name | Hex / Value | Semantic Role |
|---|---|---|
| `--pc-bg-main` | `#080D16` | Primary canvas background (Deep Midnight Obsidian) |
| `--pc-bg-secondary` | `#101827` | Secondary section background & structural panels |
| `--pc-surface-elevated`| `#151F30` | Luminous elevated card & container surface |
| `--pc-surface-popover` | `#1C283E` | High-elevation surface for modals, dropdowns, and drawers |
| `--pc-surface-inset` | `#0D1422` | Recessed data wells, code snippets, and inner cells |
| `--pc-text-primary` | `#F4F7FB` | Starlight white for high-contrast titles and headings |
| `--pc-text-secondary`| `#A7B3C4` | Slate muted for body text, descriptions, and labels |
| `--pc-text-faint` | `#627184` | Faint secondary text for timestamps and footnotes |
| `--pc-electric-teal` | `#38D9C5` | Primary brand accent: precision cyan/teal for key actions & active states |
| `--pc-electric-blue` | `#668CFF` | Secondary accent: high-energy cobalt for information and transitions |
| `--pc-warm-amber` | `#F3B66D` | Warning & attention accent: expiring warranties and high priority |
| `--pc-coral-red` | `#FF6B6B` | Danger accent: expired coverage and urgent alerts |
| `--pc-border-hairline`| `rgba(255, 255, 255, 0.08)` | Default subtle boundary line |
| `--pc-border-highlight`| `rgba(56, 217, 197, 0.35)` | Interactive focus & hover glow boundary |
| `--pc-border-subtle` | `rgba(255, 255, 255, 0.12)` | Card and panel structural boundaries |

### 2.2 Lighting & Atmosphere
- **Subtle Radial Lighting:** Soft ambient cyan and cobalt radial gradients placed strategically behind the hero and primary display elements (`radial-gradient(circle at 50% 0%, rgba(56, 217, 197, 0.08), transparent 60%)`).
- **Hairline Surface Contrast:** Precision 1px borders with controlled alpha channels create clean geometric boundaries without heavy shadows.
- **Delicate Glow Accents:** Primary buttons and active badges carry subtle localized illumination (`box-shadow: 0 0 20px rgba(56, 217, 197, 0.20)`).
- **Prohibited:** Giant random gradient blobs, excessive neon blur, or unreadable decorative frosted glass.

### 2.3 Typographic System
- **Display Headings:** *Space Grotesk* (Weights: 600, 700) — contemporary, geometric display typeface for large confident headlines and numbers.
- **Interface & Body:** *Plus Jakarta Sans* (Weights: 400, 500, 600, 700) & *Inter* — clean, legible sans-serif with excellent vertical metrics and x-height.
- **Numeric & Code:** Tabular lining figures (`font-variant-numeric: tabular-nums`) and monospace font for serial numbers, ticket codes (`PC-YYYYMMDD-XXXX`), and dates.

---

## 3. Motion & Interaction Design

1. **Staggered Hero Entrance:**
   - Display headline enters with a subtle upward translate (`translateY(16px)` to `0`) and opacity fade.
   - Supporting lead text follows with 150ms delay.
   - Action buttons and trust badges follow with 300ms delay.
   - Product demonstration enters smoothly from the right/below with 400ms delay.
2. **Scroll-Driven Reveals:**
   - Major landing page sections use an `IntersectionObserver` that toggles `.is-revealed`, animating opacity and a smooth 20px upward rise.
3. **Interactive Hover Feedback:**
   - Primary buttons: Slight lift (`translateY(-2px)`), enhanced electric teal border, and soft localized glow.
   - Navigation links: Hairline bottom indicator with smooth width expansion.
   - Table rows & feature cards: Subtle surface lightening (`#151F30` &rarr; `#1A263C`) and hairline border highlight.
4. **Password Visibility Reveal:**
   - Interactive show/hide toggle with custom SVG eye-open and eye-slash glyphs.
5. **Accessibility & Performance:**
   - Strict respect for `@media (prefers-reduced-motion: reduce)`.
   - All animations run on compositor-friendly properties (`transform`, `opacity`). Zero layout thrashing.

---

## 4. Public Landing Page Structure

### Section 1: Navigation
- **Dedicated Navigation Row:** Fixed top navigation with subtle backdrop blur (`rgba(8, 13, 22, 0.85)`).
- **Left:** Original ProductCare shield mark in electric teal and starlight white, paired with the Space Grotesk wordmark.
- **Center:** Overview | Ownership Journey | Why ProductCare | Capabilities | Security & Proof.
- **Right:** Sign In (ghost button) and Open Ownership Desk (electric teal pill).
- **Mobile Navigation:** Responsive hamburger toggle triggering a clean off-canvas drawer with no overflow.

### Section 2: Cinematic Hero & Grounded Demonstration
- **Asymmetric Editorial Layout:** Left-anchored content with large display typography:
  - *Kicker:* "THE OWNERSHIP INTELLIGENCE PLATFORM" with electric teal dot.
  - *Headline:* "Your products.<br />Their proof.<br /><span class='text-electric-teal'>Every service journey.</span>"
  - *Lead:* "One intelligent ownership platform for purchase records, warranty coverage, and after-sales service history."
  - *CTAs:* "Open Your Ownership Desk" & "Explore the Platform".
  - *Trust Badges:* Zero Subscription Fees &middot; Cryptographic Invoices &middot; 8-Stage Machine &middot; Sub-second Warranty Math.
- **Right Visual Centerpiece:** Layered, authentic ProductCare demonstration card:
  - Registered device record: Apple MacBook Pro 16" (M3 Max), S/N: `C02G1001MD6R`.
  - Verified purchase proof: 2.4 MB PDF invoice download indicator with cryptographic verification seal.
  - 36-Month dynamic warranty radar: 695 days remaining, color-coded active bar.
  - Connected service ticket: `PC-20261118-4091` in Progress, technician assignment, and live status update.

### Section 3: The Ownership Dilemma
- Explains the widespread pain points of consumer ownership across 4 asymmetric cards:
  1. *The Disappearing Receipt:* Thermal receipts fade to blank sheets; ProductCare stores verified PDFs outside web root.
  2. *The Lapsed Deadline:* Silent warranty expirations; 30-day proactive radar gives advance warning.
  3. *The Opaque Repair:* Disconnected service workshops; governed 8-state machine with immutable audit log.
  4. *Fragmented Records:* Spreadsheets and lost emails; unified digital ownership inventory.

### Section 4: The 6-Stage Ownership Journey
- Connected horizontal/stacked visual progression:
  `01. Register Acquisition` &rarr; `02. Preserve Proof` &rarr; `03. Dynamic Radar` &rarr; `04. Request Service` &rarr; `05. Governed Progress` &rarr; `06. Complete History`.

### Section 5: Core Capabilities
- Varied layout cards detailing real application modules:
  - Digital Product Vault (CRUD, serial constraints, category index).
  - Dynamic Warranty Intelligence (calendar math, leap year protection).
  - Purchase-Proof Storage (magic byte validation, GUID file storage).
  - Service Request Engine (collision-safe ticket codes, technician notes).
  - Workspaces for Customers & Administrators.
  - Operational Reports & Analytics (SQL View and Stored Procedure integration).

### Section 6: Security, Isolation & Proof
- Cryptographic invoice validation, tenant boundary isolation, ASP.NET Core Identity authentication, and immutable transition audit logs.

### Section 7: Final Invitation & Footer
- Confident closing call-to-action inviting users to launch their Ownership Desk.
- Structured footer with platform links, REST API Swagger link, and course disclosures.

---

## 5. Authentication Experience: The Ownership Desk

- **Visual Concept:** The ProductCare Ownership Desk—a focused, distraction-free environment.
- **Layout:**
  - Desktop: Asymmetric 2-column layout. Left column showcases an illuminated document record vignette with sample device credentials and verified invoice proof. Right column hosts the crisp, high-contrast form surface.
  - Mobile: Clean single-column layout with optimized touch spacing, zero horizontal clipping, and sticky action buttons.
- **Interactive Controls:**
  - Floating label inputs with visible focus halos in electric teal (`rgba(56, 217, 197, 0.25)`).
  - Accessible password visibility reveal with SVG eye toggle.
  - Progressive disclosure for optional physical address fields.

---

## 6. Customer & Administrator Portal Workspaces

- **Consistent Atmosphere:** The Midnight Precision design language continues into the authenticated experience.
- **Sidebar Navigation:** 250px deep obsidian sidebar (`#0B111D` to `#101827`) with electric teal active indicators, grouping headers, and quick-collapse drawer for mobile.
- **Top Bar:** Breadcrumb context, role badge, user profile dropdown, and quick logout.
- **Metric Cards:** High-contrast KPI indicators featuring electric teal, cobalt, and amber counters with progress gauges.
- **Tables & Lists:** High-contrast dark tables (`#151F30`) with hairline borders, tabular numerals, status pills, and responsive touch-scrolling.
- **Forms & Dialogs:** Two-column structured forms with stone dividers, clear validation summaries, and accessible confirmation modals.

---

## 7. Responsive Design Specification (Zero Overflow)

| Viewport Width | Breakpoint | Layout Adaptations |
|---|---|---|
| **< 576px (Mobile)** | `xs` | Single column; horizontal padding 16px; font scale 0.95; stacked buttons; drawer navigation; table touch-scroll; zero horizontal overflow. |
| **576px – 767px** | `sm` | Single column; 2-col metric cards; full-width hero demonstration. |
| **768px – 991px (Tablet)** | `md` | Split hero stacks cleanly; 2-column capability grid; collapsible portal sidebar. |
| **992px – 1199px** | `lg` | Full 2-column asymmetric hero; fixed portal sidebar; 3-column capability grid. |
| **&ge; 1200px (Desktop)** | `xl` | Maximum container width 1240px; full visual depth and ambient illumination. |
| **&ge; 1920px (Wide)** | `xxl` | Centered container with balanced negative margins; crisp high-DPI scaling. |
