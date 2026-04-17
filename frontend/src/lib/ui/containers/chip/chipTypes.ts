export type ChipProps = {
	testId?: string;
	text: string;
	variant?: ChipVariant;
	size?: ChipSize;
};

export enum ChipVariant {
	Primary,
	Secondary,
	Outline,
	Success,
	Error
}

export enum ChipSize {
	Small,
	Medium,
	Large
}
