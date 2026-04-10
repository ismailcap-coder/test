# Progress Details: 06-web

## What Changed

### Project File
- `BuergerPortal.Web.csproj`: net462→net8.0, removed all incompatible packages (Antlr, EF6, jQuery, ASP.NET MVC 4, WebGrease, Npgsql 2.x, etc.), kept SDK.Web, added Nullable+ImplicitUsings

### New Files
- `Program.cs`: ASP.NET Core startup replacing Global.asax.cs — registers DbContext with Npgsql, all repositories/validators/services via DI, configures MVC routing, seeds database on startup
- `appsettings.json`: PostgreSQL connection string, logging config
- `appsettings.Development.json`: Development logging config
- `Views/_ViewImports.cshtml`: Tag helpers, using statements for namespaces

### Deleted Files
- `Global.asax` and `Global.asax.cs`
- `Web.config` and `Views/Web.config`
- `App_Start/RouteConfig.cs`, `FilterConfig.cs`, `BundleConfig.cs`

### Updated Controllers
- `HomeController.cs`: System.Web.Mvc→Microsoft.AspNetCore.Mvc; constructor uses DI for BuergerPortalContext; ActionResult→IActionResult; removed Dispose override
- `CitizenController.cs`: Same DI pattern; ICitizenService injected; nullable fixes with `?? string.Empty`
- `ApplicationController.cs`: DI for IApplicationService + BuergerPortalContext; User.Identity?.Name null-safe; viewModel.Notes ?? string.Empty
- `ReportController.cs`: BuergerPortalContext via DI; removed Dispose

### Updated ViewModels
- `CitizenViewModel.cs`: Required strings use `= string.Empty`; optional strings (PhoneNumber, Email) use `string?`
- `ApplicationViewModel.cs`: Optional strings (ApplicationNumber, Notes, RejectionReason, display props) use `string?`; dropdown collections use `IEnumerable<SelectListItem>?`
- `DashboardViewModel.cs`: Display-only strings use `string?`; StatusSummaryItem.StatusName uses `= string.Empty`

### Updated Views
- `Views/Shared/_Layout.cshtml`: Removed @Styles.Render/@Scripts.Render bundling; added direct link/script tags
- `Views/Application/Index.cshtml`: Fixed nullable cast `(string?)null`

## Validation
- `dotnet build BuergerPortal.Web` → 0 errors, 0 warnings ✅
