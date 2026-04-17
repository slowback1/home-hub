<script lang="ts">
	import ToastService, { ToastVariant } from '$lib/ui/containers/toast/ToastService';
	import ToastWrapper from '$lib/ui/containers/toast/ToastWrapper.svelte';
	import Button from '$lib/ui/buttons/Button/Button.svelte';

	function addTestToast(variant: ToastVariant) {
		let service = new ToastService();

		service.AddToast({ message: 'Test toast', variant });
	}
</script>

<ToastWrapper />

<Button onClick={() => addTestToast(ToastVariant.info)}>Info</Button>
<Button onClick={() => addTestToast(ToastVariant.success)}>Success</Button>
<Button onClick={() => addTestToast(ToastVariant.warning)}>Warning</Button>
<Button onClick={() => addTestToast(ToastVariant.error)}>Error</Button>
