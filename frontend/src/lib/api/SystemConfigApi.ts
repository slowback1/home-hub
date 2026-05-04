import BaseApi from './baseApi';

export type SystemConfigOption = {
	value: string;
	label: string;
};

export type SystemConfig = {
	id: string;
	namespace: string;
	key: string;
	value: string;
	type: string;
	isSecret: boolean;
	options: SystemConfigOption[];
};

export default class SystemConfigApi extends BaseApi {
	constructor() {
		super();
	}

	async getAll(): Promise<SystemConfig[]> {
		return this.Get<SystemConfig[]>('/api/system-config');
	}

	async update(namespace: string, key: string, value: string): Promise<SystemConfig> {
		return this.Put<SystemConfig>(`/api/system-config/${namespace}/${key}`, { value });
	}
}
