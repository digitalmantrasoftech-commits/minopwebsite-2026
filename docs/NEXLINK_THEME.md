# Nexlink visual adaptation

Reference inspected: https://nexlink.layoutdrop.com/demo/index.html and its styles.css on 12 October 2026.

Instrument Sans (400–700), primary #5955D1, headings #0C243C, body #696981, canvas #FDFDFF, borders #EEEEF3. Original compatibility CSS adapts this direction to Minop; no vendor template code or plugins are imported. Google Fonts supplies the font with a sans-serif fallback.

The screen-only theme covers shared navigation, cards, controls, tables, dialogs, dashboard insights and login. All existing IDs, scripts and source content are preserved; each entry point adds only a body class and stylesheet link. Existing framework, chart engines and backend remain unchanged. Print/export documents are excluded.

This is shared theme coverage, not completed visual QA of every application page. Authenticated browser, chart and widget testing is still required; no .NET runtime is available here. Preview data is illustrative.

## Updated entry points

- `PayTimeWebClient/Views/PayTime/DevelopersAccount.cshtml`
- `PayTimeWebClient/Views/PayTime/ActiveUser.cshtml`
- `PayTimeWebClient/Views/PayTime/ModuleSelection.cshtml`
- `PayTimeWebClient/Views/PayTime/LoginPage.cshtml`
- `PayTimeWebClient/Views/PayTime/OTPCheck.cshtml`
- `PayTimeWebClient/Views/PayTime/ForgotPasswordForEmployee.cshtml`
- `PayTimeWebClient/Views/PayTime/DeveloperActiveUser.cshtml`
- `PayTimeWebClient/Views/PayTime/ForgotPassword.cshtml`
- `PayTimeWebClient/Views/PayTime/CustomIndex.cshtml`
- `PayTimeWebClient/Views/PayTime/DevelopersPassword.cshtml`
- `PayTimeWebClient/Views/Shared/_PartnerLayout.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutLandingAPI.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutLandingCloud.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutLandingEdtech.cshtml`
- `PayTimeWebClient/Views/Shared/_Layout.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutDeviceAdmin.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutDevelopers.cshtml`
- `PayTimeWebClient/Views/Shared/Error.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutSchool.cshtml`
- `PayTimeWebClient/Views/Shared/_layout404error.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutLandingPage.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutSuperAdmin.cshtml`
- `PayTimeWebClient/Views/Shared/_LayoutLandingPayroll.cshtml`
- `PayTimeWebClient/Views/Dealer/ActiveDealer.cshtml`
- `PayTimeWebClient/Views/Dealer/ForgotPassword.cshtml`
- `PayTimeWebClient/Views/Dealer/Login.cshtml`
- `PayTimeWebClient/Views/Error/Unauthorized.cshtml`
- `PayTimeWebClient/Views/School/Index.cshtml`
- `PayTimeWebClient/Views/ESS/ResetPassword.cshtml`
