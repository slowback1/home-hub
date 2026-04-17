# AGENTS.md — E2E Tests

This document describes the architecture and conventions of the `e2e/` subsystem. All paths are relative to `e2e/` unless otherwise noted.

## Project Structure

```
e2e/
├── features/               # Gherkin feature files (.feature) — source of truth
├── steps/                  # Step definition files (.ts) — implement Given/When/Then
├── pages/                  # Page Object Model classes
├── tests/                  # Generated Playwright spec files (gitignored — do not edit)
├── fixtures.ts             # Custom Playwright fixtures (Page Objects + per-scenario state)
├── playwright.config.ts    # Playwright + playwright-bdd configuration
├── package.json            # Dependencies and npm scripts
└── Taskfile.yml            # Task runner tasks
```

## Quick-Start Tasks

Run these from the **repo root** or from `e2e/`.

| Command (root)  | Command (e2e/)  | Description                                   |
| --------------- | --------------- | --------------------------------------------- |
| `task setup`    | `task install`  | Install npm deps and download Chromium binary |
| `task e2e:test` | `task test`     | Generate specs then run all E2E tests         |
| —               | `task generate` | Generate specs only (without running tests)   |

`task test` always runs `task generate` first as a dependency — you do not need to run them separately.

## How It Works

`playwright-bdd` converts Gherkin feature files into Playwright `.spec` files before each run:

```
features/auth.feature  →  bddgen  →  tests/features/auth.feature.spec.js  →  Playwright
```

The generated files in `tests/` are **gitignored**. Never edit them directly — changes will be overwritten on the next `bddgen` run. Edit the `.feature` file or the step definitions instead.

## Writing a New Feature

### 1 — Add a feature file

Create `features/<name>.feature`. Every feature file must have:

- A **domain tag** on the `Feature` line (e.g. `@auth`, `@dashboard`).
- A **slug tag** on each `Scenario` matching the scenario title in kebab-case (e.g. `@register-happy-path`).

```gherkin
@auth
Feature: Authentication

  @register-happy-path
  Scenario: Register happy path - navigates to the app on success
    Given I am on the register page
    When I register with a unique username and password
    Then I should be redirected to the app
```

**Cucumber Expression note:** The `/` character is treated as an alternation separator in step text. Write "redirected to the app" instead of "redirected to /app".

### 2 — Add step definitions

Create `steps/<name>.steps.ts`. Import `Given`, `When`, `Then` from `../fixtures` (not from `playwright-bdd` directly):

```ts
import { Given, When, Then } from '../fixtures';

Given('I am on the register page', async ({ registerPage }) => {
	await registerPage.goto();
});
```

### 3 — Add fixtures (if needed)

If a new Page Object or per-scenario state is required, extend `fixtures.ts`:

```ts
// Add to the Fixtures type
type Fixtures = {
  myPage: MyPage;
};

// Add to base.extend<Fixtures>({...})
myPage: async ({ page }, use) => {
  await use(new MyPage(page));
},
```

Export `test` and the step helpers from `fixtures.ts` — they must all come from the same extended `test` instance for `bddgen` to resolve them.

### 4 — Add a Page Object (if needed)

Create `pages/<Name>Page.ts`. Page Objects take a `Page` as their sole constructor argument and expose async methods for navigation, form interaction, and state reads. See `pages/LoginPage.ts` for a reference implementation.

## Tag Convention

| Tag type   | Where           | Example                | Purpose                                                  |
| ---------- | --------------- | ---------------------- | -------------------------------------------------------- |
| Domain tag | `Feature` line  | `@auth`                | Run all scenarios for a domain: `--grep @auth`           |
| Slug tag   | `Scenario` line | `@register-happy-path` | Run one specific scenario: `--grep @register-happy-path` |

No `@smoke` tags are assigned here — smoke classification is managed separately outside this repository.

### Running by tag

```bash
# All auth scenarios
npx playwright test --grep @auth

# One specific scenario
npx playwright test --grep @register-happy-path
```

## Configuration

`playwright.config.ts` configures both Playwright and `playwright-bdd`:

```ts
const testDir = defineBddConfig({
	features: 'features/*.feature', // source feature files
	steps: ['steps/**/*.ts', 'fixtures.ts'], // step definitions + fixture file
	outputDir: './tests' // where generated specs are written
});
```

The config auto-starts both the backend (`localhost:5272`) and the frontend (`localhost:5173`) via `webServer` before tests run. On CI, fresh server instances are always started.
