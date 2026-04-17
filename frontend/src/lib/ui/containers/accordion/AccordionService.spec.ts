import AccordionService, {
	type AccordionConfig
} from '$lib/ui/containers/accordion/AccordionService';
import { beforeEach } from 'vitest';
import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';
import getLocalStorageMock from '$lib/testHelpers/localStorageMock';
import type IStorageProvider from '$lib/bus/providers/IStorageProvider';

describe('AccordionService', () => {
	let service: AccordionService;
	let storageProvider: IStorageProvider;

	beforeEach(() => {
		storageProvider = getLocalStorageMock();
		MessageBus.initialize(storageProvider);
		service = new AccordionService('test');
	});

	function getCurrentConfig() {
		return MessageBus.getLastMessage<Map<string, AccordionConfig>>(Messages.AccordionConfig);
	}

	it('initializes the config for the accordion in the message bus', () => {
		const config = getCurrentConfig();

		expect(config.has('test')).toEqual(true);
	});

	it('does not try to store the data in the storage provider', () => {
		const storedValue = storageProvider.getItem(Messages.AccordionConfig);

		expect(storedValue).toEqual(undefined);
	});

	it('does not reinitialize the message bus if a second accordion service is present', () => {
		new AccordionService('test2');

		const config = getCurrentConfig();

		expect(config.has('test')).toEqual(true);
		expect(config.has('test2')).toEqual(true);
	});

	it('can register a config item', () => {
		service.registerConfigItem('label 1');

		const config = getCurrentConfig().get('test');

		expect(config.items.length).toEqual(1);
		expect(config.items[0].name).toEqual('label 1');
		expect(config.items[0].isOpen).toEqual(false);
	});

	it('will not double-register a config item', () => {
		service.registerConfigItem('label 1');
		service.registerConfigItem('label 1');

		const config = getCurrentConfig().get('test');

		expect(config.items.length).toEqual(1);
	});

	describe('opening and closing items', () => {
		beforeEach(() => {
			service.registerConfigItem('1');
			service.registerConfigItem('2');
			service.registerConfigItem('3');
		});

		it('can open an item', () => {
			service.toggleItem('1');

			const config = getCurrentConfig().get('test');

			const item = config.items.find((i) => i.name === '1');

			expect(item.isOpen).toEqual(true);
		});

		it('toggling twice will close the item', () => {
			service.toggleItem('1');

			let config = getCurrentConfig().get('test');

			let item = config.items.find((i) => i.name === '1');

			expect(item.isOpen).toEqual(true);

			service.toggleItem('1');

			config = getCurrentConfig().get('test');

			item = config.items.find((i) => i.name === '1');

			expect(item.isOpen).toEqual(false);
		});

		it('toggling a different item will close the first item', () => {
			service.toggleItem('1');
			service.toggleItem('2');

			const config = getCurrentConfig().get('test');

			const item = config.items.find((i) => i.name === '1');

			expect(item.isOpen).toEqual(false);
		});

		it('throws an error if toggling an item that does not exist', () => {
			expect(() => service.toggleItem('i dont exist')).toThrow();
		});
	});
});
