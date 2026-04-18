# Wire MigrateAsync on Startup

## Status

`in-progress` <!-- pending | in-progress | done -->

## Description

Call `MigrateAsync()` in `Program.cs` during app startup so that EF Core migrations are applied automatically whenever the application starts. This ensures the schema is always up to date on deploy without any manual intervention.

## Acceptance Criteria

- [ ] `Program.cs` resolves `AppDbContext` from the DI scope and calls `MigrateAsync()` during startup, before the app begins handling requests
- [ ] `MigrateAsync()` is only called when `CrudFactory:Implementation` is `"EntityFramework"` (it should not be called — and should not fail — when running under `InMemory` or `FileData`, e.g. in tests)
- [ ] App starts cleanly with the dev Postgres container running and migrations applied
- [ ] App starts cleanly in the `Test` environment (InMemory) without attempting a DB connection
- [ ] All existing tests pass (`dotnet test`)

## Notes

- The standard pattern for running migrations on startup in ASP.NET Core:
  ```csharp
  using (var scope = app.Services.CreateScope())
  {
      var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
      await db.Database.MigrateAsync();
  }
  ```
- Guard the call by checking configuration: read `CrudFactory:Implementation` from `IConfiguration` and only call `MigrateAsync()` if the value is `"EntityFramework"`. This prevents a `InvalidOperationException` ("No service for type AppDbContext") when running under `InMemory`.
- Alternatively, register `AppDbContext` unconditionally and use `MigrateAsync()` always — but the guard approach is cleaner given the swappable factory pattern.
- `Program.cs` is at `backend/WebAPI/Program.cs`.
