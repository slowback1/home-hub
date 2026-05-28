# Room Database Layer

## Status

`pending`

## Description

Define the Room database: two entities (`WalkSessionEntity` and `SettingEntity`), their DAOs, and the `SlowWalkDatabase` class. This is the single local storage layer for both walk sessions and app settings — no secondary storage mechanism is used.

## Acceptance Criteria

- [ ] `WalkSessionEntity` has columns: `clientId` (primary key, String), `startedAt` (Long, epoch millis), `durationSeconds` (Int), `stepCount` (Int), `serverSyncedAt` (Long?, nullable — null means pending)
- [ ] `SettingEntity` has columns: `key` (primary key, String), `value` (String)
- [ ] `WalkSessionDao` exposes: insert-or-replace, query all pending (`WHERE serverSyncedAt IS NULL`), delete by `clientId`, update `serverSyncedAt` by `clientId`
- [ ] `SettingDao` exposes: upsert by key, query by key
- [ ] `SlowWalkDatabase` is a `RoomDatabase` subclass that includes both entities and DAOs, built as a singleton
- [ ] The database compiles without KSP errors and can be injected into a ViewModel in a subsequent task

## Notes

`clientId` is the primary key for `WalkSessionEntity` because it's the stable, client-generated identity used for deduplication. The server-assigned `Id` is not stored locally — only `serverSyncedAt` is needed to know a session was accepted.

`startedAt` stored as epoch millis (Long) is the standard Room approach for timestamps — convert to/from `java.time.Instant` or `LocalDateTime` at the DAO boundary using a `TypeConverter` if needed.

Place entities in `data/local/entity/`, DAOs in `data/local/dao/`, and the database class in `data/local/`.
