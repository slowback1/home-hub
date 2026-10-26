import { fireEvent, render } from '@testing-library/svelte';
import { beforeEach } from 'vitest';
import UserSwitcher from './UserSwitcher.svelte';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';

const users = [
	{ id: 'u1', name: 'Alex' },
	{ id: 'u2', name: 'Sam' }
];

describe('UserSwitcher', () => {
	beforeEach(() => {
		MessageBus.clearAll();
	});

	it('renders one option per user', () => {
		const { getByText, queryByText } = render(UserSwitcher, { props: { users } });

		expect(getByText('Alex')).toBeInTheDocument();
		expect(getByText('Sam')).toBeInTheDocument();
		expect(queryByText('Bob')).not.toBeInTheDocument();
	});

	it('clicking a user sends the selection on the message bus', async () => {
		const { getByTestId } = render(UserSwitcher, { props: { users } });

		await fireEvent.click(getByTestId('user-option-u2'));

		expect(MessageBus.getLastMessage(Messages.CurrentUser)).toEqual('u2');
	});
});
