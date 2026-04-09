# BuergerPortal — ASP.NET MVC 4 Citizen Services Application

A comprehensive ASP.NET MVC 4 application on .NET Framework 4.5 for managing citizen service applications in German public offices. Designed as a realistic migration test target with version-specific technology features.

## Technology Stack (Version-Specific)

| Component | Version | Key Version-Specific Features |
|---|---|---|
| .NET Framework | 4.5 | Classic runtime, no .NET Core |
| ASP.NET MVC | 4.0 | `@Html.BeginForm`, `@Html.TextBoxFor`, `BundleConfig`, no Tag Helpers |
| Entity Framework | 5.0 (Code First) | `Database.SetInitializer`, `CreateDatabaseIfNotExists`, no async, no `HasIndex`, no `AddOrUpdate` |
| Npgsql | 2.2.7 | Legacy PostgreSQL provider via `DbProviderFactories` in Web.config |
| MSTest | v1 | `[TestClass]`, `[TestMethod]`, `[ExpectedException]` — no `[DataRow]`, no `Assert.ThrowsException` |
| Moq | 4.2 | Mock-based unit testing |
| jQuery | 1.8.2 | Bundled via `BundleConfig` |
| Project Format | Classic .csproj | `packages.config`, not SDK-style |

## Architecture

```
BuergerPortal.sln
├── BuergerPortal.Domain       — Entity classes, Enums (no dependencies)
├── BuergerPortal.Data         — EF5 DbContext, Repositories, DB Initializer
├── BuergerPortal.Business     — Services, Fee Calculation, Validators
├── BuergerPortal.Web          — MVC4 Controllers, Razor Views, ViewModels
└── BuergerPortal.Tests        — MSTest v1 + Moq unit tests
```

## Domain Model

- **Citizen** — Registered citizens with TaxId, address, low-income status
- **PublicOffice** — Government offices with district codes and fee multipliers
- **ServiceType** — Available services (Registration, Permit, License, Document, Certificate)
- **ServiceApplication** — Application with full status workflow
- **ApplicationDocument** — Uploaded documents per application
- **AuditLog** — Status change audit trail
- **FeeSchedule** — District-specific fee overrides

### Application Status Workflow

```
Draft → Submitted → UnderReview → Approved
                              ↘ Rejected
                              ↘ DocumentsRequested ↔ UnderReview
```

## Business Rules

### Fee Calculation Formula

```
1. adjustedFee = BaseFee × DistrictMultiplier
2. If express:        adjustedFee += adjustedFee × 0.50  (50% surcharge)
3. If low-income:     adjustedFee -= adjustedFee × 0.30  (30% discount)
4. If 3+ active apps: adjustedFee -= adjustedFee × 0.10  (10% multi-service discount)
5. adjustedFee += adjustedFee × 0.19  (19% VAT)
6. Minimum fee: EUR 5.00
```

### Age Restrictions

- **License**: 18+ years
- **Building Permit**: 21+ years
- **Marriage Certificate**: 16+ years

### Validation Rules

- TaxId: exactly 11 digits
- PostalCode: exactly 5 digits (German format)
- Email: standard email format (optional field)
- Date of Birth: must not be in the future

## Prerequisites

- Docker Desktop (for PostgreSQL)
- Visual Studio 2012/2013 with .NET Framework 4.5
- NuGet Package Manager

## Quick Start

### 1. Start PostgreSQL Database

```bash
cd input/sample-aspnet-mvc4-buergerportal
docker-compose up -d
```

This starts PostgreSQL 13 on **port 5434** with:
- Database: `buergerportal`
- User: `postgres`
- Password: `postgres`

### 2. Restore NuGet Packages

Open `BuergerPortal.sln` in Visual Studio and restore NuGet packages, or run:

```bash
nuget restore BuergerPortal.sln
```

### 3. Build and Run

1. Set `BuergerPortal.Web` as the startup project
2. Press F5 to build and run
3. The database will be auto-created and seeded on first run via `BuergerPortalInitializer`

### 4. Run Tests

Open Test Explorer in Visual Studio and run all tests, or use:

```bash
mstest /testcontainer:BuergerPortal.Tests\bin\Debug\BuergerPortal.Tests.dll
```

## Seed Data

The `BuergerPortalInitializer` automatically seeds:

- **5 Public Offices**: Berlin-Mitte, Munich-Central, Hamburg-Altona, Frankfurt-Zentrum, Cologne-Innenstadt
- **7 Service Types**: Residence Registration, Building Permit, Driver License, Birth Certificate, Marriage Certificate, Business License, ID Card Renewal
- **Fee Schedules**: Per-district fee overrides
- **5 Citizens**: Sample citizen records
- **2 Applications**: Sample applications in different statuses

## Connection String

Configured in `BuergerPortal.Web/Web.config`:

```xml
<connectionStrings>
  <add name="BuergerPortalContext"
       connectionString="Server=localhost;Port=5434;Database=buergerportal;User Id=postgres;Password=postgres;"
       providerName="Npgsql" />
</connectionStrings>
```

## Test Coverage

| Test Class | Tests | Coverage Area |
|---|---|---|
| CitizenServiceTests | 13 | CRUD, duplicate TaxId, delete protection, search |
| ApplicationServiceTests | 14 | Create, submit, review, approve, reject, document request workflow |
| FeeCalculationServiceTests | 16 | Full formula, express, low-income, multi-service, minimum fee, VAT |
| ApplicationValidatorTests | 15 | Status transitions, age restrictions, required fields |
| CitizenValidatorTests | 18 | TaxId format, postal code, email, date of birth, multiple errors |

**Total: 76 unit tests** covering all business logic and validation rules.

## Migration-Relevant Features

This application was specifically designed with version-specific features that require migration attention:

### EF5-Specific
- `Database.SetInitializer<T>()` pattern (replaced by `DbContext.Database.EnsureCreated()` in EF Core)
- `CreateDatabaseIfNotExists<T>` initializer
- Synchronous-only API (no `async`/`await` on DB operations)
- `DbProviderFactories` registration in Web.config
- Fluent API in `OnModelCreating` without `.HasIndex()`

### MVC4-Specific
- `@Html.BeginForm` / `@Html.TextBoxFor` helpers (replaced by Tag Helpers in MVC6+)
- `@Styles.Render` / `@Scripts.Render` (BundleConfig, replaced by bundler tools)
- `Global.asax` / `Application_Start` lifecycle
- `FilterConfig` / `RouteConfig` in `App_Start`
- `HandleErrorAttribute` global filter

### MSTest v1-Specific
- `[ExpectedException(typeof(...))]` attribute (replaced by `Assert.ThrowsException<>()` in v2)
- No `[DataRow]` / `[DataTestMethod]` parameterized tests
- `Microsoft.VisualStudio.QualityTools.UnitTestFramework` assembly

### Classic Project Format
- `packages.config` (replaced by `<PackageReference>` in SDK-style)
- `AssemblyInfo.cs` (auto-generated in SDK-style)
- Explicit `<Compile Include>` items
- `ProjectTypeGuids` for project type identification
