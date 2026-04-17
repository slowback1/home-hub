<script lang="ts">
	import ToggleSwitch from '$lib/ui/inputs/ToggleSwitch/ToggleSwitch.svelte';
	import ThemeService, { ColorTheme } from '$lib/services/Theme/ThemeService';
	import { onMount } from 'svelte';
	import MessageBus from '$lib/bus/MessageBus';
	import { Messages } from '$lib/bus/Messages';

	let themeService: ThemeService;

	function onToggle() {
		const currentTheme = MessageBus.getLastMessage(Messages.CurrentTheme) ?? ColorTheme.Light;

		const newTheme = currentTheme === ColorTheme.Dark ? ColorTheme.Light : ColorTheme.Dark;

		themeService.changeTheme(newTheme);
	}

	const labelMap = {
		[ColorTheme.Dark]: 'Turn on the Lights',
		[ColorTheme.Light]: 'Turn off the Lights'
	};

	let currentTheme: ColorTheme =
		MessageBus.getLastMessage(Messages.CurrentTheme) ?? ColorTheme.Light;

	onMount(() => {
		themeService = new ThemeService();

		MessageBus.subscribe(
			Messages.CurrentTheme,
			(value) => (currentTheme = value ?? ColorTheme.Light)
		);
	});
</script>

<ToggleSwitch
	label={labelMap[currentTheme]}
	id="theme-toggle"
	checked={currentTheme === ColorTheme.Dark}
	onClick={onToggle}
/>
