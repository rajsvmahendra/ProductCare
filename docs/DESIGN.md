# ProductCare — Product & Experience Design Specification

**Document purpose:** Authoritative visual, content, interaction and motion specification for the ProductCare web application.

**Product:** ProductCare — Product Warranty & Service Management Portal  
**Experience goal:** A credible, polished software product that can be demonstrated and pitched confidently, while remaining usable, accessible and straightforward to maintain.  
**Implementation stack:** ASP.NET Core 10 MVC, Razor Views, C#, EF Core 10, SQL Server, ASP.NET Core Identity, Bootstrap 5, HTML, CSS and JavaScript.

---

## 1. Design direction

### 1.1 The desired ProductCare character

ProductCare should feel like a thoughtful, premium consumer software product: calm, capable, reassuring and precise. The interface should communicate that the user can understand what they own, what is still covered, what needs attention and what is happening with a repair request.

The design must not look like:
- An unmodified Bootstrap template.
- A generic admin dashboard with a marketing headline placed above it.
- A student assignment cover page disguised as a website.
- A collection of repetitive white cards with identical icons.
- A technical architecture diagram presented as the main product experience.
- A flashy 3D landing page whose animation distracts from the actual product.

It should look like a coherent, original ProductCare application with a recognisable design language across the landing page, authentication, customer portal and administrator portal.

### 1.2 Inspiration synthesis — use principles, not copies

The supplied reference files are design-system studies of Airbnb, Stripe and Notion. Use their principles selectively:

**Airbnb contributes:** human warmth, generous whitespace, clear content grouping, soft surfaces, approachable rounded cards, readable captions and restrained elevation. Use these ideas to make warranty management feel reassuring rather than bureaucratic.

**Stripe contributes:** editorial typography, confident product storytelling, precise alignment, strong information hierarchy, well-composed floating product mockups, navy-tinted depth and a sense of engineering quality. Use these ideas for the landing hero and the product-preview composition.

**Notion contributes:** quiet monochrome structure, disciplined use of one main interaction colour, calm layouts, clear navigation, subtle borders, lightweight shadows and a high signal-to-noise ratio. Use these ideas for forms, dashboards, tables and content-heavy screens.

**Do not copy** any of the three brands' logos, distinctive artwork, exact page layouts, proprietary typefaces or complete palettes. ProductCare must have its own brand identity.

### 1.3 Brand promise and voice

Primary tagline:

> **Everything you own. Everything covered. All in one place.**

Alternative supporting line:

> Keep product records organised, know when warranties need attention, and follow every service request from submission to resolution.

Brand personality:
- Reassuring, never childish.
- Modern, never trendy for its own sake.
- Clear, never overloaded with technical language.
- Confident, never exaggerated.
- Helpful, never salesy.

Avoid unsupported claims such as “enterprise-ready”, “real-time” or “immutable” unless the implementation genuinely proves them. Do not call the application “AI-powered” or imply external email/SMS reminders if those features do not exist.

---

## 2. ProductCare brand system

### 2.1 Brand concept

Create an original ProductCare wordmark paired with a compact icon. The icon can combine a protective shield outline with a subtle check/coverage mark or a circular continuity motif. It must be simple enough to work at 20–28px in navigation and at larger sizes in the landing page.

Use an inline SVG or CSS-drawn mark so it remains crisp and does not rely on a random emoji. Keep the wordmark text-based. Do not use a shield icon from one page and unrelated icons elsewhere.

### 2.2 Colour tokens

Use the following ProductCare palette as the design source of truth. These are original values selected for ProductCare, informed by the supplied reference systems without reproducing their exact brand identities.

| Token | Value | Purpose |
|---|---:|---|
| `--pc-ink` | `#111827` | Primary headings and high-emphasis text |
| `--pc-ink-soft` | `#263247` | Secondary headings and dark UI surfaces |
| `--pc-navy` | `#17233F` | Portal sidebar, dark feature panel and visual depth |
| `--pc-primary` | `#4F5FEA` | Main actions, active navigation and links |
| `--pc-primary-hover` | `#3D4BD0` | Hover/pressed primary action |
| `--pc-primary-soft` | `#EEF0FF` | Selected backgrounds, subtle chips and focus context |
| `--pc-teal` | `#12A89A` | Coverage/healthy-state accent and secondary highlights |
| `--pc-teal-soft` | `#E5F7F4` | Healthy coverage tint |
| `--pc-canvas` | `#F7F8FC` | Main application background |
| `--pc-surface` | `#FFFFFF` | Cards, forms and menus |
| `--pc-surface-alt` | `#F1F3F8` | Quiet alternate section and inset panels |
| `--pc-border` | `#E4E8F0` | Hairline borders and separators |
| `--pc-text` | `#263247` | Standard body text |
| `--pc-muted` | `#68758B` | Supporting copy and metadata |
| `--pc-faint` | `#8E99AA` | Tertiary metadata only; maintain readable contrast |
| `--pc-success` | `#16835D` | Resolved/healthy states |
| `--pc-success-soft` | `#E6F5EE` | Success badge background |
| `--pc-warning` | `#B7791F` | Expiring soon / attention needed |
| `--pc-warning-soft` | `#FFF4DD` | Warning badge background |
| `--pc-danger` | `#C2414B` | Expired, rejected or destructive states |
| `--pc-danger-soft` | `#FDEBED` | Danger badge background |
| `--pc-info` | `#2878B8` | Informational state |
| `--pc-info-soft` | `#E8F3FC` | Informational badge background |

Colour rules:
- Blue-violet is the primary interaction colour. Do not make every component blue.
- Teal is a product-specific coverage accent, not a second competing CTA colour.
- Green, amber and red communicate genuine status only.
- Use white and cool off-white as the main canvas; navy is a supporting structural colour.
- Gradients are allowed only in the hero artwork, the 3D-style product preview backdrop and a few carefully controlled decorative accents. Do not put gradients on every button or card.
- Do not use pure black for normal body copy.
- Do not use low-contrast pale grey for important text.

### 2.3 Typography

Use an accessible, modern sans-serif system. Preferred stack:
- Display/headings: `Manrope`, with `Inter` and system sans-serif fallbacks.
- UI/body: `Inter`, with system sans-serif fallbacks.

If remote font loading is used, provide a reliable system fallback. Do not attempt to use Airbnb Cereal, Söhne or proprietary Notion Inter assets from the references.

Typography scale:

| Role | Desktop | Tablet | Mobile | Weight / line-height |
|---|---:|---:|---:|---|
| Marketing hero H1 | 60–64px | 48px | 38–42px | 600–650 / 1.04–1.12 |
| Marketing section H2 | 38–42px | 34px | 28–32px | 600 / 1.15–1.25 |
| Portal page title | 30–34px | 28px | 25–28px | 600 / 1.2 |
| Card heading | 18–20px | 18px | 17–18px | 600 / 1.3 |
| Lead paragraph | 18px | 17px | 16–17px | 400 / 1.55–1.65 |
| Body / form text | 15–16px | 15–16px | 15–16px | 400 / 1.5–1.6 |
| Field label / navigation | 13–14px | 13–14px | 13–14px | 500–600 / 1.35 |
| Caption / metadata | 12–13px | 12–13px | 12–13px | 400–500 / 1.4 |
| Eyebrow / overline | 11–12px | 11–12px | 11–12px | 600, uppercase, 0.08em tracking |

### 2.4 Layout, spacing and grid

- Desktop content max-width: 1240–1280px.
- Wide hero and dashboard areas may use up to 1320px when useful.
- Desktop page gutters: 40–56px; medium screens: 28–32px; mobile: 18–22px.
- Use a 12-column grid for major marketing compositions.
- Typical card/grid gap: 20–24px.
- Marketing section vertical spacing: 88–104px desktop, 64–72px tablet, 48–56px mobile.
- Portal section spacing: 28–36px between major dashboard regions, 16–24px within groups.
- Header height: 72–76px marketing; 64–72px portal topbar.
- Customer/admin sidebar: approximately 248px desktop, collapsible on smaller screens.

### 2.5 Shape and elevation

| Element | Radius |
|---|---:|
| Small icon controls | 10px |
| Inputs and compact buttons | 10px |
| Main buttons | 12px |
| Standard cards | 16px |
| Large hero/mockup panels | 20–24px |
| Status pills and small tags | 999px |

Elevation levels:
- Flat: page canvas and ordinary content.
- Hairline: `1px solid var(--pc-border)` for cards, tables and inputs.
- Soft: `0 4px 18px rgba(24, 39, 75, 0.06)` for selected cards and menus.
- Floating: `0 24px 70px rgba(18, 31, 63, 0.16)` for the landing-page product mockup only.

### 2.6 Controls and component tokens

Buttons:
- Primary: 48px height, 12px radius, blue-violet fill (`#4F5FEA`), white label, 15–16px medium text.
- Secondary: 46–48px height, white or pale blue fill, 1px border, dark/blue text.
- Tertiary/ghost: transparent with readable text and clear hover state.
- Icon-only controls: at least 40px hit area; 44px on touch layouts.

Inputs:
- 48–52px height for standard fields.
- 12px radius, 1px border, white fill.
- Clear label above the control.
- Visible focus ring using blue-violet tint.
- Error text directly beneath the field.

Status badges:
- Active/covered: teal or green tint.
- Expiring soon: amber tint.
- Expired/rejected: red tint.
- Submitted/under review: blue tint.

---

## 3. Motion and dimensional design

- Hero product visualization: Layered ProductCare software mockup using HTML, CSS and inline SVG. Dimensional depth using `perspective`, `transform-style: preserve-3d`, `rotateX` and `rotateY`. Slow ambient float (6–9s, 6–10px).
- Floating chips with subtle independent drift.
- Entry and scroll reveals with `IntersectionObserver` or CSS transitions.
- Respect `prefers-reduced-motion: reduce`.
- No Three.js or heavy external libraries.

---

## 4. Public marketing experience

- **Header:** Light, confident, translucent with subtle blur, ProductCare brand mark + wordmark.
- **Hero:** "Know what’s covered. Stay ahead of what’s next." with dimensional interactive product mockup.
- **Problem section:** "Purchase details shouldn't disappear into a drawer."
- **Workflow:** 3-step connected visual process.
- **Feature bento:** Asymmetrical bento grid with 4 distinct, purposeful UI miniatures (Product Library, Warranty Clarity, Service Timeline, Secure Records).
- **Service Journey:** Real status lifecycle visual.
- **Trust & Control:** Customer data ownership and private invoice security.
- **Footer:** Crisp, modern, unobtrusive.

---

## 5. Authentication experience

- **Split layout:** 44% brand/story panel on left with rich navy atmosphere, ProductCare shield mark, and interactive coverage preview; 56% clean form canvas on right.
- **Focused forms:** Name, Email, Password, Confirm Password with show/hide password toggle.

---

## 6. Authenticated customer & admin portals

- **Portal shell:** Dedicated portal layout with modern 248px navy/ink sidebar, crisp brand mark, active navigation indicator, contextual breadcrumb topbar, user profile avatar menu.
- **Dashboard:** Rich summary cards, coverage breakdown ring/progress, expiring warranty alerts, recent service requests.
- **Product library:** Branded category SVG badges/illustrations, warranty countdown chips, modern responsive cards and tables.
- **Service tickets:** Polished vertical progress stepper for the 8 lifecycle states, priority badges, technician notes, customer cancellation.
- **Admin queue & reports:** High-density data grid, multi-parameter filters, operational metrics from stored procedures & views.
