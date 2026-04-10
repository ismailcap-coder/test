# Progress Details: 02-sdk-conversion

## What Changed
- Converted all 5 projects from old-style to SDK-style format using `convert_project_to_sdk_style` tool
- Removed `packages.config` from BuergerPortal.Tests, BuergerPortal.Data, BuergerPortal.Web
- Removed `AssemblyInfo.cs` from all 5 projects' Properties/ folders
- Removed `App.config` from BuergerPortal.Data

## Converted Projects (dependency order)
1. BuergerPortal.Domain → SDK-style, net462 (leaf node)
2. BuergerPortal.Data → SDK-style, net462, EF6 as PackageReference
3. BuergerPortal.Business → SDK-style, net462
4. BuergerPortal.Tests → SDK-style, net462, Moq/Castle.Core as PackageReference, MSTest v1 still as GAC reference
5. BuergerPortal.Web → SDK-style, net462, all old packages as PackageReference

## Validation
- All projects converted successfully
- packages.config files removed from all projects
- AssemblyInfo.cs files removed (SDK auto-generates)
- Note: Projects still target net462 — TFM changes happen in subsequent tasks
