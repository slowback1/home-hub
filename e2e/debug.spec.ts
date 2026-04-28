import { test, expect } from './fixtures';

test('debug sidebar collapse', async ({ page }) => {
    await page.goto('/');
    const nav = page.getByRole('navigation');
    await nav.waitFor({ state: 'visible' });
    
    const classBefore = await nav.getAttribute('class');
    const dataBefore = await nav.getAttribute('data-collapsed');
    console.log('Before - class:', classBefore, '| data-collapsed:', dataBefore);
    
    await page.getByTestId('sidebar-toggle').click();
    await page.waitForTimeout(1000);
    
    const classAfter = await nav.getAttribute('class');
    const dataAfter = await nav.getAttribute('data-collapsed');
    console.log('After - class:', classAfter, '| data-collapsed:', dataAfter);
});
