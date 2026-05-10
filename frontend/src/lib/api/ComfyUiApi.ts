import BaseApi from './baseApi';

export type ComfyUiWorkflow = {
	id: string;
	name: string;
};

export type GenerateResult = {
	imageBase64: string;
};

export default class ComfyUiApi extends BaseApi {
	constructor() {
		super();
	}

	async getWorkflows(): Promise<ComfyUiWorkflow[]> {
		return this.Get<ComfyUiWorkflow[]>('/api/comfyui/workflows');
	}

	async createWorkflow(name: string, workflowJson: string): Promise<ComfyUiWorkflow> {
		return this.Post<ComfyUiWorkflow>('/api/comfyui/workflows', { name, workflowJson });
	}

	async deleteWorkflow(id: string): Promise<void> {
		return this.Delete<void>(`/api/comfyui/workflows/${id}`);
	}

	async generateImage(
		workflowId: string,
		prompt: string,
		negativePrompt: string
	): Promise<GenerateResult> {
		return this.Post<GenerateResult>('/api/comfyui/generate', {
			workflowId,
			prompt,
			negativePrompt
		});
	}
}
