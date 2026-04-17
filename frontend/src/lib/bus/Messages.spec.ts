import { Messages } from '$lib/bus/Messages';

describe('Message List', () => {
	it('every message is unique', () => {
		const values = Object.values(Messages);

		values.forEach((value, index, array) => {
			if (array.indexOf(value) !== index)
				expect.fail(
					`Value ${value} appears twice in the message list!  Make sure each message is unique`
				);
		});
	});
});
