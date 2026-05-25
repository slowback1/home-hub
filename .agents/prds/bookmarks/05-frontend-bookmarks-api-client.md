# Frontend BookmarksApi Client

## Status

`pending`

## Description

Implement the TypeScript API client class for the bookmarks backend, covering all five endpoints. This is the data layer the UI components will call.

## Acceptance Criteria

- [ ] `frontend/src/lib/api/BookmarksApi.ts` exists and extends `BaseApi`
- [ ] `Bookmark` type exported: `{ id: string; name: string; url: string; description: string | null; starred: boolean; createdAt: string }`
- [ ] `CreateBookmarkRequest` and `UpdateBookmarkRequest` types exported
- [ ] `listBookmarks(): Promise<Bookmark[]>` — `GET /api/bookmarks`
- [ ] `createBookmark(req: CreateBookmarkRequest): Promise<Bookmark>` — `POST /api/bookmarks`
- [ ] `updateBookmark(id: string, req: UpdateBookmarkRequest): Promise<Bookmark>` — `PUT /api/bookmarks/{id}`
- [ ] `deleteBookmark(id: string): Promise<void>` — `DELETE /api/bookmarks/{id}`
- [ ] `toggleStar(id: string): Promise<Bookmark>` — `PATCH /api/bookmarks/{id}/star`
- [ ] Unit/spec tests exist and pass (mirroring `TasksApi.spec.ts`)

## Notes

- Follow the pattern in `TasksApi.ts` and its spec file `TasksApi.spec.ts`
- `BaseApi` is at `frontend/src/lib/api/baseApi.ts`
