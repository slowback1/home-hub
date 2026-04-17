import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';

export enum ToastVariant {
	info,
	warning,
	error,
	success
}

export type ToastConfig = {
	message: string;
	variant?: ToastVariant;
};

export default class ToastService {
	AddToast(config: ToastConfig) {
		const toasts = this.getCurrentToasts();

		if (!config.variant) config.variant = ToastVariant.info;

		MessageBus.sendMessage(Messages.Toasts, [...toasts, config]);
	}

	RemoveToast(index: number) {
		const toasts = this.getCurrentToasts();

		toasts.splice(index, 1);

		MessageBus.sendMessage(Messages.Toasts, toasts);
	}

	private getCurrentToasts(): ToastConfig[] {
		const toasts = MessageBus.getLastMessage<ToastConfig[]>(Messages.Toasts);

		return toasts ?? [];
	}
}
