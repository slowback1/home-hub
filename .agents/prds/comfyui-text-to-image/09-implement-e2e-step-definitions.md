# Implement E2E Step Definitions and Verify GREEN

## Status

`done`

## Description

Implement all step definitions and page object methods for the four `@comfyui` scenarios. Replace the stubs from task 01 with real Playwright interactions. All scenarios must pass GREEN against the stub server from task 08.

## Acceptance Criteria

- [ ] `e2e/steps/comfyui-text-to-image.steps.ts` fully implements all Given/When/Then steps for all four scenarios
- [ ] `e2e/pages/ComfyUiPage.ts` implements navigation and all interactions for the inference page (workflow select, prompt input, generate click, image visible, error banner visible)
- [ ] `e2e/pages/ComfyUiConfigPage.ts` implements navigation and all interactions for the config page (fill add form, submit, delete workflow, assert list contents)
- [ ] `@comfyui-add-workflow` passes GREEN
- [ ] `@comfyui-delete-workflow` passes GREEN
- [ ] `@comfyui-generate-happy-path` passes GREEN (stub server returns test image)
- [ ] `@comfyui-generate-error` passes GREEN (stub server in failure mode returns error, UI shows banner)
- [ ] `task e2e:test` with `--grep @comfyui` exits with code 0

## Notes

The `@comfyui-generate-happy-path` scenario needs to assert that an `<img>` element is visible with a `src` starting with `data:image/`. Checking for the literal base64 string is brittle — checking the `src` prefix is sufficient.

For `@comfyui-generate-error`, the step "the ComfyUI stub server is set to fail" should call `POST http://localhost:8199/stub/fail` (the stub server's control endpoint) before navigating to the page.

The `@comfyui-add-workflow` and `@comfyui-delete-workflow` scenarios need a known-valid workflow JSON to paste into the form. A minimal placeholder JSON such as `{"1": {"inputs": {"text": "{{prompt}}"}, "class_type": "CLIPTextEncode"}}` is sufficient for the test.
