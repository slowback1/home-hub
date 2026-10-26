<script lang="ts">
	import MessageBus from '$lib/bus/MessageBus';
	import { Messages } from '$lib/bus/Messages';
	import type { AppUser } from '$lib/api/UsersApi';
	import Button from '$lib/ui/buttons/Button/Button.svelte';
	import Heading from '$lib/ui/typography/Heading/Heading.svelte';

	export let users: AppUser[] = [];

	const HEADING_LEVEL_2 = 2;

	function selectUser(userId: string) {
		MessageBus.sendMessage(Messages.CurrentUser, userId);
	}
</script>

<div class="user-switcher" data-testid="user-switcher">
	<Heading level={HEADING_LEVEL_2}>Who's using HomeHub?</Heading>
	<div class="user-options">
		{#each users as user (user.id)}
			<Button
				variant="secondary"
				testId={`user-option-${user.id}`}
				onClick={() => selectUser(user.id)}
			>
				{user.name}
			</Button>
		{/each}
	</div>
</div>

<style>
	.user-switcher {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: var(--space-6);
		min-height: 100vh;
		padding: var(--space-6);
	}

	.user-options {
		display: flex;
		flex-wrap: wrap;
		justify-content: center;
		gap: var(--space-4);
	}
</style>
