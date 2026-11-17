import { load } from './+page';

type LoadEvent = Parameters<typeof load>[0];

describe('Shopping lists load', () => {
	it('passes the fullscreen query param through to page data', async () => {
		const data = await load({
			url: new URL('http://localhost/shopping-lists?fullscreen=l1')
		} as LoadEvent);

		expect(data.fullscreen).toBe('l1');
	});

	it('is null when the fullscreen query param is absent', async () => {
		const data = await load({ url: new URL('http://localhost/shopping-lists') } as LoadEvent);

		expect(data.fullscreen).toBeNull();
	});
});
