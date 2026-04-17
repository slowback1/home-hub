import ToastService, {
	type ToastConfig,
	ToastVariant
} from '$lib/ui/containers/toast/ToastService';
import { beforeEach, expect } from 'vitest';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';

describe('ToastService', () => {
	let service: ToastService;

	beforeEach(() => {
		MessageBus.clearAll();
		service = new ToastService();
	});

	function getCurrentToasts(): ToastConfig[] {
		return MessageBus.getLastMessage(Messages.Toasts);
	}

	it('can add a new toast to the message bus', () => {
		service.AddToast({
			message: 'hello world'
		});

		const toasts = getCurrentToasts();

		expect(toasts.length).toEqual(1);
		expect(toasts[0].message).toEqual('hello world');
	});

	it('can add multiple toasts to the message bus', () => {
		service.AddToast({
			message: 'hello world'
		});
		service.AddToast({
			message: 'hello world'
		});

		const toasts = getCurrentToasts();

		expect(toasts.length).toEqual(2);
	});

	it('defaults the variant to "info"', () => {
		service.AddToast({
			message: 'hello world'
		});

		const toasts = getCurrentToasts();

		expect(toasts[0].variant).toEqual(ToastVariant.info);
	});

	it.each([ToastVariant.error, ToastVariant.info, ToastVariant.success, ToastVariant.warning])(
		'can set the variant to %s',
		(variant) => {
			service.AddToast({
				message: `variant ${variant}`,
				variant
			});

			const toasts = getCurrentToasts();

			expect(toasts[0].variant).toEqual(variant);
		}
	);

	function addTestToast(message: string = 'hello') {
		service.AddToast({
			message
		});
	}

	it('can remove a toast', () => {
		addTestToast();

		service.RemoveToast(0);

		const toasts = getCurrentToasts();

		expect(toasts.length).toEqual(0);
	});

	it('removes the correct toast when there are multiple', () => {
		addTestToast();
		addTestToast('goodbye');

		service.RemoveToast(1);

		const toasts = getCurrentToasts();

		expect(toasts.length).toEqual(1);
		expect(toasts[0].message).toEqual('hello');
	});
});
