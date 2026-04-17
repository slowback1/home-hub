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
	.heading__1 {
		font-size: 48px;
	}

	.heading__2 {
		font-size: 40px;
	}

	.heading__3 {
		font-size: 32px;
	}

	.heading__4 {
		font-size: 24px;
	}

	.heading__5 {
		font-size: 20px;
	}

	.heading__6 {
		font-size: 16px;
	}

	.heading__bold {
		font-weight: bold;
	}
</style>
