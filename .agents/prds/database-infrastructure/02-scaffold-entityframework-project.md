# Scaffold EntityFramework Project

## Status

`done` <!-- pending | in-progress | done -->

## Description

Create the `EntityFramework` class library project under `backend/`, add it to the solution, and install the required NuGet packages. This is pure scaffolding — no implementation code yet — but it establishes the project structure that all subsequent EF tasks build on.

## Acceptance Criteria

- [ ] `backend/EntityFramework/EntityFramework.csproj` exists targeting `net8.0`
- [ ] Project is added to `backend/HomeHubBackend.sln`
- [ ] NuGet packages installed: `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`
- [ ] Project references `Common` (for `ICrud<T>`, `ICrudFactory`, `IIdentifyable`, and entity models)
- [ ] `dotnet build` passes with no errors

## Notes

- Follow the same project structure as `backend/InMemory/` and `backend/FileData/` — minimal `.csproj`, no unnecessary boilerplate.
- Use the same `net8.0` target framework as all other projects in the solution.
- Package versions should align with the EF Core version compatible with .NET 8 (8.x).
