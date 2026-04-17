import { GET as getSiteMap } from './+server';

describe('Sitemap', () => {
	it('should return a sitemap.xml', async () => {
		const response = await getSiteMap();
		expect(response.status).toBe(200);

		const headers = response.headers;

		expect(headers.get('Content-Type')).toBe('application/xml');
	});

	it('Should set the max-age headers to 3600', async () => {
		const response = await getSiteMap();
		const headers = response.headers;

		expect(headers.get('Cache-Control')).toBe('max-age=0, s-maxage=3600');
	});

	it('response body is valid xml', async () => {
		const response = await getSiteMap();
		const body = await response.text();

		expect(() => new DOMParser().parseFromString(body, 'text/xml')).not.toThrow();
	});

	async function getHomeRecord() {
		const body = await getSiteMap().then((response) => response.text());

		const xml = new DOMParser().parseFromString(body, 'text/xml');

		const urls = xml.querySelectorAll('urlset > url');

		const home = Array.from(urls)[0];

		expect(home).not.toBeFalsy();
		return home;
	}

	it('the home page should have a priority of 1.0', async () => {
		const home = await getHomeRecord();

		expect(home.querySelector('priority').textContent).toBe('1.0');
	});

	it('the home page should have a changefreq of monthly', async () => {
		const home = await getHomeRecord();

		expect(home.querySelector('changefreq').textContent).toBe('monthly');
	});
});
