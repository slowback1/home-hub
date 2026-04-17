<script>
	import SkipToContentLink from '$lib/components/navigation/SkipToContentLink.svelte';
	import HeaderLink from '$lib/components/navigation/HeaderLink.svelte';
	import ThemeToggle from '$lib/components/navigation/ThemeToggle.svelte';
</script>

<nav data-testid="header">
	<SkipToContentLink />

	<HeaderLink href="/" label="Home" />
	<ThemeToggle />
</nav>

<style>
	nav {
		display: flex;
		flex-direction: row;
		justify-content: flex-end;
		align-items: center;
		margin: 0;
		padding-inline: var(--gutters-x);
		background-color: var(--color-primary-background);
		color: var(--color-primary-font);
		height: var(--header-height);
		gap: 12px;
	}
</style>
