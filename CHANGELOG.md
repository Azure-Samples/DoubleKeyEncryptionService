# Changelog

## 2026-11-11

### Changed

- Retarget the DKE service and its tests from .NET 8 to .NET 10 LTS.
- Update the service's Microsoft framework package references to 10.0.15.

### Added

- Add `global.json` to select .NET SDK 10.0.404 or a later stable patch in the same feature band.
- Document build prerequisites and deployment runtime maintenance in the README.

### Compatibility

- Building the sample now requires the .NET 10 SDK selected by `global.json`.
- Framework-dependent deployments require a .NET 10 hosting environment with ASP.NET Core Runtime 10.0.15 or later, or the corresponding IIS Hosting Bundle.
- SDK selection does not update customer-hosted runtimes. Customers must maintain the hosting runtime and restart the service after runtime updates.
- Self-contained deployments must be rebuilt and republished with a patched runtime; container deployments must be rebuilt and redeployed using patched .NET 10 runtime images.
