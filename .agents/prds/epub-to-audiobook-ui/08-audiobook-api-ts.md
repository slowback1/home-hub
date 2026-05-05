# AudiobookApi.ts

## Status

`pending`

## Description

Add `AudiobookApi.ts` to the SvelteKit frontend API layer. It wraps all `AudiobookController` endpoints using the existing `baseApi.ts` middleware chain, giving the UI typed methods for every job and voice-sample operation.

## Acceptance Criteria

- [ ] `AudiobookApi.ts` exists at `frontend/src/lib/api/AudiobookApi.ts` and extends `baseApi` (or follows the same constructor/middleware pattern as `ActivityApi.ts`)
- [ ] All job methods are implemented with correct HTTP verbs and paths:
  - `listJobs()` → `GET /api/audiobook/jobs`
  - `submitJob(epubFile: File, voiceSampleName: string)` → `POST /api/audiobook/jobs` (multipart)
  - `getJob(id: string)` → `GET /api/audiobook/jobs/{id}`
  - `cancelJob(id: string)` → `DELETE /api/audiobook/jobs/{id}`
  - `getFileUrl(id: string)` → returns the URL string for `GET /api/audiobook/jobs/{id}/file` (used as anchor href for download)
  - `deleteFile(id: string)` → `DELETE /api/audiobook/jobs/{id}/file`
- [ ] All voice sample methods are implemented:
  - `listVoiceSamples()` → `GET /api/audiobook/voice-samples`
  - `uploadVoiceSample(file: File, name: string)` → `POST /api/audiobook/voice-samples` (multipart)
  - `deleteVoiceSample(name: string)` → `DELETE /api/audiobook/voice-samples/{name}`
- [ ] TypeScript types `AudiobookJob` and `AudiobookJobStatus` are defined (matching the backend model fields in camelCase)
- [ ] Unit tests (Vitest) cover at least `listJobs`, `submitJob`, and `listVoiceSamples` using the existing mock utilities in `lib/testHelpers/`

## Notes

`getFileUrl` does not make a fetch call — it just constructs and returns the URL so the UI can set it as an anchor `href` with `download` attribute, triggering a native browser download. This is simpler than fetching a blob and creating an object URL.

Look at `ActivityApi.ts` and `baseApi.ts` for the established patterns for multipart form data and typed response parsing.

`AudiobookJobStatus` should be a TypeScript string union or enum matching the values returned by the backend: `"queued" | "in_progress" | "completed" | "failed" | "cancelled"`.
