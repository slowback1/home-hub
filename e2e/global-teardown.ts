import * as fs from 'fs';
import * as path from 'path';

const CONFIG_PATH = path.resolve(__dirname, '../frontend/static/config/config.json');
const BACKUP_PATH = CONFIG_PATH + '.bak';

export default async function globalTeardown(): Promise<void> {
	if (fs.existsSync(BACKUP_PATH)) {
		fs.copyFileSync(BACKUP_PATH, CONFIG_PATH);
		fs.unlinkSync(BACKUP_PATH);
	}
}
