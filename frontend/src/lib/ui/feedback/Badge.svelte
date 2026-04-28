<script lang="ts">
	type BadgeVariant = 'default' | 'success' | 'warning' | 'error';
	export let text: string;
	export let variant: BadgeVariant = 'default';
	export let testId: string = 'badge';
</script>

<span
	class="badge"
	class:badge--default={variant === 'default'}
	class:badge--success={variant === 'success'}
	class:badge--warning={variant === 'warning'}
	class:badge--error={variant === 'error'}
	data-testid={testId}
>
	{text}
</span>

<style>
	.badge {
		display: inline-block;
		padding: 2px var(--space-2);
		border-radius: var(--radius-full);
		font-size: var(--font-size-xs);
		font-weight: var(--font-weight-semibold);
	}

	.badge--default {
		background-color: var(--color-surface-overlay);
		color: var(--color-text-secondary);
		border: 1px solid var(--color-border-default);
	}

	.badge--success {
		background-color: var(--color-success-surface);
		color: var(--color-success-text);
		border: 1px solid var(--color-success);
	}

	.badge--warning {
		background-color: var(--color-warning-surface);
		color: var(--color-warning-text);
		border: 1px solid var(--color-warning);
	}

	.badge--error {
		background-color: var(--color-error-surface);
		color: var(--color-error-text);
		border: 1px solid var(--color-error);
	}
</style>
