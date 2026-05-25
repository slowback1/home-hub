# Add DashboardSlot Model and Migration

## Status

`done`

## Description

Create the `DashboardSlot` model in `Common.Models`, register it in `AppDbContext`, and generate an EF Core migration. This is the data-layer foundation everything else builds on.

## Acceptance Criteria

- [ ] `DashboardSlot` model exists in `backend/Common/Models/` with properties: `Id` (string PK), `LayoutFormat` (string), `SlotIndex` (int), `WidgetType` (string, nullable — null means empty)
- [ ] `AppDbContext` has a `DashboardSlots` `DbSet<DashboardSlot>`
- [ ] A unique constraint is applied on `(LayoutFormat, SlotIndex)` in `OnModelCreating`
- [ ] EF Core migration is generated via `dotnet ef migrations add` (not hand-written); `.Designer.cs` and snapshot are tool-generated
- [ ] `dotnet build` succeeds across all backend projects

## Notes

Migration command (from repo root):
```bash
export PATH="$PATH:/home/claude/.dotnet/tools"
dotnet ef migrations add AddDashboardSlotTable --project backend/EntityFramework --startup-project backend/WebAPI
```
See `CLAUDE.md` for full migration guidance. `WidgetType` being nullable is intentional — the column stores the widget slug (e.g. `"tasks"`) or `null` for an empty slot. The initial format shipped is `"3x2"` (6 slots, indices 0–5).
