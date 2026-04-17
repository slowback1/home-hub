<script>
	import Accordion from '$lib/ui/containers/accordion/Accordion.svelte';
	import AccordionItem from '$lib/ui/containers/accordion/AccordionItem.svelte';
</script>

<Accordion name="test" let:accordionService>
	<AccordionItem {accordionService} name="label 1">Content 1</AccordionItem>
	<AccordionItem {accordionService} name="label 2">Content 2</AccordionItem>
	<AccordionItem {accordionService} name="label 3">Content 3</AccordionItem>
</Accordion>
