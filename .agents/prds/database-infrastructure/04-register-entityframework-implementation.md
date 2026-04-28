# Register EntityFramework Implementation

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Wire the `EntityFramework` implementation into `WebAPI` by updating `CrudFactoryConfigurator` to add an explicit `case "entityframework":` branch (with `Scoped` registration), adding `EntityFramework` to the `CrudFactoryType` enum, and registering `AppDbContext` in `Program.cs` using the connection string from configuration.

## Acceptance Criteria

- [ ] `CrudFactoryType` enum gains an `EntityFramework` entry
- [ ] `CrudFactoryConfigurator` has an explicit `case "entityframework":` branch that registers `EfCrudFactory` as `Scoped`
- [ ] `CrudFactoryConfigurator` `default:` branch handles `InMemory` (no separate `case "inmemory":` to avoid linter duplicate-action warning)
- [ ] `AppDbContext` is registered in `Program.cs` via `AddDbContext<AppDbContext>` using `UseNpgsql` with the connection string from `configuration.GetConnectionString("DefaultConnection")`
- [ ] `WebAPI` project references the `EntityFramework` project
- [ ] `CrudFactoryConfiguratorTests` are updated to cover the new `EntityFramework` case (verifies `EfCrudFactory` is registered when `"EntityFramework"` is configured)
- [ ] `dotnet build` passes with no errors
- [ ] All existing tests pass (`dotnet test`)

## Notes

- `CrudFactoryConfigurator` is at `backend/WebAPI/Configuration/CrudFactoryConfigurator.cs`.
- The `EntityFramework` case must register both `AppDbContext` (via `AddDbContext`) and `EfCrudFactory` (via `AddScoped<ICrudFactory, EfCrudFactory>`). Alternatively, `AppDbContext` registration can live directly in `Program.cs` — either approach is acceptable as long as it is only registered once.
- Connection string key is `ConnectionStrings:DefaultConnection` (accessed via `configuration.GetConnectionString("DefaultConnection")`).
- The `CrudFactoryConfiguratorTests` are at `backend/WebAPI.Tests/` — check the exact path before editing.
