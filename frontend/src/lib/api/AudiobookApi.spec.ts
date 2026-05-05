import { mockApi } from '$lib/testHelpers/getFetchMock';
import AudiobookApi, { type AudiobookJob } from './AudiobookApi';

const mockJob: AudiobookJob = {
	id: 'job-1',
	status: 'queued',
	epubFilename: 'book.epub',
	voiceSampleName: 'default',
	createdAt: '2026-05-05T12:00:00Z',
	updatedAt: '2026-05-05T12:00:00Z',
	errorMessage: null
};

describe('AudiobookApi', () => {
	describe('listJobs', () => {
		it('calls GET /api/audiobook/jobs and returns AudiobookJob[]', async () => {
			const mock = mockApi({ '/api/audiobook/jobs': [mockJob] });
			const api = new AudiobookApi();

			const result = await api.listJobs();

			const [url, options] = mock.mock.calls[0] as never as [string, RequestInit];
			expect(url).toContain('/api/audiobook/jobs');
			expect((options as RequestInit).method).toEqual('GET');
			expect(result).toEqual([mockJob]);
		});
	});

	describe('getJob', () => {
		it('calls GET /api/audiobook/jobs/{id}', async () => {
			const mock = mockApi({ '/api/audiobook/jobs/job-1': mockJob });
			const api = new AudiobookApi();

			const result = await api.getJob('job-1');

			const [url] = mock.mock.calls[0] as never as [string, RequestInit];
			expect(url).toContain('/api/audiobook/jobs/job-1');
			expect(result).toEqual(mockJob);
		});
	});

	describe('cancelJob', () => {
		it('calls DELETE /api/audiobook/jobs/{id}', async () => {
			const mock = mockApi({ '/api/audiobook/jobs/job-1': {} });
			const api = new AudiobookApi();

			await api.cancelJob('job-1');

			const [url, options] = mock.mock.calls[0] as never as [string, RequestInit];
			expect(url).toContain('/api/audiobook/jobs/job-1');
			expect((options as RequestInit).method).toEqual('DELETE');
		});
	});

	describe('getFileUrl', () => {
		it('returns the correct URL without making a fetch call', () => {
			const api = new AudiobookApi();
			const url = api.getFileUrl('job-1');
			expect(url).toContain('/api/audiobook/jobs/job-1/file');
		});
	});

	describe('deleteFile', () => {
		it('calls DELETE /api/audiobook/jobs/{id}/file', async () => {
			const mock = mockApi({ '/api/audiobook/jobs/job-1/file': {} });
			const api = new AudiobookApi();

			await api.deleteFile('job-1');

			const [url, options] = mock.mock.calls[0] as never as [string, RequestInit];
			expect(url).toContain('/api/audiobook/jobs/job-1/file');
			expect((options as RequestInit).method).toEqual('DELETE');
		});
	});

	describe('listVoiceSamples', () => {
		it('calls GET /api/audiobook/voice-samples and returns string[]', async () => {
			const mock = mockApi({ '/api/audiobook/voice-samples': ['default', 'narrator'] });
			const api = new AudiobookApi();

			const result = await api.listVoiceSamples();

			const [url] = mock.mock.calls[0] as never as [string, RequestInit];
			expect(url).toContain('/api/audiobook/voice-samples');
			expect(result).toEqual(['default', 'narrator']);
		});
	});

	describe('deleteVoiceSample', () => {
		it('calls DELETE /api/audiobook/voice-samples/{name}', async () => {
			const mock = mockApi({ '/api/audiobook/voice-samples/narrator': {} });
			const api = new AudiobookApi();

			await api.deleteVoiceSample('narrator');

			const [url, options] = mock.mock.calls[0] as never as [string, RequestInit];
			expect(url).toContain('/api/audiobook/voice-samples/narrator');
			expect((options as RequestInit).method).toEqual('DELETE');
		});
	});

	describe('submitJob', () => {
		it('calls POST /api/audiobook/jobs with multipart form data', async () => {
			const mockFetch = vi.fn().mockResolvedValue({
				json: () => Promise.resolve(mockJob),
				status: 201
			});
			global.fetch = mockFetch as unknown as typeof fetch;
			const api = new AudiobookApi();
			const file = new File(['epub content'], 'book.epub', { type: 'application/epub+zip' });

			const result = await api.submitJob(file, 'default');

			const [url, options] = mockFetch.mock.calls[0] as [string, RequestInit];
			expect(url).toContain('/api/audiobook/jobs');
			expect((options as RequestInit).method).toEqual('POST');
			expect((options as RequestInit).body).toBeInstanceOf(FormData);
			expect(result).toEqual(mockJob);
		});
	});

	describe('uploadVoiceSample', () => {
		it('calls POST /api/audiobook/voice-samples with multipart form data', async () => {
			const mockFetch = vi.fn().mockResolvedValue({
				json: () => Promise.resolve(undefined),
				status: 201
			});
			global.fetch = mockFetch as unknown as typeof fetch;
			const api = new AudiobookApi();
			const file = new File([new Uint8Array(4)], 'narrator.wav', { type: 'audio/wav' });

			await api.uploadVoiceSample(file, 'narrator');

			const [url, options] = mockFetch.mock.calls[0] as [string, RequestInit];
			expect(url).toContain('/api/audiobook/voice-samples');
			expect((options as RequestInit).method).toEqual('POST');
			expect((options as RequestInit).body).toBeInstanceOf(FormData);
		});
	});
});
