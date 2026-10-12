# Minop modern interface refinement

This change refines the existing shared theme with Minop blue, a neutral canvas, white surfaces, quieter shadows, readable Poppins typography, consistent controls, and clearer attendance metrics.

## Scope

- All six authenticated layouts load the final theme after their legacy head styles.
- The existing bundled theme files are included as project Content so MSBuild publishing copies them.
- Shared forms, tables, cards, navigation, toolbars, dialogs, and dashboard attendance widgets receive visual improvements.
- Primary navigation styles now target the actual clickable `li.menu-item` markup.
- Toolbar labels retain natural width; dashboard headings can wrap; cards and DevExpress popup containers no longer clip their menus.
- Keyboard focus, disabled/read-only controls, error states, and narrow-screen form readability are improved.
- Login, marketing, standalone pages and print/export templates are outside this refinement.

## Preserved application behavior

No controller, model, API, authentication, database, configuration or JavaScript source changed. All six Razor layout files are byte-identical to their previous versions after removing the single added stylesheet link. Existing IDs, names, event handlers, routes, conditional rendering, and form actions are therefore unchanged.

The only project-file change adds three existing CSS assets to the publish manifest. No compile items or dependencies change.

## Preview

Open `docs/ui-preview.html` through a static server at the repository root. It uses the real attendance widget markup and repository styles with explicitly labeled sample values. The preview has no backend connection. Its shell geometry and controls are illustrative and are not a substitute for authenticated application testing.

## Verification

Passed: source invariants for all changed Razor files, no backend/JavaScript/configuration edits, project XML parsing, theme asset existence, and git whitespace checks.

Not run: browser rendering (Playwright's browser download failed), .NET Framework 4.8 build, or authenticated IIS runtime checks. This environment has no .NET Framework runtime. Before merging, verify admin, employee, school, developer, device-admin and partner layouts at 1440, 1024, 768 and 390 pixels. Check menu open/close, long toolbar labels, Select2, validation, grid selection/paging, modal controls and attendance widget loading with actual data. Verify a Release publish contains all three theme stylesheets.

## Workforce insights extension

Added a frontend-only insights panel inside the existing `Activity-1` widget. It inherits that widget's authorization and visibility. Existing IDs and view code remain unchanged; new markup uses data attributes.

`MinopInsights.js` observes the existing jQuery `GetActivityMonitorCounts` responses for reqid 1, 6, 3 and 4. It issues no requests and never reads credentials. The visual check-in share is checked in / (checked in + not checked in), explicitly labeled as a share of reported counts, not productivity. Late arrivals and early departures remain separate because employees may overlap. Review actions trigger the existing employee-list controls.

The original request sequence starts each refresh with reqid 1. The adapter resets its state for that request and tracks exact request objects and company/branch scope, rejecting stale responses. Missing, invalid or failed counts display as unavailable; zero totals have no percentage. No historical trends or comparisons are generated from current-only counts.

Validation: `node tests/minop-insights.test.cjs`, JS syntax and whitespace checks passed. Existing dashboard Razor bytes are identical after removing the new markup and script tag; all original IDs match. No backend or configuration source changed. Browser and authenticated IIS checks remain pending. The new project Content entry publishes only the frontend JS asset. The statement above about no JavaScript changes applies to the first refinement; this extension adds a separate JS file without modifying existing JavaScript.
