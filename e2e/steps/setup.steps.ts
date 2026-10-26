import { Before, After } from '../fixtures';

const BACKEND_URL = 'http://localhost:5273';
const TEST_USER_NAME = 'E2E User';

Before({ tags: 'not @user-switching' }, async ({ request, userSwitchingPage }) => {
	await request.delete(`${BACKEND_URL}/api/test/users`);
	const response = await request.post(`${BACKEND_URL}/api/test/users`, {
		data: { name: TEST_USER_NAME }
	});
	if (!response.ok()) {
		throw new Error(`Failed to seed e2e test user: ${response.status()}`);
	}
	await userSwitchingPage.gotoHome();
	await userSwitchingPage.selectUserByName(TEST_USER_NAME);
});

After({ tags: 'not @user-switching' }, async ({ request }) => {
	await request.delete(`${BACKEND_URL}/api/test/users`);
});
