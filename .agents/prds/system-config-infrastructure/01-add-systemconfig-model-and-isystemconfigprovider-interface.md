# Add SystemConfig model and ISystemConfigProvider interface

## Status

`done` <!-- pending | in-progress | done -->

## Description

Scaffold the Common-layer types that the rest of the feature depends on: the `SystemConfig` model and the `ISystemConfigProvider` interface. Nothing else in the feature can be built until these exist.

## Acceptance Criteria

- [ ] `Common/Models/SystemConfig.cs` exists and implements `IIdentifyable`
- [ ] `SystemConfig` has fields: `Id` (string), `Namespace` (string), `Key` (string), `Value` (string), `Type` (string), `IsSecret` (bool)
- [ ] `Id` is always in the format `"{namespace}::{key}"`; namespace and key are snake_case and must not contain `::`
- [ ] `Common/Interfaces/ISystemConfigProvider.cs` exists with the four methods: `GetAsync`, `GetSecretAsync`, `GetAllAsync`, `UpdateAsync`
- [ ] All method signatures use `string @namespace` and `string key` parameters
- [ ] The solution builds with no errors

## Notes

Interface signatures:

```csharp
public interface ISystemConfigProvider
{
    Task<SystemConfig> GetAsync(string @namespace, string key);
    Task<SystemConfig> GetSecretAsync(string @namespace, string key);
    Task<IEnumerable<SystemConfig>> GetAllAsync();
    Task<SystemConfig> UpdateAsync(string @namespace, string key, string value);
}
```

`GetAsync` — returns entry with `Value = "***"` when `IsSecret` is true; throws if not found.
`GetSecretAsync` — returns entry with the real value; throws if not found.
`GetAllAsync` — returns all entries with secrets masked.
`UpdateAsync` — updates `Value` only; all other fields are immutable.

Supported `Type` values for now: `"string"` and `"boolean"`.
