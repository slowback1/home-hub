<script lang="ts">
	export let url: string;
	export let name: string;
	export let size: number = 40;

	const INTRANET_PATTERN = /^(127\.|10\.|192\.168\.|172\.(1[6-9]|2[0-9]|3[01])\.)/;

	// RFC 1918 and .local patterns that Google's favicon service can't reach
	function isIntranet(rawUrl: string): boolean {
		try {
			const { hostname } = new URL(rawUrl);
			return hostname.endsWith('.local') || INTRANET_PATTERN.test(hostname);
		} catch {
			return false;
		}
	}

	function faviconSrc(rawUrl: string): string | null {
		if (isIntranet(rawUrl)) return null;
		try {
			const { hostname } = new URL(rawUrl);
			return `https://www.google.com/s2/favicons?domain=${hostname}&sz=64`;
		} catch {
			return null;
		}
	}

	function firstLetter(s: string): string {
		const c = s
			.trim()
			.replace(/^[^a-z0-9]+/i, '')
			.charAt(0)
			.toUpperCase();
		return c || '#';
	}

	$: src = faviconSrc(url);
	let errored = false;
	let loaded = false;

	$: {
		// Reset state when url changes
		if (url) {
			errored = false;
			loaded = false;
		}
	}

	$: showLetter = !src || errored || !loaded;
</script>

<div class="favicon" style="width: {size}px; height: {size}px;" aria-hidden="true">
	{#if showLetter}
		<span class="letter-tile">{firstLetter(name)}</span>
	{/if}
	{#if src && !errored}
		<img
			{src}
			alt=""
			loading="lazy"
			on:error={() => (errored = true)}
			on:load={() => (loaded = true)}
		/>
	{/if}
</div>

<style>
	.favicon {
		position: relative;
		flex-shrink: 0;
		border-radius: var(--radius-sm);
		overflow: hidden;
	}

	.letter-tile {
		display: flex;
		align-items: center;
		justify-content: center;
		width: 100%;
		height: 100%;
		background-color: var(--color-surface-deep);
		color: var(--color-text-secondary);
		font-size: calc(var(--font-size-sm) * 1.1);
		font-weight: var(--font-weight-semibold);
		border-radius: var(--radius-sm);
		user-select: none;
	}

	img {
		position: absolute;
		inset: 0;
		width: 100%;
		height: 100%;
		object-fit: contain;
	}
</style>
