export type FeatureFlag = {
	name: string;
	isEnabled: boolean;
};

export default interface IFeatureFlagProvider {
	getFeatureFlags(): Promise<FeatureFlag[]>;
}
