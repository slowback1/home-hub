import BaseApi from './baseApi';

export type NbaGame = {
	id: string;
	sourceGameId: string | null;
	startAt: string;
	homeTeam: string;
	awayTeam: string;
};

export default class NbaApi extends BaseApi {
	constructor() {
		super();
	}

	async getSchedule(from?: Date, to?: Date): Promise<NbaGame[]> {
		const queryParameters: Record<string, string> = {};
		if (from) queryParameters.from = from.toISOString();
		if (to) queryParameters.to = to.toISOString();

		return this.Get<NbaGame[]>('/api/nba/schedule', queryParameters);
	}
}
