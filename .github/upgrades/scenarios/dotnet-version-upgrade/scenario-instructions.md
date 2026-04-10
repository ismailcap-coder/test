# BuergerPortal .NET Version Upgrade

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net8.0
- **Commit Strategy**: After Each Task

## Source Control
- **Source Branch**: copilot/update-dotnet-to-version-8-again
- **Working Branch**: copilot/update-dotnet-to-version-8-again

## Strategy
**Selected**: Bottom-Up (Dependency-First)
**Rationale**: 5 projects on .NET Framework 4.6.2 — Bottom-Up non-negotiable per framework-migration.md rules. 4-tier dependency graph: Domain → Data → Business → Web/Tests.

### Execution Constraints
- Strict tier ordering: Tier N must complete before Tier N+1 begins
- SDK-style conversion is a separate task from TFM upgrade — never merged
- All projects upgraded in-place (no side-by-side web migration)
- Between-tier: after each tier, verify solution still compiles before proceeding
- Final validation: full solution build + test suite must pass

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- Upgrade Strategy: Bottom-Up (Dependency-First)

### Project Structure
- Project Approach: In-place rewrite (Web), In-place (Class Libraries)
- Package Management: Migrate to PackageReference

### Compatibility
- Unsupported Packages: Replace with compatible alternatives
- Unsupported API Handling: Migrate to .NET 8 equivalents

### Modernization
- Configuration Migration: Migrate to appsettings.json
- Dependency Injection: Use built-in ASP.NET Core DI
- Entity Framework: Migrate to EF Core 8 + Npgsql
- Nullable Reference Types: Enable (warnings only)
- Assembly Binding Redirects: Remove
