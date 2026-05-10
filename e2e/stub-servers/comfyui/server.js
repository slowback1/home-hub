// Minimal ComfyUI stub server for E2E tests.
// Responds to the subset of ComfyUI's REST API used by HomeHub.

const http = require('http');

const PORT = 8199;
const PROMPT_ID = 'test-prompt-123';

// 1x1 transparent PNG
const MINIMAL_PNG = Buffer.from(
	'iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==',
	'base64'
);

const HISTORY_RESPONSE = JSON.stringify({
	[PROMPT_ID]: {
		outputs: {
			9: {
				images: [{ filename: 'test.png', subfolder: '', type: 'output' }]
			}
		},
		status: { completed: true }
	}
});

let failMode = false;

function readBody(req) {
	return new Promise((resolve) => {
		let body = '';
		req.on('data', (chunk) => (body += chunk));
		req.on('end', () => resolve(body));
	});
}

const server = http.createServer(async (req, res) => {
	const { method, url } = req;

	// GET /health — readiness probe for Playwright webServer
	if (method === 'GET' && url === '/health') {
		res.writeHead(200);
		res.end('ok');
		return;
	}

	// POST /stub/fail — enable failure mode for next prompt
	if (method === 'POST' && url === '/stub/fail') {
		failMode = true;
		res.writeHead(200);
		res.end('ok');
		return;
	}

	// POST /prompt — submit a workflow
	if (method === 'POST' && url === '/prompt') {
		await readBody(req); // consume body
		if (failMode) {
			failMode = false;
			res.writeHead(500, { 'Content-Type': 'application/json' });
			res.end(JSON.stringify({ error: 'ComfyUI stub: forced failure' }));
			return;
		}
		res.writeHead(200, { 'Content-Type': 'application/json' });
		res.end(JSON.stringify({ prompt_id: PROMPT_ID, number: 1, node_errors: {} }));
		return;
	}

	// GET /history/:promptId — return completed output
	if (method === 'GET' && url.startsWith('/history/')) {
		res.writeHead(200, { 'Content-Type': 'application/json' });
		res.end(HISTORY_RESPONSE);
		return;
	}

	// GET /view — return minimal PNG image
	if (method === 'GET' && url.startsWith('/view')) {
		res.writeHead(200, { 'Content-Type': 'image/png' });
		res.end(MINIMAL_PNG);
		return;
	}

	res.writeHead(404);
	res.end('not found');
});

server.listen(PORT, () => {
	console.log(`ComfyUI stub server listening on http://localhost:${PORT}`);
});
