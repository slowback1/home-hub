import { commandsSingleton } from '$lib/utils/reactiveClasses/commandsSingleton';

export type CommandTarget<T> = T & { [commandsSingleton]?: Set<string> };

export function command<T, S>(target: CommandTarget<T>, name: string, descriptor: S): S {
	if (target[commandsSingleton] === undefined) {
		target[commandsSingleton] = new Set<string>();
	}

	target[commandsSingleton].add(name);
	return descriptor;
}
