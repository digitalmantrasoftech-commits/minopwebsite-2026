# Page-by-page design review

The user requested individual page previews on 9 October 2026. Each screen will be implemented and presented in sequence. Shared theme coverage is not equivalent to individual page completion.

## Page 1: Login

Source: `Views/PayTime/LoginPage.cshtml`.
Styles: `app_assets/css/Login_Custom.css`.
Preview: `docs/login-preview.html` (self-contained, no network or authentication requests).

A full-page split layout pairs an edge-to-edge workplace photo with an uncluttered sign-in panel. Desktop uses the full viewport; mobile stacks the photo and form. The photo is AI-generated. Product branding, typography, account selection, inputs and focus indicators have been refined.

Existing IDs, forms, Razor branches, login/registration/OTP scripts and backend files are preserved. The latest update adds only presentation classes, a brand link and scoped CSS. The preview supports employee/admin selection and password visibility locally; no authentication requests are sent.

Validation: original IDs and embedded scripts match the preceding revision. Preview JavaScript syntax and whitespace checks passed. Browser rendering and IIS authentication checks remain pending.

## Remaining sequence

1. Admin dashboard (existing design and analytics require individual visual review)
2. Employee dashboard
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
