# Upgrade Options — BuergerPortal

Assessment: 5 projects on .NET Framework 4.6.2, old-style csproj, packages.config, ASP.NET MVC 4 web project, EF6, MSTest v1

## Strategy

### Upgrade Strategy
.NET Framework multi-project solution (5 projects, 4-tier dependency graph) — Bottom-Up is non-negotiable per framework-migration.md planning rules.

| Value | Description |
|-------|-------------|
| **Bottom-Up (Dependency-First)** (selected) | Upgrade from leaf nodes to root applications, tier by tier; validates each layer independently |
| All-at-Once | Single atomic upgrade of all projects simultaneously |
| Top-Down | Applications first, add multi-targeting to libraries as needed |

## Project Structure

### Project Approach
All 5 projects target .NET Framework 4.6.2. The web project (BuergerPortal.Web) has 4 controllers — small project, user explicitly requested in-place rewrite (Replace ASP.NET MVC 4 with ASP.NET Core MVC). Class libraries have no remaining .NET Framework consumers after migration.

| Value | Description |
|-------|-------------|
| **In-place rewrite** (selected) | Web project: replace Framework web project entirely in one pass; Class Libraries: replace TFM directly |
| Side-by-side | Creates new ASP.NET Core project alongside old; injects scaffold/migrate tasks |

### Package Management
All projects use packages.config (old-style). SDK-style conversion migrates these to PackageReference automatically.

| Value | Description |
|-------|-------------|
| **Migrate to PackageReference** (selected) | Convert packages.config to PackageReference as part of SDK-style conversion |
| Keep packages.config | Leave package management as-is |

## Compatibility

### Unsupported Packages
Assessment found NuGet.0001 (incompatible packages) in BuergerPortal.Tests and BuergerPortal.Web — MSTest v1, old Moq, ASP.NET MVC 4, EF6 all incompatible with net8.0.

| Value | Description |
|-------|-------------|
| **Replace with compatible alternatives** (selected) | Replace MSTest v1→v3, Moq→latest, EF6→EF Core 8, ASP.NET MVC 4→ASP.NET Core MVC |
| Skip incompatible packages | Leave as-is, likely causes build failures |

### Unsupported API Handling
System.Web APIs, EF6 APIs (DbModelBuilder, Configuration property), and ASP.NET MVC 4 APIs are not available in .NET 8.

| Value | Description |
|-------|-------------|
| **Migrate to .NET 8 equivalents** (selected) | Replace all System.Web/EF6/MVC4 APIs with .NET 8 equivalents |
| Use Windows Compatibility Pack | Add compat shim — not sufficient for System.Web/MVC4 |

## Modernization

### Configuration Migration
Web.config present with connectionStrings, entityFramework, appSettings sections, and assembly binding redirects.

| Value | Description |
|-------|-------------|
| **Migrate to appsettings.json** (selected) | Convert connectionStrings and appSettings to appsettings.json; binding redirects removed (no longer needed) |
| Keep Web.config | Not supported in ASP.NET Core |

### Dependency Injection
No third-party DI container in use. ASP.NET Core has built-in DI. User requested ASP.NET Core startup (Program.cs).

| Value | Description |
|-------|-------------|
| **Use built-in ASP.NET Core DI** (selected) | Register services in Program.cs using Microsoft.Extensions.DependencyInjection |
| Add Autofac/other container | Third-party container not needed |

### Entity Framework
EF6 (EntityFramework 6.4.4) in use, user explicitly requested upgrade to EF Core with Npgsql provider.

| Value | Description |
|-------|-------------|
| **Migrate to EF Core 8 + Npgsql** (selected) | Replace EF6 DbContext/DbModelBuilder/initializer with EF Core equivalents; Fluent API ported to OnModelCreating |
| Keep EF6 | EF6 does not support net8.0 |

### Nullable Reference Types
Target is net8.0 and projects do not have nullable enabled.

| Value | Description |
|-------|-------------|
| **Enable (warnings only)** (selected) | Add `<Nullable>enable</Nullable>` — surfaces null-safety warnings without forcing fixes |
| Disable | Leave nullable disabled |
| Enable (errors) | Treat nullable warnings as errors |

### Assembly Binding Redirects
Web.config contains assemblyBinding entries for System.Web.WebPages, System.Web.Mvc, WebGrease. These are .NET Framework artifacts.

| Value | Description |
|-------|-------------|
| **Remove** (selected) | Binding redirects are not needed or supported in .NET 8; remove entirely |
| Keep | Cannot be used in .NET 8 |
