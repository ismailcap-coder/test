# Progress Details: 07-tests

## What Changed
- `BuergerPortal.Tests.csproj`: net462→net8.0, replaced legacy QualityTools GAC reference + old Moq/Castle.Core with MSTest.TestFramework 4.2.1, MSTest.TestAdapter 4.2.1, Microsoft.NET.Test.Sdk 17.12.0, Moq 4.20.72
- Converted all [ExpectedException] usages to Assert.ThrowsExactly<T>() (MSTest v4 removed Assert.ThrowsException)
- Fixed nullable warnings: fields initialized in [TestInitialize] → null!, null literals → null!, Returns((T)null) → Returns((T?)null)

## Validation
- Build: 0 errors, 0 warnings ✅
- Tests: 82 passed, 0 failed ✅
