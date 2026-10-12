# Page-by-page design review

The user requested individual page previews on 9 October 2026. Each screen will be implemented and presented in sequence. Shared theme coverage is not equivalent to individual page completion.

## Page 1: Login

Source: `Views/PayTime/LoginPage.cshtml`.
Styles: `app_assets/css/Login_Custom.css`.
Preview: `docs/login-preview.html` (self-contained, no network or authentication requests).

A full-page split layout pairs an edge-to-edge workplace photo with an uncluttered sign-in panel. Desktop uses the full viewport; mobile stacks the photo and form. The photo is AI-generated. Product branding, typography, account selection, inputs and focus indicators have been refined.

Existing IDs, forms, Razor branches, login/registration/OTP scripts and backend files are preserved. The latest update adds only presentation classes, a brand link and scoped CSS. The preview supports employee/admin selection and password visibility locally; no authentication requests are sent.

Validation: original IDs and embedded scripts match the preceding revision. Preview JavaScript syntax and whitespace checks passed. Browser rendering and IIS authentication checks remain pending.

## Page 2: Admin Dashboard

Source: `Views/Dashboard/Admindashboard.cshtml` (the Dashboard route).
Styles: `assets/css/minop-admin-page.css`, loaded after the existing view styles and included in the publish manifest.
Preview: `docs/admin-dashboard-preview.html`.

Refined widget cards, six responsive attendance tiles, check-in insights, labeled Customize/Filters controls, readable widget headings and filter stacking. Existing widget visibility, sorting, charts, request handlers and permissions are preserved. No historical analytics were added; existing current-count insights remain in place.

All original IDs and embedded scripts match the prior revision. Backend and existing JS files are unchanged. Project XML and whitespace checks passed. Preview uses sample data and local controls; its simplified chart and shell are illustrative. Browser rendering, actual chart sizing, filter behavior and authenticated IIS testing remain pending. Other admin routes remain queued for separate review.

## Page 3: Employee Dashboard

Source: `Views/Dashboard/EmployeeDashboard.cshtml`.
Styles: `assets/css/minop-employee-page.css`.
Preview: `docs/employee-dashboard-preview.html` (sample values; no attendance submissions).

Compact Nexlink-style cards, clearer attendance times, responsive request tabs, leave balance tracks and calendar controls. Existing sortable columns, visibility, authorization, check-in/out behavior and plugins remain intact. The view differs only by two classes and one stylesheet link; original IDs/scripts match exactly. The stylesheet is included in the publish manifest. JS syntax, XML and whitespace checks passed; authenticated browser checks remain pending. Preview shell and sample calendar illustrate the design and do not run the original calendar plugin.

## Remaining sequence

3. Employee directory and employee master
4. Onboarding and employee details
5. Attendance and correction
6. Leave and approvals
7. Shift, roster and holiday management
8. Payroll
9. Reports and analytics
10. Devices and enrollment
11. User roles, settings and notifications
12. Other application modules and standalone account screens

`page-review-inventory.json` records all 475 Razor views as a preliminary queue. Partials, layout templates, marketing pages and exports require classification; they must not be represented as standalone app screens or redesigned blindly. No remaining page is marked individually complete.
