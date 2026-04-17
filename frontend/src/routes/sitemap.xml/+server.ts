const site = 'https://yourdomain.com';

const pages: string[] = [];

const sveltePageFiles = import.meta.glob('/src/routes/**/+page.svelte');
const filePaths = Object.keys(sveltePageFiles);

type PageOverride = {
	/** This should a numeric value that is between 0.0 and 1.0 */
	priority?: string;
	/** This should be one of the following values: always, hourly, daily, weekly, monthly, yearly, never */
	changeFreq?: string;
};

const overrides: Record<string, PageOverride> = {
	'': {
		priority: '1.0',
		changeFreq: 'monthly'
	}
};

for (const filePath of filePaths) {
	const page = filePath.replace('/src/routes/', '').replace('+page.svelte', '');
	pages.push(page);
}

function getOverride(page: string): PageOverride {
	return overrides[page] ?? {};
}

function getPriority(page: string) {
	const defaultPagePriority = 0.5;

	return getOverride(page).priority ?? defaultPagePriority;
}

function getChangeFreq(page: string) {
	return getOverride(page)?.changeFreq ?? 'daily';
}

export const prerender = true;

/** @type {import('./$types').RequestHandler} */
export async function GET() {
	const body = buildSitemapXml(pages);
	const response = new Response(body);
	response.headers.set('Cache-Control', 'max-age=0, s-maxage=3600');
	response.headers.set('Content-Type', 'application/xml');

	return response;
}

const buildSitemapXml = (pages: string[]) => `<?xml version="1.0" encoding="UTF-8" ?>
<urlset
  xmlns="https://www.sitemaps.org/schemas/sitemap/0.9"
  xmlns:news="https://www.google.com/schemas/sitemap-news/0.9"
  xmlns:xhtml="https://www.w3.org/1999/xhtml"
  xmlns:mobile="https://www.google.com/schemas/sitemap-mobile/1.0"
  xmlns:image="https://www.google.com/schemas/sitemap-image/1.1"
  xmlns:video="https://www.google.com/schemas/sitemap-video/1.1"
>
  ${pages
		.map(
			(page) => `
  <url>
    <loc>${site}/${page}</loc>
    <changefreq>${getChangeFreq(page)}</changefreq>
    <priority>${getPriority(page)}</priority>
  </url>
  `
		)
		.join('')}
</urlset>`;
