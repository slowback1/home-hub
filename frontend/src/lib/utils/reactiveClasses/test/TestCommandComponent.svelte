<script lang="ts">
	import { reactiveInstanceOf } from '$lib/utils/reactiveClasses/reactiveInstanceOf';
	import TestCommandClass from '$lib/utils/reactiveClasses/test/testCommandClass';

	let cmd = reactiveInstanceOf(TestCommandClass, 'test');
</script>

<p data-testid="test-command__value">{$cmd.value}</p>
<button on:click={() => cmd.clear()} data-testid="test-command__clear">Clear</button>
<button on:click={() => cmd.addLetter('a')} data-testid="test-command__add">Add A</button>
<button on:click={() => cmd.setValueInvisibly('asdf')} data-testid="test-command__set"
	>Set Value</button
>
