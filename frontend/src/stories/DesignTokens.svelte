<script lang="ts">
	const colors = [
		{ label: 'Brand', var: '--color-brand' },
		{ label: 'Brand Light', var: '--color-brand-light' },
		{ label: 'Brand Lighter', var: '--color-brand-lighter' },
		{ label: 'Surface Deep', var: '--color-surface-deep' },
		{ label: 'Surface Base', var: '--color-surface-base' },
		{ label: 'Surface Raised', var: '--color-surface-raised' },
		{ label: 'Surface Overlay', var: '--color-surface-overlay' },
		{ label: 'Text Primary', var: '--color-text-primary' },
		{ label: 'Text Secondary', var: '--color-text-secondary' },
		{ label: 'Text Disabled', var: '--color-text-disabled' },
		{ label: 'Border Subtle', var: '--color-border-subtle' },
		{ label: 'Border Default', var: '--color-border-default' },
		{ label: 'Error', var: '--color-error' },
		{ label: 'Warning', var: '--color-warning' },
		{ label: 'Success', var: '--color-success' },
		{ label: 'Info', var: '--color-info' }
	];

	const typeScale = [
		{ label: 'xs — 12px', var: '--font-size-xs' },
		{ label: 'sm — 14px', var: '--font-size-sm' },
		{ label: 'md — 16px', var: '--font-size-md' },
		{ label: 'lg — 20px', var: '--font-size-lg' },
		{ label: 'xl — 24px', var: '--font-size-xl' },
		{ label: '2xl — 32px', var: '--font-size-2xl' },
		{ label: '3xl — 40px', var: '--font-size-3xl' }
	];

	const spacing = [
		{ label: 'space-1 — 4px', var: '--space-1', px: 4 },
		{ label: 'space-2 — 8px', var: '--space-2', px: 8 },
		{ label: 'space-3 — 12px', var: '--space-3', px: 12 },
		{ label: 'space-4 — 16px', var: '--space-4', px: 16 },
		{ label: 'space-6 — 24px', var: '--space-6', px: 24 },
		{ label: 'space-8 — 32px', var: '--space-8', px: 32 },
		{ label: 'space-12 — 48px', var: '--space-12', px: 48 },
		{ label: 'space-16 — 64px', var: '--space-16', px: 64 }
	];

	const radii = [
		{ label: 'radius-sm — 8px', var: '--radius-sm' },
		{ label: 'radius-md — 12px', var: '--radius-md' },
		{ label: 'radius-full — 9999px', var: '--radius-full' }
	];
</script>

<div class="tokens-page">
	<section class="section">
		<h2 class="section-title">Color Palette</h2>
		<div class="color-grid">
			{#each colors as color (color.var)}
				<div class="color-swatch">
					<div class="swatch" style="background-color: var({color.var})"></div>
					<span class="swatch-label">{color.label}</span>
					<code class="swatch-var">{color.var}</code>
				</div>
			{/each}
		</div>
	</section>

	<section class="section">
		<h2 class="section-title">Type Scale</h2>
		<div class="type-list">
			{#each typeScale as step (step.var)}
				<div class="type-row">
					<span class="type-sample" style="font-size: var({step.var})">Aa</span>
					<code class="type-label">{step.label}</code>
				</div>
			{/each}
		</div>
	</section>

	<section class="section">
		<h2 class="section-title">Spacing Scale</h2>
		<div class="spacing-list">
			{#each spacing as step (step.var)}
				<div class="spacing-row">
					<div class="spacing-bar" style="width: {step.px}px"></div>
					<code class="spacing-label">{step.label}</code>
				</div>
			{/each}
		</div>
	</section>

	<section class="section">
		<h2 class="section-title">Border Radius</h2>
		<div class="radius-list">
			{#each radii as r (r.var)}
				<div class="radius-row">
					<div class="radius-swatch" style="border-radius: var({r.var})"></div>
					<code class="radius-label">{r.label}</code>
				</div>
			{/each}
		</div>
	</section>
</div>

<style>
	.tokens-page {
		display: flex;
		flex-direction: column;
		gap: var(--space-12);
		padding: var(--space-8);
		background-color: var(--color-surface-deep);
		min-height: 100vh;
		color: var(--color-text-primary);
		font-family: var(--font-family-primary);
	}

	.section-title {
		font-size: var(--font-size-xl);
		font-weight: var(--font-weight-bold);
		color: var(--color-text-primary);
		margin: 0 0 var(--space-4);
		border-bottom: 1px solid var(--color-border-subtle);
		padding-bottom: var(--space-3);
	}

	/* Colors */
	.color-grid {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
		gap: var(--space-4);
	}

	.color-swatch {
		display: flex;
		flex-direction: column;
		gap: var(--space-1);
	}

	.swatch {
		width: 100%;
		height: 56px;
		border-radius: var(--radius-sm);
		border: 1px solid var(--color-border-subtle);
	}

	.swatch-label {
		font-size: var(--font-size-sm);
		font-weight: var(--font-weight-medium);
		color: var(--color-text-primary);
	}

	.swatch-var {
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
	}

	/* Type */
	.type-list {
		display: flex;
		flex-direction: column;
		gap: var(--space-3);
	}

	.type-row {
		display: flex;
		align-items: center;
		gap: var(--space-4);
	}

	.type-sample {
		font-family: var(--font-family-primary);
		font-weight: var(--font-weight-semibold);
		color: var(--color-text-primary);
		min-width: 60px;
	}

	.type-label {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}

	/* Spacing */
	.spacing-list {
		display: flex;
		flex-direction: column;
		gap: var(--space-3);
	}

	.spacing-row {
		display: flex;
		align-items: center;
		gap: var(--space-4);
	}

	.spacing-bar {
		height: var(--space-4);
		background-color: var(--color-brand-lighter);
		border-radius: 2px;
		flex-shrink: 0;
	}

	.spacing-label {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}

	/* Radius */
	.radius-list {
		display: flex;
		gap: var(--space-6);
		flex-wrap: wrap;
	}

	.radius-row {
		display: flex;
		flex-direction: column;
		align-items: center;
		gap: var(--space-2);
	}

	.radius-swatch {
		width: 80px;
		height: 80px;
		background-color: var(--color-surface-raised);
		border: 2px solid var(--color-brand-lighter);
	}

	.radius-label {
		font-size: var(--font-size-xs);
		color: var(--color-text-secondary);
	}
</style>
