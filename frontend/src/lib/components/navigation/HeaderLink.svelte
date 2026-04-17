<script lang="ts">
	import UrlPathProvider from '$lib/providers/urlPathProvider';
	import { onMount } from 'svelte';

	export let href: string;
	export let label: string;

	let isActivePath = UrlPathProvider.matchesPath(href);

	onMount(() => {
		isActivePath = UrlPathProvider.matchesPath(href);
	});
</script>

<a class="header-link" class:header-link__active={isActivePath} {href}>{label}</a>

<style>
	.header-link {
		font-size: var(--font-size-medium);
	}

	.header-link__active {
		text-decoration: underline;
		color: red;
	}
</style>
