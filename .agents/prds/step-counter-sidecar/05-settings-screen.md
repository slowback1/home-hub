# Settings Screen

## Status

`pending`

## Description

Implement the Settings screen: a text field for the HomeHub backend URL, a Save button that persists it to Room, and a Test Connection button that fires a request to `GET /api/health` and shows inline success or failure feedback.

## Acceptance Criteria

- [ ] Settings screen replaces the placeholder on the Settings tab
- [ ] Backend URL text field is pre-populated with the currently saved value on screen entry
- [ ] Tapping Save persists the URL to Room via `SettingDao` (key: `backend_url`) and shows a brief confirmation (e.g. toast or snackbar "Saved")
- [ ] Tapping Test Connection fires `GET <backend_url>/api/health`; shows a loading indicator while in flight
- [ ] A successful response (2xx) shows "Connected" inline
- [ ] A failed response or network error shows "Connection failed" inline
- [ ] The URL field is disabled while the connection test is in flight
- [ ] No crash if the URL field is empty when Test Connection is tapped — show a validation message instead

## Notes

Use a `SettingsViewModel` backed by `SettingDao`. Expose UI state via `StateFlow`.

For the HTTP call in this screen only, a simple `HttpURLConnection` or a one-off `OkHttpClient` call is fine — the full Retrofit client is set up in task 09. Keep the networking minimal here.

The `/api/health` endpoint exists in the HomeHub backend (`HealthCheckController`).
