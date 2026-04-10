# BuergerPortal Upgrade Plan — .NET Framework 4.6.2 → .NET 8

### Selected Strategy
**Bottom-Up (Dependency-First)** — Upgrade from leaf nodes to root applications, tier by tier.
**Rationale**: 5 projects with 4-tier dependency graph. All projects on .NET Framework — Bottom-Up is required per framework-migration planning rules.

## Dependency Graph

```
Tier 4: [BuergerPortal.Web]  [BuergerPortal.Tests]
               ↓                      ↓
Tier 3:   [BuergerPortal.Business]
               ↓
Tier 2:   [BuergerPortal.Data]
               ↓
Tier 1:   [BuergerPortal.Domain]
```

---

## Tasks

### 01-prerequisites: Verify .NET 8 SDK and toolchain readiness

Ensure the environment has the .NET 8 SDK installed and that the solution is in a clean state before starting the upgrade. This includes checking global.json compatibility, confirming the SDK version, and establishing a clean baseline.

**Done when**: `dotnet --version` confirms .NET 8 SDK is available; no global.json version conflicts exist.

---

### 02-sdk-conversion: Convert all 5 projects from old-style to SDK-style csproj

All 5 projects (Domain, Data, Business, Tests, Web) use the old-style project format with `ToolsVersion` attributes, explicit `<Compile>` item lists, and `packages.config` files. This task converts all projects to SDK-style format while staying on the current .NET Framework TFM (net462). This is a structural-only change: no TFM or API changes happen here.

Key concerns: removing explicit `<Compile>` entries (SDK-style auto-discovers), migrating `packages.config` to `PackageReference`, removing old `<Import>` targets (Microsoft.CSharp.targets, Microsoft.WebApplication.targets), removing AssemblyInfo.cs files (SDK auto-generates), and ensuring project GUIDs and solution references remain intact.

The Web project has additional complexity: `ProjectTypeGuids` for ASP.NET MVC must be cleaned up, and `packages.config` references to EF6, ASP.NET MVC 4, WebGrease, Antlr, etc., must be migrated to PackageReference. After conversion, verify all projects still build on net462.

**Done when**: All projects use SDK-style format; `packages.config` files removed; `AssemblyInfo.cs` files removed; solution builds (possibly with warnings) on current TFM.

---

### 03-domain: Upgrade BuergerPortal.Domain to net8.0

BuergerPortal.Domain is the leaf node of the dependency graph (Tier 1) with zero project references. It contains only domain entities (Citizen, ServiceApplication, PublicOffice, ServiceType, ApplicationDocument, FeeSchedule, AuditLog) and enums. The only framework reference is `System.ComponentModel.DataAnnotations`, which is available in .NET 8 via built-in assemblies.

This task changes the TFM from `net462` to `net8.0`, enables Nullable reference types (`<Nullable>enable</Nullable>`), and verifies the project builds cleanly. No package updates are needed — the project has no NuGet dependencies.

**Done when**: BuergerPortal.Domain targets net8.0 and builds without errors.

---

### 04-data: Upgrade BuergerPortal.Data to net8.0 with EF Core 8 and Npgsql

BuergerPortal.Data is Tier 2 (depends only on Domain). It uses EF6 (EntityFramework 6.4.4) with a SQL Server connection string in Web.config and App.config — but the application description specifies PostgreSQL via Npgsql. This task upgrades the TFM to net8.0 and replaces EF6 with EF Core 8 + Npgsql.EntityFrameworkCore.PostgreSQL.

Major API changes required: `DbContext` base class constructor changes (no more string-name connection, use `DbContextOptions`), remove `Configuration.LazyLoadingEnabled/ProxyCreationEnabled` properties (configure via `optionsBuilder`), replace `DbModelBuilder` with `ModelBuilder` in `OnModelCreating`, remove `System.Data.Entity.EntityState` references (use `Microsoft.EntityFrameworkCore.EntityState`), remove `BuergerPortalInitializer` (EF Core uses Migrations or `EnsureCreated`/seed instead), update all repository `System.Data.Entity` using statements.

The `CitizenRepository.Update` method uses `_context.Entry(entity).State = EntityState.Modified` which remains valid in EF Core. The `Include` calls on navigation properties remain syntactically compatible. Remove `App.config` entirely (connection string will come from appsettings.json via DI).

**Done when**: BuergerPortal.Data targets net8.0; EF Core 8 + Npgsql packages referenced; DbContext works with DbContextOptions; all repositories compile; App.config removed.

---

### 05-business: Upgrade BuergerPortal.Business to net8.0

BuergerPortal.Business is Tier 3 (depends on Domain and Data). It contains service classes (CitizenService, ApplicationService, FeeCalculationService) and validators, all using pure C# without any .NET Framework-specific APIs. The services depend on concrete repository types (CitizenRepository, ServiceApplicationRepository, etc.) rather than the IRepository interface.

This task changes the TFM from net462 to net8.0 and enables Nullable reference types. No package changes are needed (the project has no NuGet dependencies). Verify that any string formatting using `string.Format` is still valid (it is in .NET 8), and that the exception handling and validation patterns compile cleanly.

**Done when**: BuergerPortal.Business targets net8.0 and builds without errors.

---

### 06-web: Upgrade BuergerPortal.Web to net8.0 with ASP.NET Core MVC

BuergerPortal.Web is Tier 4 (depends on Domain, Data, and Business). This is the most complex task — it replaces ASP.NET MVC 4 (System.Web.Mvc) with ASP.NET Core MVC, adds Program.cs, converts Web.config to appsettings.json, replaces Global.asax startup with ASP.NET Core middleware pipeline, and registers all services via built-in DI.

**Framework changes**:
- Remove Global.asax / Global.asax.cs → replace with Program.cs using `WebApplication.CreateBuilder`
- Remove Web.config (top-level and Views/Web.config) → replace with appsettings.json
- Remove App_Start/ files (RouteConfig, FilterConfig, BundleConfig) → integrate into Program.cs
- Replace `System.Web.Mvc.Controller` with `Microsoft.AspNetCore.Mvc.Controller`
- Replace `ActionResult` with `IActionResult` where appropriate
- Replace `System.Web.Mvc.SelectListItem` with `Microsoft.AspNetCore.Mvc.Rendering.SelectListItem`
- Remove all `System.Web.*` using statements

**DI registration**: Register `BuergerPortalContext` (with DbContextOptions from appsettings.json), all repositories and services in Program.cs. Update controllers to receive dependencies via constructor injection instead of instantiating them directly.

**View changes**: Update `_Layout.cshtml` — replace `@Styles.Render()`/`@Scripts.Render()` bundling helpers with direct `<link>`/`<script>` tags. Update `_ViewImports.cshtml` (create if not present). Views using `@Html.ActionLink`, `@Html.BeginForm`, `@Html.TextBoxFor`, etc. remain compatible with ASP.NET Core Tag Helpers or HtmlHelpers.

**Package changes**: Remove ASP.NET MVC 4, WebGrease, Antlr3, Microsoft.Web.Infrastructure, System.Web.Optimization. Add `Microsoft.AspNetCore.Mvc` (included in `Microsoft.NET.Sdk.Web`). Keep Newtonsoft.Json if used, or switch to System.Text.Json.

**Done when**: BuergerPortal.Web targets net8.0; builds and runs as ASP.NET Core MVC app; all controllers compile with DI; appsettings.json contains connection string; no System.Web references remain.

---

### 07-tests: Upgrade BuergerPortal.Tests to net8.0 with MSTest v3

BuergerPortal.Tests is Tier 4 (depends on Domain, Data, and Business). It uses MSTest v1 (Microsoft.VisualStudio.QualityTools.UnitTestFramework 10.0.0.0 — referenced as assembly, not NuGet package) with Moq 4.2.x and Castle.Core 3.3.x.

This task replaces the GAC-referenced MSTest v1 assembly with `MSTest.TestFramework` v3 NuGet package (and `MSTest.TestAdapter`), upgrades Moq to the latest 4.x version, and removes the legacy Castle.Core reference (Moq now bundles its own Castle.Core). Test classes use `[TestClass]`, `[TestMethod]`, `[TestInitialize]`, `[ExpectedException]` attributes — all remain valid in MSTest v3. The `Assert` class API is the same.

Key change: The tests mock concrete classes (`Mock<CitizenRepository>`, `Mock<ServiceApplicationRepository>`) using `MockBehavior.Loose` with null context constructor args. This pattern still works with Moq 4.x and MSTest v3 without changes.

**Done when**: BuergerPortal.Tests targets net8.0; MSTest v3 NuGet packages referenced; all tests compile and pass.

---

### 08-final-validation: Full solution build, test run, and documentation

Run the full solution build (`dotnet build`) and the complete test suite (`dotnet test`) to confirm all projects work together. Resolve any remaining issues found during integration. Update the README to reflect the new .NET 8 / ASP.NET Core technology stack, and document any deferred items or known post-upgrade steps.

**Done when**: `dotnet build BuergerPortal.sln` succeeds with zero errors and zero warnings; `dotnet test` passes all tests; README updated.
