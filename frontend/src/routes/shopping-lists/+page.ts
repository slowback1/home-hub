import type { PageLoadEvent } from './$types';

export function load({ url }: PageLoadEvent) {
	return {
		fullscreen: url.searchParams.get('fullscreen')
	};
}
