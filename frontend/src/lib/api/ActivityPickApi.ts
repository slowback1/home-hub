import BaseApi from './baseApi';

export type ActivityPick = {
	id: string;
	activityName: string;
	pickedAt: string;
};

export default class ActivityPickApi extends BaseApi {
	constructor() {
		super();
	}

	async getCurrent(): Promise<ActivityPick> {
		return this.Get<ActivityPick>('/api/activity/current');
	}

	async getHistory(): Promise<ActivityPick[]> {
		return this.Get<ActivityPick[]>('/api/activity/history');
	}
}
