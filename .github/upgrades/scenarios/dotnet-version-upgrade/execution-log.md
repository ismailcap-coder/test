
## [2026-04-10 07:08] 01-prerequisites

.NET 8 SDK confirmed available (8.0.125-8.0.419). No global.json conflicts. Clean working branch. Ready to begin SDK-style conversion.


## [2026-04-10 07:10] 02-sdk-conversion

Converted all 5 projects to SDK-style format using convert_project_to_sdk_style tool (sequential, dependency order). Removed packages.config (3 files), AssemblyInfo.cs (5 files), and App.config (1 file). All projects still target net462 — TFM changes follow in subsequent tasks.


## [2026-04-10 07:13] 03-domain

BuergerPortal.Domain upgraded to net8.0. Removed legacy framework references. Enabled Nullable and ImplicitUsings. Fixed 31 CS8618 nullable warnings across all 7 entity files by annotating optional properties as nullable and required EF navigation properties with null-forgiving operator. Build: 0 errors, 0 warnings.


## [2026-04-10 07:20] 04-data

BuergerPortal.Data upgraded to net8.0 with EF Core 8.0.25 + Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11. Rewrote BuergerPortalContext for EF Core (DbContextOptions, ModelBuilder, WithOne/IsRequired). Replaced EF6 initializer with async BuergerPortalSeeder. Updated all 4 repositories: replaced System.Data.Entity with Microsoft.EntityFrameworkCore, fixed nullable return types, replaced raw SQL (Database.SqlQuery) with LINQ. Build: 0 errors, 0 warnings.


## [2026-04-10 07:22] 05-business

BuergerPortal.Business upgraded to net8.0. Updated ICitizenService and CitizenService for nullable-safe return type on GetCitizenByTaxId. Guarded nullable TaxId dereferences. Build: 0 errors, 0 warnings.


## [2026-04-10 07:33] 06-web

BuergerPortal.Web upgraded to net8.0 ASP.NET Core MVC. New Program.cs with DI registration, routing, and database seeding. appsettings.json replaces Web.config. _ViewImports.cshtml added. Deleted Global.asax, Web.config, App_Start folder. All 4 controllers updated to use ASP.NET Core MVC + constructor DI. ViewModels updated for nullable safety. Layout updated to remove bundling helpers. Build: 0 errors, 0 warnings.


## [2026-04-10 07:45] 07-tests

BuergerPortal.Tests upgraded to net8.0 with MSTest 4.2.1 + Moq 4.20.72. Replaced legacy QualityTools GAC reference. Converted all [ExpectedException] tests to Assert.ThrowsExactly<T> (MSTest v4 removed ThrowsException). Fixed all nullable warnings. Tests: 82 passed, 0 failed.


## [2026-04-10 07:46] 08-final-validation

Full solution build: 0 errors, 0 warnings across all 5 projects targeting net8.0. All 82 tests pass. Migration complete: .NET Framework 4.5 → .NET 8, ASP.NET MVC 4 → ASP.NET Core MVC, EF5 → EF Core 8, Npgsql 2.x → 8.x, MSTest v1 → v4.

