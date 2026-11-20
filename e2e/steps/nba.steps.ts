import { expect } from '@playwright/test';
import { Given, When, Then, Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const FLAG = 'NBA_SCHEDULE_ENABLED';

type SeededGame = { homeTeam: string; awayTeam: string; startAt: Date };

let seededGames: SeededGame[] = [];

function soonTodayLocal(now: Date): Date {
	const todayEnd = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 1);
	const soon = new Date(now.getTime() + 60 * 60_000);
	return soon < todayEnd ? soon : new Date(todayEnd.getTime() - 60 * 60_000);
}

function formatLocalTime(date: Date): string {
	return new Intl.DateTimeFormat(undefined, { timeStyle: 'short' }).format(date);
}

Before({ tags: '@nba' }, async ({ request }) => {
	seededGames = [];
	await request.delete(`${BACKEND_URL}/api/test/nba-games`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

After({ tags: '@nba' }, async ({ request }) => {
	seededGames = [];
	await request.delete(`${BACKEND_URL}/api/test/nba-games`).catch(() => {});
	await request.delete(`${BACKEND_URL}/api/test/dashboard`).catch(() => {});
	await request
		.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, { data: { isEnabled: false } })
		.catch(() => {});
});

Given('the NBA_SCHEDULE_ENABLED feature flag is enabled', async ({ request }) => {
	const response = await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, {
		data: { isEnabled: true }
	});
	if (!response.ok()) throw new Error(`Failed to enable ${FLAG}: ${response.status()}`);
});

Given('the NBA_SCHEDULE_ENABLED feature flag is disabled', async ({ request }) => {
	const response = await request.patch(`${BACKEND_URL}/api/feature-flags/${FLAG}`, {
		data: { isEnabled: false }
	});
	if (!response.ok()) throw new Error(`Failed to disable ${FLAG}: ${response.status()}`);
});

Given('NBA games are seeded with deterministic timestamps', async ({ request }) => {
	const now = new Date();
	seededGames = [
		{ homeTeam: 'Lakers', awayTeam: 'Celtics', startAt: soonTodayLocal(now) },
		{ homeTeam: 'Warriors', awayTeam: 'Suns', startAt: new Date(now.getTime() - 4 * 60 * 60_000) },
		{ homeTeam: 'Bulls', awayTeam: 'Knicks', startAt: new Date(now.getTime() + 24 * 60 * 60_000) }
	];
	for (const game of seededGames) {
		const response = await request.post(`${BACKEND_URL}/api/test/nba-games`, {
			data: {
				startAt: game.startAt.toISOString(),
				homeTeam: game.homeTeam,
				awayTeam: game.awayTeam,
				sourceGameId: null
			}
		});
		if (!response.ok()) throw new Error(`Failed to seed NBA game: ${response.status()}`);
	}
});

Given('the nba-games widget is placed in slot {int}', async ({ request, testUser }, slotIndex) => {
	await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
		data: {
			layoutFormat: '3x2',
			slots: [{ slotIndex, widgetType: 'nba-games' }]
		},
		headers: { 'X-User-Id': testUser.id }
	});
});

Given('a filler widget is placed in slot {int}', async ({ request, testUser }, slotIndex) => {
	await request.put(`${BACKEND_URL}/api/dashboard/layout`, {
		data: {
			layoutFormat: '3x2',
			slots: [{ slotIndex, widgetType: 'tasks' }]
		},
		headers: { 'X-User-Id': testUser.id }
	});
});

Then(
	'the NBA widget shows the game row for {string} with its local start time',
	async ({ dashboardPage }, label: string) => {
		const [homeTeam, awayTeam] = label.split(' @ ');
		const game = seededGames.find((g) => g.homeTeam === homeTeam && g.awayTeam === awayTeam);
		if (!game) throw new Error(`Seeded game not found: ${label}`);
		const row = dashboardPage.page.locator(
			`[data-testid="nba-game-row"]:has-text("${homeTeam} @ ${awayTeam}")`
		);
		await expect(row).toBeVisible({ timeout: 5000 });
		await expect(row).toContainText(formatLocalTime(game.startAt));
	}
);

Then(
	'the NBA widget does not show the game row for {string}',
	async ({ dashboardPage }, label: string) => {
		const [homeTeam, awayTeam] = label.split(' @ ');
		await expect(
			dashboardPage.page.locator(
				`[data-testid="nba-game-row"]:has-text("${homeTeam} @ ${awayTeam}")`
			)
		).toBeHidden();
	}
);

Then('the NBA widget list has a scroll container', async ({ dashboardPage }) => {
	await expect(dashboardPage.page.locator('[data-testid="nba-games-scroll"]')).toHaveCSS(
		'overflow-y',
		'auto'
	);
});

When('I click the chevron on the NBA widget', async ({ dashboardPage }) => {
	await dashboardPage.page.click('a.widget-header__open[title="Open NBA Games"]');
});

Then('I am on the NBA page', async ({ dashboardPage }) => {
	await dashboardPage.page.waitForURL('**/nba');
});

Then('the NBA page lists the game {string}', async ({ dashboardPage }, label: string) => {
	const [homeTeam, awayTeam] = label.split(' @ ');
	await expect(
		dashboardPage.page.locator(`[data-testid="nba-game-row"]:has-text("${homeTeam} @ ${awayTeam}")`)
	).toBeVisible({ timeout: 5000 });
});

Then('the NBA page does not list the game {string}', async ({ dashboardPage }, label: string) => {
	const [homeTeam, awayTeam] = label.split(' @ ');
	await expect(
		dashboardPage.page.locator(`[data-testid="nba-game-row"]:has-text("${homeTeam} @ ${awayTeam}")`)
	).toBeHidden();
});

Then('the NBA nav item is not visible in the sidebar', async ({ dashboardPage }) => {
	await expect(dashboardPage.page.locator('[data-testid="nav-item-nba"]')).toBeHidden();
});

Then('the NBA widget option is not visible in the picker', async ({ dashboardPage }) => {
	await expect(
		dashboardPage.page.locator('[data-testid="widget-picker-card"][data-widget-id="nba-games"]')
	).toBeHidden();
});
