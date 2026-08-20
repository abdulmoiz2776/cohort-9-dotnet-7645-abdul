---
name: Kinetic Logic
colors:
  surface: '#f7f9fb'
  surface-dim: '#d8dadc'
  surface-bright: '#f7f9fb'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f2f4f6'
  surface-container: '#eceef0'
  surface-container-high: '#e6e8ea'
  surface-container-highest: '#e0e3e5'
  on-surface: '#191c1e'
  on-surface-variant: '#434655'
  inverse-surface: '#2d3133'
  inverse-on-surface: '#eff1f3'
  outline: '#737686'
  outline-variant: '#c3c6d7'
  surface-tint: '#0053db'
  primary: '#004ac6'
  on-primary: '#ffffff'
  primary-container: '#2563eb'
  on-primary-container: '#eeefff'
  inverse-primary: '#b4c5ff'
  secondary: '#505f76'
  on-secondary: '#ffffff'
  secondary-container: '#d0e1fb'
  on-secondary-container: '#54647a'
  tertiary: '#005a82'
  on-tertiary: '#ffffff'
  tertiary-container: '#0074a6'
  on-tertiary-container: '#e4f2ff'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#dbe1ff'
  primary-fixed-dim: '#b4c5ff'
  on-primary-fixed: '#00174b'
  on-primary-fixed-variant: '#003ea8'
  secondary-fixed: '#d3e4fe'
  secondary-fixed-dim: '#b7c8e1'
  on-secondary-fixed: '#0b1c30'
  on-secondary-fixed-variant: '#38485d'
  tertiary-fixed: '#c9e6ff'
  tertiary-fixed-dim: '#89ceff'
  on-tertiary-fixed: '#001e2f'
  on-tertiary-fixed-variant: '#004c6e'
  background: '#f7f9fb'
  on-background: '#191c1e'
  surface-variant: '#e0e3e5'
typography:
  display-lg:
    fontFamily: Inter
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  headline-md:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '500'
    lineHeight: 20px
  label-sm:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 4px
  xs: 4px
  sm: 8px
  md: 16px
  lg: 24px
  xl: 32px
  2xl: 48px
  3xl: 64px
  gutter: 24px
  margin-mobile: 16px
  margin-desktop: 40px
---

## Brand & Style

The design system is anchored in **Corporate Modernism**, prioritizing clarity, efficiency, and reliability. It is designed for a task management and authentication platform where security and productivity are paramount.

The visual language focuses on a "clear-to-act" philosophy:
- **Minimalism:** High use of white space to reduce cognitive load during complex task management.
- **Precision:** Perfect alignment and systematic spacing to evoke a sense of order.
- **Trust:** A light-themed, airy aesthetic that feels professional and approachable.
- **Subtlety:** Using depth and soft color shifts rather than loud decorative elements to guide the user's eye.

## Colors

This design system utilizes a high-contrast, professional palette designed for long-term legibility.

- **Action Blue (#2563EB):** Reserved exclusively for primary actions, progress indicators, and active states. It signals "intent" and "completion."
- **Slate Tones:** Used for the typography hierarchy. The darkest slate (#1E293B) provides maximum readability for body text and headers, while lighter slates handle secondary metadata.
- **Foundation Grays:** A tiered system of light grays (#F8FAFC for main backgrounds, #F1F5F9 for nested containers) creates logical grouping without the need for heavy borders.
- **Functional Colors:** Success (Emerald), Warning (Amber), and Error (Rose) should follow the same saturation levels as the primary blue to ensure a cohesive look.

## Typography

The typography system is built on **Inter**, a typeface optimized for screen readability and UI precision. 

- **Hierarchy:** Use `display-lg` exclusively for marketing or empty-state hero sections. `headline-md` is the standard for dashboard titles and card headers.
- **Legibility:** Body text uses a standard 16px base (`body-md`) with generous line height (1.5x) to ensure comfort during heavy reading or task list scanning.
- **Labels:** Small labels use a semi-bold or bold weight to remain legible at 12px, especially when used for status badges or category tags.
- **Spacing:** For headings, use negative letter-spacing to maintain a tight, professional appearance.

## Layout & Spacing

This design system employs a **12-column fluid grid** for desktop and a **4-column grid** for mobile.

- **The 8pt Rhythm:** All spacing (padding, margins, heights) must be a multiple of 8px. Use 4px for fine-tuning small components like labels or icon pairings.
- **Safe Zones:** Content containers should maintain a max-width of 1280px on desktop to prevent excessive line lengths.
- **Density:** For task lists, use "Comfortable" vertical padding (16px). For data-heavy tables, a "Compact" mode (8px) can be toggled.
- **Responsive reflow:** On tablet, 3-column card layouts should collapse to 2 columns. On mobile, all columns stack vertically with a 16px side margin.

## Elevation & Depth

Hierarchy is established through **Tonal Layering** and **Soft Ambient Shadows**.

- **Surface Levels:** 
    - Level 0: `#F8FAFC` (Global Page Background)
    - Level 1: `#FFFFFF` (Cards, Modals, Sidebar)
- **Shadows:** Avoid harsh black shadows. Use `Action Blue` or `Slate` tints for shadow colors to keep the UI "light."
    - **Low:** `0 1px 3px 0 rgba(15, 23, 42, 0.1), 0 1px 2px -1px rgba(15, 23, 42, 0.1)` (Static Cards)
    - **Medium:** `0 10px 15px -3px rgba(15, 23, 42, 0.08)` (Hover states, Dropdowns)
    - **High:** `0 20px 25px -5px rgba(15, 23, 42, 0.1)` (Modals, Authentication Dialogs)
- **Outlines:** Use 1px borders in `#E2E8F0` for all UI containers to define boundaries on white surfaces without relying solely on shadows.

## Shapes

The shape language is **Rounded**, striking a balance between modern friendliness and professional structure.

- **Standard Elements:** 8px (`0.5rem`) radius for buttons, input fields, and small cards.
- **Large Containers:** 16px (`1rem`) radius for main dashboard widgets and modals.
- **Interactive Elements:** Checkboxes use a 4px radius, while radio buttons remain fully circular.
- **Status Badges:** Use a "Pill" shape (999px radius) to differentiate them from interactive buttons.

## Components

### Buttons
- **Primary:** Background `#2563EB`, Text `#FFFFFF`. 8px corner radius. On hover, darken to `#1D4ED8`.
- **Secondary:** Background `#F1F5F9`, Text `#1E293B`. Subtle border `#E2E8F0`. 
- **Ghost:** No background, Text `#64748B`. For low-priority actions like "Cancel."

### Input Fields
- **Idle State:** 1px border `#E2E8F0`, Background `#FFFFFF`, 8px radius.
- **Focus State:** 2px border `#2563EB`, with a soft blue outer glow (4px spread, 10% opacity).
- **Validation:** Error states use `#EF4444` for borders and helper text.

### Cards & Task Items
- White background (`#FFFFFF`) with a 1px border (`#E2E8F0`). 
- Include a 4px vertical "Category Stripe" on the left edge of task cards to indicate priority levels or project colors.

### Authentication Specifics
- **Auth Modals:** Centralized with `High` elevation. Large 1.25rem padding.
- **Social Auth:** Use standardized icons with neutral borders to keep the focus on the primary platform branding.

### Feedback Elements
- **Chips/Badges:** Small font (`label-sm`), pill-shaped. Backgrounds should be 10-15% opacity versions of their respective functional colors (e.g., light blue background for "In Progress").