# Progress Details: 03-domain

## What Changed
- `BuergerPortal.Domain/BuergerPortal.Domain.csproj`: Changed TFM from net462 → net8.0, removed legacy framework references, added `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`
- `Entities/Citizen.cs`: Optional string properties marked `string?` (StreetAddress, City, PostalCode, PhoneNumber, Email, TaxId); required strings kept `string = null!`
- `Entities/ServiceApplication.cs`: Optional strings (ApplicationNumber, Notes, RejectionReason) → `string?`; required navigation properties → `= null!`
- `Entities/ServiceType.cs`: Required strings `= null!`; optional Description → `string?`
- `Entities/PublicOffice.cs`: Required strings `= null!`; optional strings → `string?`
- `Entities/AuditLog.cs`: Required strings `= null!`; optional Details → `string?`; navigation → `= null!`
- `Entities/ApplicationDocument.cs`: Required strings `= null!`; optional VerifiedBy → `string?`; navigation → `= null!`
- `Entities/FeeSchedule.cs`: Required string `= null!`; navigation → `= null!`

## Validation
- `dotnet build BuergerPortal.Domain` → 0 errors, 0 warnings ✅
