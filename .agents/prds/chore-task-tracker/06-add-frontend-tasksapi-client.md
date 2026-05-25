# Add Frontend TasksApi Client

## Status

`done`

## Description

Add `TasksApi.ts` to `frontend/src/lib/api/` as a typed client covering all task endpoints. Include a corresponding unit test file following the pattern of existing API clients.

## Acceptance Criteria

- [ ] `frontend/src/lib/api/TasksApi.ts` exists and extends `BaseApi`
- [ ] Exports a `ChoreTask` type with all fields from the data model: `id`, `name`, `isRecurring`, `intervalDays` (nullable), `doDate` (nullable string), `completedAt` (nullable string), `createdAt`
- [ ] Exports a `TaskCompletion` type with `id`, `taskId`, `completedAt`, `previousDoDate` (nullable string)
- [ ] Methods: `listTasks()`, `createTask(req)`, `updateTask(id, req)`, `deleteTask(id)`, `completeTask(id)`, `undoCompletion(id)`
- [ ] `frontend/src/lib/api/TasksApi.spec.ts` exists with unit tests for at least the create and complete methods
- [ ] `npm test` (or equivalent frontend test command) passes

## Notes

- Follow `AudiobookApi.ts` / `ActivityApi.ts` for class structure and `BaseApi` usage
- `doDate` and `completedAt` are returned as ISO date strings from the API — keep them as `string | null` in the TypeScript type; the page component can parse them as needed
- `createTask` request type: `{ name: string; doDate: string | null; isRecurring: boolean; intervalDays: number | null }`
- `updateTask` request type mirrors `createTask`
