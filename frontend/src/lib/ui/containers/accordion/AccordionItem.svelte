<script lang="ts">
	import type {
		default as AccordionService,
		AccordionConfig
	} from '$lib/ui/containers/accordion/AccordionService';
	import { onMount } from 'svelte';
	import MessageBus from '$lib/bus/MessageBus';
	import { Messages } from '$lib/bus/Messages';
	import { slugify } from '$lib/utils/stringUtils';

	export let accordionService: AccordionService;
	export let name: string;

	let isOpen: boolean = false;

	onMount(() => {
		accordionService.registerConfigItem(name);

		MessageBus.subscribe<Map<string, AccordionConfig>>(Messages.AccordionConfig, (value) => {
			let item = value.get(accordionService.name).items.find((i) => i.name === name);

			isOpen = item?.isOpen ?? false;
		});
	});

	function onToggle() {
		accordionService.toggleItem(name);
	}

	let contentId = slugify(name);
	let labelId = `${slugify(name)}-control`;
</script>

<div data-testid="accordion-item">
	<button
		aria-expanded={isOpen}
		data-testid="accordion-item__label"
		on:click={onToggle}
		aria-controls={contentId}
		id={labelId}
		class="accordion-item__label"
	>
		<slot name="label">
			{name}
		</slot>
	</button>
	<div
		class="accordion-item__content"
		class:accordion-item__content-open={isOpen}
		data-testid="accordion-item__content"
		id={contentId}
		role="region"
		aria-labelledby={labelId}
	>
		<slot />
	</div>
</div>

<style>
	.accordion-item__content {
		display: none;
	}

	.accordion-item__content-open {
		display: block;
	}
</style>
