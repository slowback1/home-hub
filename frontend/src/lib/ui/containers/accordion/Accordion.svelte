<script lang="ts">
	import AccordionService from '$lib/ui/containers/accordion/AccordionService';

	export let name: string;
	const accordionService: AccordionService = new AccordionService(name);
</script>

<div class="accordion">
	<slot {accordionService} />
</div>

<style>
	.accordion {
		display: flex;
		flex-direction: column;
		gap: 12px;
	}
</style>
