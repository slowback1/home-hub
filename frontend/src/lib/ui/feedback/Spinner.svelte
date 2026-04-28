<script lang="ts">
	type SpinnerSize = 'small' | 'medium' | 'large';
	export let size: SpinnerSize = 'medium';
</script>

<span
	class="spinner"
	class:spinner--small={size === 'small'}
	class:spinner--medium={size === 'medium'}
	class:spinner--large={size === 'large'}
	data-testid="spinner"
	role="status"
	aria-label="Loading"
></span>

<style>
	.spinner {
		display: inline-block;
		border: 2px solid var(--color-border-default);
		border-top-color: var(--color-brand-lighter);
		border-radius: var(--radius-full);
		animation: spin 0.7s linear infinite;
	}

	.spinner--small {
		width: var(--space-4);
		height: var(--space-4);
	}

	.spinner--medium {
		width: var(--space-6);
		height: var(--space-6);
	}

	.spinner--large {
		width: var(--space-8);
		height: var(--space-8);
	}

	@keyframes spin {
		to {
			transform: rotate(360deg);
		}
	}
</style>
