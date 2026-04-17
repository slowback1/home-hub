import * as fs from 'fs';
import * as path from 'path';

const CONFIG_PATH = path.resolve(__dirname, '../frontend/static/config/config.json');
const BACKUP_PATH = CONFIG_PATH + '.bak';

const E2E_CONFIG = {
	baseUrl: 'http://localhost:5272',
	bigBeanSpawnChance: 0.05,
	bigBeanTimeoutMs: 2000,
	featureFlags: [
		{ name: 'DEMO_FEATURE_FLAG', isEnabled: false },
		{ name: 'BIG_BEAN', isEnabled: true },
		{ name: 'CHEAT_CODES', isEnabled: true }
	]
};

export default async function globalSetup(): Promise<void> {
	// Back up the existing config
	if (fs.existsSync(CONFIG_PATH)) {
		fs.copyFileSync(CONFIG_PATH, BACKUP_PATH);
	}

	// Write the deterministic E2E config
	fs.writeFileSync(CONFIG_PATH, JSON.stringify(E2E_CONFIG, null, 2));
}
