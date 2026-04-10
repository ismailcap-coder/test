# BuergerPortal .NET Version Upgrade Progress

## Overview

Upgrading 5 projects from .NET Framework 4.6.2 to .NET 8 using the Bottom-Up strategy. The solution is a classic ASP.NET MVC 4 citizen services portal (BuergerPortal) being modernized to ASP.NET Core MVC with EF Core 8 + Npgsql, SDK-style projects, and MSTest v3.

**Progress**: 8/8 tasks complete <progress value="100" max="100"></progress> 100%

## Tasks

- ✅ 01-prerequisites: Verify .NET 8 SDK and toolchain readiness ([Content](tasks/01-prerequisites/task.md), [Progress](tasks/01-prerequisites/progress-details.md))
- ✅ 02-sdk-conversion: Convert all 5 projects from old-style to SDK-style csproj ([Content](tasks/02-sdk-conversion/task.md), [Progress](tasks/02-sdk-conversion/progress-details.md))
- ✅ 03-domain: Upgrade BuergerPortal.Domain to net8.0 ([Content](tasks/03-domain/task.md), [Progress](tasks/03-domain/progress-details.md))
- ✅ 04-data: Upgrade BuergerPortal.Data to net8.0 with EF Core 8 and Npgsql ([Content](tasks/04-data/task.md), [Progress](tasks/04-data/progress-details.md))
- ✅ 05-business: Upgrade BuergerPortal.Business to net8.0 ([Content](tasks/05-business/task.md), [Progress](tasks/05-business/progress-details.md))
- ✅ 06-web: Upgrade BuergerPortal.Web to net8.0 with ASP.NET Core MVC ([Content](tasks/06-web/task.md), [Progress](tasks/06-web/progress-details.md))
- ✅ 07-tests: Upgrade BuergerPortal.Tests to net8.0 with MSTest v3 ([Content](tasks/07-tests/task.md), [Progress](tasks/07-tests/progress-details.md))
- ✅ 08-final-validation: Full solution build, test run, and documentation ([Content](tasks/08-final-validation/task.md), [Progress](tasks/08-final-validation/progress-details.md))
