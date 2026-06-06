# MinopCloud UI/UX Page Coverage

## Scope

This branch applies the approved clean SaaS visual system to the MinopCloud authenticated application without changing backend code, controller actions, APIs, database logic, session logic, form actions, Razor element IDs, or JavaScript event handlers.

Typography is restricted to **Poppins ExtraLight 200, Light 300, and Regular 400**.

## Verification method

The repository was audited at the view and layout level using:

1. `PayTimeWebClient/PayTimeWebClient.csproj` view inventory.
2. `Views/_ViewStart.cshtml` default-layout behavior.
3. Explicit `Layout = ...` declarations in Razor views.
4. Shared-layout stylesheet loading order.
5. Standalone full-document views using `Layout = null` and their page-specific CSS.
6. AJAX, partial, print, export, and embedded report views that must not be converted into full pages.

## Authenticated layout families verified

The following authenticated layout families load `~/bundles/layoutcss3`, whose final file is the new MinopCloud SaaS override stylesheet:

- `Views/Shared/_Layout.cshtml`
- `Views/Shared/_LayoutSuperAdmin.cshtml`
- `Views/Shared/_LayoutDevelopers.cshtml`
- `Views/Shared/_LayoutDeviceAdmin.cshtml`
- `Views/Shared/_LayoutSchool.cshtml`
- `Views/Shared/_PartnerLayout.cshtml`

The final authenticated design layer is:

- `assets/css/minop-dashboard-unicorn.css`

It loads after the legacy layout CSS and provides the highest-priority visual overrides.

## Authenticated module coverage

Views rendered through the authenticated layouts receive the new design system, including:

- Admin and employee dashboards
- Master data
- Employee management and onboarding
- Attendance and attendance correction
- Shift, schedule, roster, holiday, and leave management
- Payroll generation, approval, tax, increment, workflow, and salary modules
- ESS requests and approvals
- Reports, report dashboards, analytics, and DevExpress grids
- User management and role rights
- Device management and enrollment
- Notifications and alert configuration
- Field tracking and task management
- Expense and trip management
- Conference room booking
- School management
- Visitor management
- Canteen management
- OKR and PMS modules
- Business unit workflows
- TP onboarding and approval workflows
- Super-admin pages
- Developer and device-admin portals
- Partner/dealer authenticated pages

## Component coverage

The final authenticated UI system covers:

- Header and top navigation
- Primary and secondary sidebars
- Breadcrumbs and page toolbars
- Dashboard KPI cards
- Activity-monitor cards
- Chart containers and Highcharts typography
- Forms, inputs, selects, Select2, multiselect, and validation states
- Buttons and action toolbars
- Tables and DataTables
- DevExpress grids and report controls
- Tabs and pills
- Modals and dropdowns
- Status badges and alerts
- Calendars and schedulers
- Progress bars and wizards
- Profile, report, payroll, leave, ESS, task, visitor, meal, expense, policy, OKR, and room cards
- Empty states, loaders, and responsive behavior

## Dashboard-specific verification

The actual admin dashboard view is:

- `Views/Dashboard/Admindashboard.cshtml`

Its legacy classes were specifically covered, including:

- `card_box`
- `card_header`
- `card_summarylist`
- `activity_block`
- `activity_count`
- `date_box`
- `filter_portlet_wrapper`
- `filter_portlet_tbl`
- `dashboard-sortable-row`
- Highcharts dashboard container IDs

## Standalone full-document pages

Standalone pages do not inherit an authenticated layout. They are covered through page-family CSS or direct page updates.

### Login

- `Views/PayTime/LoginPage.cshtml`
- `app_assets/css/Login_Custom.css`

### Password recovery, OTP, and reset

Covered through:

- `Newlayout/css/forgotpassword.css`

This family includes pages such as:

- Admin forgot password
- Employee forgot password
- OTP verification
- ESS reset password

### Dealer/partner entry pages

Covered through:

- `assets/Dealer/dealercustom.css`

This family includes:

- Dealer login
- Dealer forgot password
- Dealer registration/activation screens using the same stylesheet

### Error pages

Directly redesigned or covered by the standalone design layer:

- `Views/Shared/Error.cshtml`
- `Views/Error/Unauthorized.cshtml`
- `Views/Shared/_layout404error.cshtml`
- `Views/Error/Fatal.cshtml`
- `assets/css/minop-standalone-v2.css`

## AJAX and partial views

Views beginning with `_`, data-grid fragments, report partials, modal fragments, and pages returned into an existing layout container were intentionally **not converted into standalone pages**.

They inherit the new styling from the parent authenticated page for:

- Tables
- Forms
- Cards
- Statuses
- Buttons
- Report grids
- DevExpress controls

This preserves AJAX rendering and JavaScript behavior.

## Print and export views

Print, PDF, Excel, bank-export, raw-report, and download-format views are intentionally not forced into the interactive SaaS shell because doing so can break printable dimensions and exported output.

Their source application pages and filters receive the new UI, while the generated print/export output preserves its functional formatting.

## Backend safety confirmation

No backend application files were modified for this redesign:

- No controllers
- No models
- No services or REST clients
- No API URLs
- No database or migration files
- No session or authentication logic
- No form actions
- No Razor input names or element IDs
- No JavaScript event handlers

## Runtime QA requirement

Repository-level coverage is complete. Final visual verification still requires running the application and testing role-specific routes because menus and pages are dynamically controlled by:

- User role
- Company plan
- Company configuration
- Product access
- Database content
- API responses

Recommended runtime QA roles:

1. Admin
2. Employee/ESS
3. Super Admin
4. Developer
5. Device Admin
6. School Admin
7. Partner/Dealer

Recommended viewport checks:

- 1440px desktop
- 1024px tablet landscape
- 768px tablet
- 390px mobile

## Branch

`ui-ux-admin-saas-redesign`

## Pull request

PR #1: Clean SaaS UI refresh for authenticated admin pages
