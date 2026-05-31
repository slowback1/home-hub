import BaseApi from './baseApi';

export type WalkSession = {
	id: string;
	clientId: string;
	startedAt: string;
	durationSeconds: number;
	stepCount: number;
	syncedAt: string;
};

export default class WalkSessionApi extends BaseApi {
	constructor() {
		super();
	}

	async listSessions(): Promise<WalkSession[]> {
		return this.Get<WalkSession[]>('/api/walk-sessions');
	}
}
