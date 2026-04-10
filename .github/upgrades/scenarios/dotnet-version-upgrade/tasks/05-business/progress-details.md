# Progress Details: 05-business

## What Changed
- `BuergerPortal.Business.csproj`: Changed TFM from net462 → net8.0, added Nullable and ImplicitUsings, removed OutputType Library (not needed in SDK-style)
- `ICitizenService.cs`: Changed `GetCitizenByTaxId` return type to `Citizen?`
- `CitizenService.cs`: Updated `GetCitizenByTaxId` return type to `Citizen?`; guarded `TaxId` null references before calling `GetByTaxId`

## Validation
- `dotnet build BuergerPortal.Business` → 0 errors, 0 warnings ✅
