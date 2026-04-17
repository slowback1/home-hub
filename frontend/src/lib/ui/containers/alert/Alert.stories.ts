import type { Meta } from '@storybook/svelte';
import Alert from '$lib/ui/containers/alert/Alert.svelte';
import { type AlertProps, AlertType } from '$lib/ui/containers/alert/alertTypes';

const meta: Meta = {
	title: 'UI/Alert',
	component: Alert as never
};

export default meta;

function makeProps(overrides: Partial<AlertProps> = {}): AlertProps {
	return {
		onClose: () => {},
		message: 'I am an alert message!',
		...overrides
	};
}

export const Info = {
	args: makeProps({ type: AlertType.Info })
};

export const Warning = {
	args: makeProps({ type: AlertType.Warning })
};

export const Error = {
	args: makeProps({ type: AlertType.Error })
};
