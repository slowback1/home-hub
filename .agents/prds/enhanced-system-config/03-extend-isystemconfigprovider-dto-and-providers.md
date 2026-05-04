# Extend ISystemConfigProvider, DTO, and Providers to Return Typed Fields with Options

## Status

`done`

## Description

Update the backend's config provider contract and all implementations to surface `Type` and `Options` alongside each config entry. The frontend will use these fields to decide how to render each row. The `SystemConfigController` requires no changes — it serialises whatever the provider returns.

## Acceptance Criteria

- [ ] `SystemConfigOptionDto` record exists with `Value` (string) and `Label` (string) properties
- [ ] `SystemConfigEntry` DTO (or equivalent response model) includes `Type` (string) and `Options` (`IReadOnlyList<SystemConfigOptionDto>`) fields
- [ ] `ISystemConfigProvider.GetAllAsync()` return type includes `Type` and `Options` on each entry
- [ ] `EfSystemConfigProvider.GetAllAsync()` queries `SystemConfigOptions` via the FK relationship and populates `Options` for each entry
- [ ] `DictionarySystemConfigProvider` supports options: entries pre-seeded with `weather::provider` return the correct two options; entries without options return an empty list
- [ ] `GET /api/system-config` response includes `type` and `options` fields for each entry (verified manually or via existing E2E)
- [ ] `dotnet build` passes with no errors or warnings
- [ ] All existing system-config-ui E2E scenarios continue to pass

## Notes

`EfSystemConfigProvider` should use `.Include(c => c.Options)` (or equivalent explicit load/join) rather than a separate query per entry.

`DictionarySystemConfigProvider` is used in E2E mode (`ASPNETCORE_ENVIRONMENT=E2E`). Its in-memory entries should carry options so the E2E reset hook can restore them correctly.

`ISystemConfigProvider.GetAsync(namespace, key)` (single-entry lookup) should also return `Type` and `Options` for consistency, even though the admin page currently only calls `GetAllAsync`.
