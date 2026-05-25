import BaseApi from './baseApi';

export type SlotAssignment = {
	slotIndex: number;
	widgetType: string | null;
};

export type LayoutResponse = {
	layoutFormat: string;
	slots: SlotAssignment[];
};

const ACTIVE_FORMAT = '3x2';

export default class DashboardApi extends BaseApi {
	async getLayout(): Promise<LayoutResponse> {
		return this.Get<LayoutResponse>('/api/dashboard/layout');
	}

	async saveLayout(slots: SlotAssignment[]): Promise<void> {
		return this.Put<void>('/api/dashboard/layout', { layoutFormat: ACTIVE_FORMAT, slots });
	}
}
