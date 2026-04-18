# Implement AppDbContext and EfCrud

## Status

`done` <!-- pending | in-progress | done -->

## Description

Implement the core EF Core persistence classes inside the `EntityFramework` project: an `AppDbContext` with `DbSet<T>` properties for each `IIdentifyable` entity, a generic `EfCrud<T>` that implements `ICrud<T>` using `DbContext.Set<T>()`, and an `EfCrudFactory` that implements `ICrudFactory` and receives `AppDbContext` via constructor injection.

## Acceptance Criteria

- [ ] `AppDbContext` exists with `DbSet<AppUser>` and `DbSet<ExampleData>` properties
- [ ] `AppDbContext` accepts `DbContextOptions<AppDbContext>` via constructor (standard EF Core pattern)
- [ ] `EfCrud<T>` implements `ICrud<T>` using `DbContext.Set<T>()` for all six methods
- [ ] `EfCrud<T>` uses `Expression<Func<T, bool>>` directly in EF LINQ queries (no `.Compile()` needed — EF translates expressions to SQL)
- [ ] `EfCrudFactory` implements `ICrudFactory`, receives `AppDbContext` via constructor, and returns a new `EfCrud<T>` instance per `GetCrud<T>()` call
- [ ] `dotnet build` passes with no errors
- [ ] Unit tests exist for `EfCrud<T>` covering all six `ICrud<T>` methods using an in-memory or SQLite EF provider (not a real Postgres instance)

## Notes

- `AppUser` and `ExampleData` are the only `IIdentifyable` entities currently in `Common/Models/`. `FeatureFlag` does not implement `IIdentifyable` and should not be added to `AppDbContext`.
- For `CreateAsync`: EF Core does not auto-assign string IDs — generate a new `Guid.NewGuid().ToString()` and assign it to `item.Id` before calling `AddAsync`.
- For `UpdateAsync`: use `Update()` + `SaveChangesAsync()`; return the updated entity (or `null` if not found).
- For unit tests, use `Microsoft.EntityFrameworkCore.InMemory` provider (add as a test-only package in the test project) to avoid requiring a real Postgres instance.
- `EfCrudFactory` and `AppDbContext` will be registered as `Scoped` in a later task (`04`).
