import BaseApi from './baseApi';
import ConfigService from '$lib/services/Config/ConfigService';

export type AudiobookJobStatus = 'queued' | 'in_progress' | 'completed' | 'failed' | 'cancelled';

export type AudiobookJob = {
	id: string;
	status: AudiobookJobStatus;
	epubFilename: string;
	voiceSampleName: string;
	createdAt: string;
	updatedAt: string;
	errorMessage: string | null;
};

export default class AudiobookApi extends BaseApi {
	constructor() {
		super();
	}

	async listJobs(): Promise<AudiobookJob[]> {
		return this.Get<AudiobookJob[]>('/api/audiobook/jobs');
	}

	async getJob(id: string): Promise<AudiobookJob> {
		return this.Get<AudiobookJob>(`/api/audiobook/jobs/${id}`);
	}

	async cancelJob(id: string): Promise<void> {
		return this.Delete<void>(`/api/audiobook/jobs/${id}`);
	}

	async deleteJob(id: string): Promise<void> {
		return this.Delete<void>(`/api/audiobook/jobs/${id}`);
	}

	getFileUrl(id: string): string {
		const config = new ConfigService();
		const base = (config.getConfig<string>('baseUrl') ?? '').replace(/\/$/, '');
		return `${base}/api/audiobook/jobs/${id}/file`;
	}

	async deleteFile(id: string): Promise<void> {
		return this.Delete<void>(`/api/audiobook/jobs/${id}/file`);
	}

	async submitJob(epubFile: File, voiceSampleName: string): Promise<AudiobookJob> {
		const config = new ConfigService();
		const base = (config.getConfig<string>('baseUrl') ?? '').replace(/\/$/, '');
		const form = new FormData();
		form.append('epubFile', epubFile, epubFile.name);
		form.append('voiceSampleName', voiceSampleName);
		const response = await fetch(`${base}/api/audiobook/jobs`, { method: 'POST', body: form });
		return response.json();
	}

	async listVoiceSamples(): Promise<string[]> {
		return this.Get<string[]>('/api/audiobook/voice-samples');
	}

	async uploadVoiceSample(file: File, name: string): Promise<void> {
		const config = new ConfigService();
		const base = (config.getConfig<string>('baseUrl') ?? '').replace(/\/$/, '');
		const form = new FormData();
		form.append('file', file, file.name);
		form.append('name', name);
		await fetch(`${base}/api/audiobook/voice-samples`, { method: 'POST', body: form });
	}

	async deleteVoiceSample(name: string): Promise<void> {
		return this.Delete<void>(`/api/audiobook/voice-samples/${encodeURIComponent(name)}`);
	}
}
