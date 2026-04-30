# Admin Page: Load and Display

## Status

`done`

## Description

Create the `/admin/system-config` SvelteKit route. On mount the page fetches all config entries, shows a spinner while loading, shows an inline error message if the fetch fails, and renders the entries grouped by namespace in a read-only table.

## Acceptance Criteria

- [ ] Route exists at `frontend/src/routes/admin/system-config/+page.svelte`
- [ ] Page title is `HomeHub — Admin: System Config`
- [ ] `Spinner` component is shown while the fetch is in flight
- [ ] An inline error message is shown if `getAll()` fails (no toast — page-level failure shown in context)
- [ ] On success, entries are grouped by `namespace` with a visible section header per namespace
- [ ] Each row displays the `key` and `value` (plain text, no editing yet)
- [ ] Unit/component tests cover: loading state, error state, and grouped display

## Notes

Use `SystemConfigApi.getAll()` from task 05. The `Spinner` component is at `frontend/src/lib/ui/feedback/Spinner.svelte`.

Grouping logic: collect unique namespaces from the response array, then render a section per namespace containing its entries. Plain JS `reduce` or `Map` is sufficient — no extra library needed.

This task intentionally excludes editing — that is task 07.
