import type { Writable } from 'svelte/store';
import { writable } from 'svelte/store';
import { commandsSingleton } from '$lib/utils/reactiveClasses/commandsSingleton';
import type { CommandTarget } from '$lib/utils/reactiveClasses/command';

type Subscribable<T> = Omit<Writable<T>, 'update' | 'set'>;
type ReactiveInstance<T> = T & Subscribable<T>;

type Command = (...args: unknown[]) => void;

const emptySet = Object.freeze(new Set<string>());

function collectCommandsNames(proto: unknown): Set<string> {
	if (proto === null || proto === Object.prototype) {
		return emptySet;
	}
	const currentCommands = (proto as CommandTarget<unknown>)[commandsSingleton] ?? emptySet;
	const parentCommands = collectCommandsNames(Object.getPrototypeOf(proto));
	return new Set([...currentCommands, ...parentCommands]);
}

function getReactiveCommands<TClass, TConstructorArgs>(
	store: Writable<TClass>,
	instance: TClass,
	commandsNames: Set<string>,
	constructor: {
		new (...args: TConstructorArgs[]): TClass;
	}
) {
	const reactiveCommandOf = (command: Command) => {
			return (...args: unknown[]) => {
				command(...args);
				store.set({ ...instance });
			};
		},
		commandPairs: [string, Command][] = [...commandsNames].map((it) => [
			it,
			constructor.prototype[it].bind(instance)
		]);

	return commandPairs.reduce((acc, [name, command]) => {
		return { ...acc, [name]: reactiveCommandOf(command) };
	}, {});
}

export function reactiveInstanceOf<TClass, TConstructorArgs extends unknown[]>(
	constructor: new (...args: TConstructorArgs) => TClass,
	...args: TConstructorArgs
): ReactiveInstance<TClass> {
	const instance = new constructor(...args);
	const store = writable(instance);

	const commandsNames: Set<string> = collectCommandsNames(constructor.prototype);
	const reactiveCommands = getReactiveCommands(store, instance, commandsNames, constructor);

	const reactiveInstance = {
		...instance,
		...reactiveCommands,
		subscribe: store.subscribe
	};

	Object.setPrototypeOf(reactiveInstance, Object.getPrototypeOf(instance));
	return reactiveInstance;
}
