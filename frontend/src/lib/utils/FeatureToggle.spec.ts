import { render } from '@testing-library/svelte';
import FeatureToggle from '$lib/utils/FeatureToggle.test.svelte';
import TestFeatureFlagProvider, {
	createTestFeatureFlag
} from '$lib/testHelpers/testFeatureFlagProvider';
import FeatureFlagService from '$lib/services/FeatureFlag/FeatureFlagService';

describe('FeatureToggle', () => {
	it.each([
		{ isEnabled: true, idToFind: 'enabled' },
		{ isEnabled: false, idToFind: 'disabled' }
	])(
		'when the feature is %isEnabled% then it should render the %idToFind% element',
		async ({ isEnabled, idToFind }) => {
			const provider = new TestFeatureFlagProvider([createTestFeatureFlag('test', isEnabled)]);
			await FeatureFlagService.initialize(provider);

			const featureToggle = render(FeatureToggle);

			expect(featureToggle.getByTestId(idToFind)).toBeInTheDocument();
		}
	);
});
