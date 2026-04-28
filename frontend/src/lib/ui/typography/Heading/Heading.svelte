<script lang="ts">
	type FontWeight = 'bold' | 'normal';

	const headingMap = {
		1: 'h1',
		2: 'h2',
		3: 'h3',
		4: 'h4',
		5: 'h5',
		6: 'h6'
	};

	export let level: number = 1;
	export let fontWeight: FontWeight = 'normal';

	const heading = headingMap[level];
</script>

<svelte:element
	this={heading}
	class={`heading heading__${level}`}
	class:heading__bold={fontWeight === 'bold'}
>
	<slot>Heading</slot>
</svelte:element>

<style>
	.heading {
		font-family: var(--font-family-primary);
		color: var(--color-text-primary);
		font-weight: var(--font-weight-semibold);
		line-height: var(--line-height-tight);
		margin: 0;
	}

	.heading__1 {
		font-size: var(--font-size-3xl);
	}

	.heading__2 {
		font-size: var(--font-size-2xl);
	}

	.heading__3 {
		font-size: var(--font-size-xl);
	}

	.heading__4 {
		font-size: var(--font-size-lg);
	}

	.heading__5 {
		font-size: var(--font-size-md);
	}

	.heading__6 {
		font-size: var(--font-size-sm);
		color: var(--color-text-secondary);
	}

	.heading__bold {
		font-weight: var(--font-weight-bold);
	}
</style>
