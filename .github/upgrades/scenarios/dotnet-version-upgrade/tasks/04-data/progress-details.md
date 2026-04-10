# Progress Details: 04-data

## What Changed
- `BuergerPortal.Data.csproj`: net462→net8.0, removed EntityFramework 6.4.4 and System.Data.Entity ref, added Microsoft.EntityFrameworkCore 8.0.25 + Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11
- `BuergerPortalContext.cs`: Rewrote for EF Core — constructor uses DbContextOptions, OnModelCreating uses ModelBuilder (EF Core) instead of DbModelBuilder (EF6), WithRequired→WithOne+IsRequired, HasRequired→HasOne, DbSet properties use `=> Set<T>()` pattern
- `BuergerPortalInitializer.cs`: Replaced EF6 `CreateDatabaseIfNotExists<T>` with static `BuergerPortalSeeder` class using async methods compatible with EF Core
- `IRepository.cs`: Updated GetById return type to `T?` for nullable safety
- `CitizenRepository.cs`: Updated using statements (System.Data.Entity→Microsoft.EntityFrameworkCore), nullable return types on GetById/GetByIdWithApplications/GetByTaxId, EntityState fix
- `ServiceApplicationRepository.cs`: Same using update, nullable returns, EF6 raw SQL (Database.SqlQuery) replaced with EF Core LINQ query with Include
- `ServiceTypeRepository.cs`: Same using update, nullable returns, EntityState fix
- `PublicOfficeRepository.cs`: Same using update, nullable returns, EntityState fix

## Validation
- `dotnet build BuergerPortal.Data` → 0 errors, 0 warnings ✅
