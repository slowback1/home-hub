import MessageBus from '$lib/bus/MessageBus';
import { Messages } from '$lib/bus/Messages';

export type AccordionConfig = {
	name: string;
	items: AccordionConfigItem[];
};

export type AccordionConfigItem = {
	name: string;
	isOpen: boolean;
};

export default class AccordionService {
	constructor(public name: string) {
		this.initializeMessageBus();
	}

	registerConfigItem(name: string) {
		const config = this.getCurrentConfig();

		const itemAlreadyExists = config.items.find((i) => i.name === name);
		if (itemAlreadyExists) return;

		config.items.push({
			name,
			isOpen: false
		});

		this.sendMessage(config);
	}

	toggleItem(itemName: string) {
		const config = this.getCurrentConfig();

		if (!config.items.find((i) => i.name === itemName))
			throw new Error(
				`Cannot toggle item ${itemName} that does not exist on accordion ${this.name}`
			);

		config.items.forEach((item) => {
			const isTheItemToToggle = item.name === itemName;

			item.isOpen = isTheItemToToggle ? !item.isOpen : false;
		});

		this.sendMessage(config);
	}

	private initializeMessageBus() {
		MessageBus.configure.doNotStoreDataForMessage(Messages.AccordionConfig);

		this.sendMessage({
			name: this.name,
			items: []
		});
	}

	private sendMessage(value: AccordionConfig) {
		let config = MessageBus.getLastMessage<Map<string, AccordionConfig>>(Messages.AccordionConfig);

		if (!config) config = new Map<string, AccordionConfig>();

		config.set(this.name, value);

		MessageBus.sendMessage(Messages.AccordionConfig, config);
	}

	private getCurrentConfig() {
		return MessageBus.getLastMessage<Map<string, AccordionConfig>>(Messages.AccordionConfig).get(
			this.name
		);
	}
}
