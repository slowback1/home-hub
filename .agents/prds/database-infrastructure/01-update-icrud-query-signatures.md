# Update ICrud<T> Query Signatures

## Status

`done` <!-- pending | in-progress | done -->

## Description

Change `GetByQueryAsync` and `QueryAsync` on `ICrud<T>` from `Func<T, bool>` to `Expression<Func<T, bool>>` so that EF Core can translate queries to SQL. Update all existing implementations (`InMemory`, `FileData`, `TestCrud`) to call `.Compile()` on the expression before applying it in-memory. Also change all three `ICrudFactory` registrations in `CrudFactoryConfigurator` from `AddSingleton` to `AddScoped` to prevent captive-dependency runtime errors when the EF implementation (which holds a scoped `DbContext`) is introduced.

## Acceptance Criteria

- [ ] `ICrud<T>.GetByQueryAsync` and `ICrud<T>.QueryAsync` accept `Expression<Func<T, bool>>` in `Common/Interfaces/ICrud.cs`
- [ ] `InMemoryCrud<T>` compiles the expression via `.Compile()` before use
- [ ] `FileData/FileCrud<T>` compiles the expression via `.Compile()` before use
- [ ] `TestUtilities/CrudImplementations/TestCrud<T>` compiles the expression via `.Compile()` before use
- [ ] `InMemoryCrudFactory` and `FileCrudFactory` are registered as `Scoped` (not `Singleton`) in `CrudFactoryConfigurator`
- [ ] All existing call sites in `Logic/` (e.g. `RegisterUseCase`, `LoginUseCase`, `UserAuthorizationUseCase`) compile without changes (compiler promotes lambdas to expressions automatically)
- [ ] `dotnet build` passes with no errors or warnings
- [ ] All existing tests pass (`dotnet test`)

## Notes

- Call sites are in `backend/Logic/User/RegisterUseCase.cs:56`, `backend/Logic/User/LoginUseCase.cs:13`, `backend/Logic/Authorization/UserAuthorizationUseCase.cs:49` — these pass simple lambdas which the C# compiler automatically promotes to `Expression<Func<T, bool>>`, so no manual changes are needed there.
- `using System.Linq.Expressions;` will need to be added to `ICrud.cs` and any implementation files that don't already import it.
- `CrudFactoryConfigurator` is at `backend/WebAPI/Configuration/CrudFactoryConfigurator.cs`.
