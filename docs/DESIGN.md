# ProductCare — The Ownership Continuity Experience
## Master Visual, Interaction and UX Design Specification

**Document Version:** 2.0 (Authoritative Specification)  
**Product:** ProductCare — Product Warranty and Service Management Portal (INT402)  
**Design Philosophy:** The Ownership Continuity Experience  
**Core Technologies:** ASP.NET Core 10 MVC, Razor Views, C# 13, EF Core 10, Microsoft SQL Server, ASP.NET Core Identity, CSS3 Tokens, Vanilla JavaScript  

---

## 1. Product Design Philosophy & Positioning

### 1.1 Product Positioning
"ProductCare is the ownership management platform that keeps every product record, warranty commitment and service journey connected in one place."

ProductCare addresses a universal human problem: consumers acquire valuable physical goods, yet the operational paperwork of ownership—purchase dates, serial numbers, physical receipts, warranty expirations, and after-sales service tickets—is fragmented across paper receipts, unread emails, and disconnected service counters.

### 1.2 Design Character & Personality
The visual character of ProductCare is defined as:
- **Premium & Editorial:** Anchored by balanced proportions, disciplined serif display typography, and structured spatial composition.
- **Precise & Calm:** Warm mineral surfaces, hairline borders, and muted stone contrasts that invite focused interaction.
- **Human & Trustworthy:** Clear document-inspired visual details that reassure the customer their physical property is digitally protected.
- **Functional & Grounded:** Every element, button, and table is tied directly to verified backend domain logic, without decorative AI-style fluff or generic SaaS tropes.

### 1.3 Prohibited Design Habits (Anti-Patterns)
The following visual habits are explicitly prohibited:
- Generic indigo/blue or bright purple SaaS gradients.
- Oversized marketing headlines with randomly colored gradient highlight words.
- Floating browser windows tilted in 3D with fake badges and infinite bouncing animations.
- Repetitive generic 3-step numbered cards with identical iconography.
- Standard asymmetric bento boxes stuffed with fake metrics and decorative chips.
- Unnecessary Three.js, Canvas, or heavy animation libraries.
- Hardcoded sample data in place of real database values.

---

## 2. Color System & Design Tokens

The color system replaces generic neon/indigo palettes with a quiet, organic mineral palette designed for readability, calm authority, and accessibility.

### 2.1 Palette Tokens

| Token | Hex Value | Semantic Purpose |
|---|---|---|
| `--pc-canvas` | `#F2F0E9` | Warm mineral application canvas |
| `--pc-surface` | `#FAFAF7` | Soft white primary card, form, and table surface |
| `--pc-surface-inset` | `#EAE8DF` | Inset panels, table headers, and neutral containers |
| `--pc-graphite` | `#202A28` | Deep graphite text, dark portal sidebar, and high-emphasis labels |
| `--pc-forest` | `#315D50` | Primary action button, active brand signature, and focused states |
| `--pc-forest-hover` | `#264A40` | Hover and pressed state for primary forest green actions |
| `--pc-forest-soft` | `#E7EFEA` | Soft forest green tint for active indicators and selection context |
| `--pc-stone` | `#DCDAD1` | Muted stone borders, hairline separators, and card outlines |
| `--pc-stone-dark` | `#B8B5A8` | Input border hover and active divider rules |
| `--pc-amber` | `#D69A51` | Warm amber warning, expiring-soon attention, and priority badges |
| `--pc-amber-soft` | `#FDF4E9` | Warm amber alert and badge background |
| `--pc-text` | `#202A28` | Primary reading and form text |
| `--pc-text-muted` | `#747B75` | Secondary text, field descriptions, and metadata |
| `--pc-text-faint` | `#969D97` | Timestamp and tertiary notation |
| `--pc-status-active` | `#2E6F40` | Verified active warranty coverage (accessible dark green) |
| `--pc-status-active-bg`| `#EBF5EE` | Soft green background for active coverage |
| `--pc-status-expiring`| `#C87D20` | Expiring warranty coverage within 30 days (warm amber) |
| `--pc-status-expiring-bg`|`#FDF3E7` | Soft amber background for expiring coverage |
| `--pc-status-expired` | `#B83A38` | Expired warranty status and destructive alerts (restrained red) |
| `--pc-status-expired-bg`|`#FCEEEF` | Soft red background for expired status |
| `--pc-status-progress`| `#35688F` | Active in-progress service requests (muted slate blue) |
| `--pc-status-progress-bg`|`#EDF4F9`| Soft slate background for in-progress requests |

### 2.2 Accessibility & Contrast Rules
- All text on `--pc-canvas` and `--pc-surface` maintains a minimum contrast ratio of 4.5:1 for normal text and 7:1 for headings.
- Status badges do not rely solely on color; every badge includes semantic text and appropriate ARIA attributes.
- Borders use `--pc-stone` (`#DCDAD1`), providing clear physical boundaries without harsh black lines.

---

## 3. Typography Hierarchy

### 3.1 Font Families
- **Editorial Display:** `Newsreader`, Georgia, 'Times New Roman', serif. Used for thoughtful marketing introductions, section overviews, and the Ownership Desk brand signature.
- **Interface & Body:** `IBM Plex Sans`, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif. Used for navigation, buttons, forms, tables, and portal interfaces.
- **Tabular & Monospace:** `IBM Plex Sans`, SFMono-Regular, Consolas, monospace (with `font-variant-numeric: tabular-nums`). Used for ticket numbers (`PC-YYYYMMDD-XXXX`), serial numbers, dates, and warranty countdowns.

### 3.2 Type Scale

| Role | Element | Font & Weight | Size / Line-Height | Tracking |
|---|---|---|---|---|
| Hero Headline | H1 | `Newsreader` 500/600 | 52–58px / 1.12 | -0.02em |
| Section Headline | H2 | `Newsreader` 500/600 | 34–38px / 1.2 | -0.015em |
| Portal Page Title | H1 | `IBM Plex Sans` 600 | 26–28px / 1.25 | -0.01em |
| Section / Card Heading| H3/H4 | `IBM Plex Sans` 600 | 18–20px / 1.3 | -0.005em |
| Lead Editorial Text | p.lead | `IBM Plex Sans` 400 | 17–18px / 1.6 | normal |
| Standard Body Copy | p, body | `IBM Plex Sans` 400 | 15–16px / 1.55 | normal |
| Form Label / Header Nav| label, nav | `IBM Plex Sans` 500/600 | 13–14px / 1.4 | 0.01em |
| Metadata & Timestamps | small | `IBM Plex Sans` 400/500 | 12–13px / 1.4 | 0.02em |
| Eyebrow / Overline | .eyebrow | `IBM Plex Sans` 600 | 11–12px / 1.3 | 0.08em (caps) |

---

## 4. Header, Navigation and Footer Architecture

### 4.1 Global Public Header
- **Container:** Warm canvas background with subtle bottom border (`1px solid var(--pc-stone)`). Height: 70px.
- **Brand Treatment:** Original ProductCare shield mark featuring a protective outline and continuous circular verification knot rendered in `--pc-forest` and `--pc-amber`. Paired with a clean serif-inspired logotype "ProductCare" in `--pc-graphite`.
- **Navigation Items:** Clean text links with subtle forest-green underline on hover/active. No bulky pill buttons.
- **Action Buttons:** Secondary "Sign In" text link alongside a refined, solid "Create Account" button in `--pc-forest`.

### 4.2 Authenticated Portal Layout (`_PortalLayout.cshtml`)
- **Left Sidebar (250px):** Deep graphite background (`#202A28`), quiet stone borders, and subtle muted text (`#A1A9A3`).
- **Active Navigation States:** Left border accent line in `--pc-amber` (`#D69A51`) with soft background tint (`rgba(255,255,255,0.06)`).
- **Portal Topbar:** 64px clean header on `--pc-canvas` displaying contextual breadcrumb navigation, current role badge (`Customer` or `Admin`), customer name, and quick logout button.
- **Responsive Drawer:** Off-canvas menu on mobile viewports triggered by an accessible hamburger icon.

### 4.3 Branded Footer
- Clean, structured 3-column layout on `--pc-surface`.
- Clear description of ProductCare as an ownership management system.
- Direct links to Documentation, REST API (Swagger), Privacy, and Account Login.
- Respectful academic INT402 attribution without cluttering the public marketing presentation.

---

## 5. Public Landing Page Experience

### 5.1 Hero Section
- **Headline:** *"Everything you own. Every warranty. Every service journey. One place."*
- **Sub-headline:** *"A centralized ownership management platform that preserves purchase proofs, monitors warranty coverage, and tracks after-sales repair journeys with complete clarity."*
- **Call-to-Action:** Primary button *"Begin Product Registry"* leading to registration; secondary button *"View System Architecture & APIs"* leading to documentation.

### 5.2 Real Ownership Lifecycle Demonstration (Product Preview)
Instead of a floating, tilted mockup with stock art, the hero features a structured, realistic demonstration of a registered device:
- **Device Card:** Apple MacBook Pro 16" (M3 Max), Serial Number `C02G1001MD6R`.
- **Verified Proof of Purchase:** Physical invoice attached (`Apple-Invoice-2025.pdf`), 2.4 MB verified.
- **Coverage Radar:** 36-Month AppleCare+ commitment, 942 days remaining, status `Active`.
- **Service Journey Tracker:** Active maintenance request `PC-20261116-8912` progressing from `Submitted` to `Under Review` to `Assigned`.

### 5.3 Problem and Solution Narrative
Four human real-world ownership dilemmas presented with ProductCare's systematic resolution:
1. **The Disappearing Receipt:** Faded thermal receipts and misplaced emails replaced by secure, organized physical document storage.
2. **The Unnoticed Expiration:** Missing warranty claim deadlines replaced by automatic calculation and proactive 30-day radar alerts.
3. **The Opaque Repair Service:** Phone calls and unrecorded visits replaced by a formal 8-stage state machine with immutable status history.
4. **Fragmented Ownership History:** Scattered records across manufacturer sites centralized into a unified customer ownership registry.

### 5.4 Ownership Journey (How It Works)
A connected horizontal chronological timeline:
1. **Catalog:** Register brand, model, serial number, and attach the original purchase proof.
2. **Monitor:** Automated business rules calculate expiration dates and flag approaching thresholds.
3. **Resolve:** Raise service requests directly against registered devices and follow diagnostic updates.

### 5.5 Implemented Capabilities
- **Digital Product Registry:** Strict per-customer ownership and serial number uniqueness.
- **Warranty Calculator Service:** Dynamic date calculation (`PurchaseDate.AddMonths`) taking leap years and boundary dates into account.
- **Secure Document Vault:** Physical files stored outside web root (`Uploads/Invoices/`) with magic byte validation and authorized streaming retrieval.
- **State Machine Workflow:** 8-stage transition engine with immutable history logs.

---

## 6. Authentication Design — "The ProductCare Ownership Desk"

### 6.1 Concept & Atmosphere
When customers enter the login or registration pages, they arrive at **The ProductCare Ownership Desk**—a quiet, focused personal workspace designed to organize and protect their valuable belongings.

### 6.2 Layout Architecture
- **Editorial Header:** Soft serif introduction *"The Ownership Desk"* with supporting caption *"Your product records, warranties, and service histories are preserved right where you left them."*
- **Primary Form Card:** Centered, comfortable 520px card on `--pc-surface` with `--pc-stone` border and gentle natural shadow.
- **Record Badge Vignette:** A subtle, document-styled visual note accompanying the form showing an illustrative protected product record:
  ```text
  OWNERSHIP DESK — ACTIVE RECORD
  Device: ThinkPad X1 Carbon Gen 11
  Purchased: 18 October 2025
  Coverage: Active (426 Days Remaining)
  Proof: Verified PDF Attached
  ```
- **Interactive Details:** 
  - Accessible password visibility reveal toggle (SVG eye icons).
  - Floating focus halos in `--pc-forest`.
  - Clear server-side and client-side validation errors in `--pc-status-expired`.
  - Collapsible address section on registration for streamlined first-step onboarding.

---

## 7. Customer Application Experience

### 7.1 Customer Dashboard
- **Executive Metric Row:** 4 clean metric cards:
  - *Registered Products* (Total devices in personal vault).
  - *Active Warranties* (Products with valid remaining coverage).
  - *Expiring Soon* (Warranties expiring within 30 days, highlighted in `--pc-amber`).
  - *Open Service Requests* (Active repair tickets under review or in progress).
- **Expiring Warranty Radar Table:** Immediate attention list displaying device name, serial number, remaining days countdown, and a direct "Raise Service Ticket" action.
- **Recent Service Requests:** Live status stepper and technician remarks for ongoing tickets.
- **Quick Action Drawer:** Fast navigation to register a new device or check full warranty timeline.

### 7.2 Product Library & Details
- **Inventory Table:** Clean tabular layout with brand, model, serial number chip, purchase date, warranty badge, and invoice indicator.
- **Search & Filters:** Instant filtering by Category (Electronics, Appliances, Computers, etc.) and Warranty Status.
- **Product Details View:** 
  - Comprehensive device specifications.
  - Dynamic Warranty Card with remaining coverage progress bar.
  - Verified Purchase Invoice Card with direct secure download link (`DownloadInvoice`).
  - Related Service Tickets list.
  - Delete Product confirmation modal.

### 7.3 Warranty Timeline Overview
- Filterable tabs: *All Coverage*, *Active*, *Expiring Soon (&le;30 Days)*, *Expired*.
- Chronological expiration milestone table with remaining days count and expiration date.

### 7.4 Service Request Management
- **Creation Form:** Dropdown populated exclusively with the customer's registered devices, category selection, priority level (Low, Medium, High, Urgent), and detailed symptom text.
- **Ticket Tracking View:** 
  - Unique ticket identifier (`PC-YYYYMMDD-XXXX`).
  - Vertical status stepper showing current state and historical transitions.
  - Administrative remarks and technician assignment block.
  - Customer ticket cancellation button (available while in `Submitted` state before administrative triage begins).

---

## 8. Administrator Portal Experience

### 8.1 Operational Dashboard
- System-wide metric row: Total Customers, Total Products, Open Service Requests, Critical/Urgent Tickets.
- Triage queue of recently submitted requests requiring review.

### 8.2 Master Service Ticket Queue
- Multi-parameter filtering by Status, Priority, Category, and keyword search across ticket number, customer name, and serial number.
- High information density with tabular formatting and color-coded status badges.

### 8.3 Ticket Triage & Status Transition Form
- Displays full customer contact information, device specifications, and symptom description.
- Status transition dropdown populated strictly with valid permissible next states enforced by `ServiceRequestWorkflowService`.
- Technician assignment field and administrative remarks input.
- Complete chronological audit log with actor username and UTC timestamps.

### 8.4 Customer Directory & Global Inventory
- Searchable customer account directory showing registered device counts and open ticket numbers.
- Global product inventory across all customer accounts.

### 8.5 Operational Reports & Intelligence
- Ticket volume and resolution metrics powered by stored procedure `sp_GetServiceTicketStatistics`.
- Product warranty expiration forecast powered by database view `vw_ProductWarrantySummary`.

---

## 9. Motion & Interaction Rules

- **Restrained & Purposeful:** Motion is used strictly to provide physical feedback (button clicks, form validation, modal appearance).
- **Durations:** Hover transitions `150ms ease`, modal reveals `200ms ease-out`, alert dismissals `250ms ease-in`.
- **Reduced Motion:** When `@media (prefers-reduced-motion: reduce)` is detected, all transitions and transforms are disabled (`transition: none !important; animation: none !important;`).
- **Forbidden:** No continuous rotating badges, bouncing cards, or distracting mouse-following parallax particles.

---

## 10. Responsive Breakpoints & Testing Matrix

| Breakpoint | Target Viewport | Target Device | Layout Behavior |
|---|---|---|---|
| Large Desktop | `&ge; 1440px` | 27" / 24" Monitors | 1280px max-width container, full sidebar |
| Standard Laptop | `1200px – 1439px`| 13" / 15" Laptops | 1140px container, full sidebar |
| Tablet Landscape | `992px – 1199px` | iPad Pro / Tablets | 960px container, compact sidebar |
| Tablet Portrait | `768px – 991px` | iPad Mini / Tablets | Single-column forms, collapsible off-canvas drawer |
| Mobile Large | `390px – 767px` | iPhone 14/15/16, Pixel | Responsive tables with horizontal scroll, stacked cards |
| Mobile Small | `360px – 389px` | Android Compact | Full-width inputs, 14px typography, 44px min touch targets |

---

## 11. Implementation Integrity & Verification Rules

1. **Zero Fake Data:** All numbers, tickets, dates, and products displayed in the portal must originate from the database via strongly typed ViewModels.
2. **Strict Server-Side Authorization:** Every customer action must verify `UserId == CurrentUserId`. Every administrator action requires `[Authorize(Roles = "Admin")]`.
3. **No Dead Buttons or Links:** Every button on every page must execute a real GET or POST action.
4. **Preserve Business Logic:** `WarrantyCalculatorService`, `ServiceRequestWorkflowService`, and `InvoiceStorageService` must remain intact and verified by 50 automated xUnit tests.
