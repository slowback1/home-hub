<script lang="ts">
	export let testId: string = 'card';
</script>

<div class="card" data-testid={testId}>
	<slot />
</div>

<style>
	.card {
		background-color: var(--color-surface-raised);
		border: 1px solid var(--color-border-subtle);
		border-radius: var(--radius-md);
		padding: var(--space-6);
	}
</style>
