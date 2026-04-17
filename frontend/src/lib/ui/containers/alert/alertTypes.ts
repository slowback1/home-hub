export type AlertProps = {
	message: string;
	testId?: string;
	type?: AlertType;
	onClose?: () => void;
};

export enum AlertType {
	Info = 0,
	Warning = 1,
	Error = 2
}
