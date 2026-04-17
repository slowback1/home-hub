import type { Component } from 'svelte';

export type TableColumnDefinition<T> = {
	/**
	 * The key of the column, used when generating unique ids for the column header and any related body cells.  This should be unique across all columns in the same table.
	 * @example 'first-name'
	 */
	key: string;
	/**
	 * The title of the column, used for the column header
	 * @example 'First Name'
	 */
	title: string;
	/**
	 * The key of the row object to use when rendering the cell.  If all you need to do is display a property of the row object, use this.  If you need to do something more complex, use renderTemplate. If both renderKey and renderTemplate are provided, renderKey will be used.
	 * @example 'firstName'
	 */
	renderKey?: keyof T;
	/**
	 * A custom template to use when rendering the cell.  This template is a Svelte component that will be passed a row context object, with the row data, index of the row, and the column definition as props.  For more detailed information of what is passed as props, see the `TableRenderContext` type.  If both renderKey and renderTemplate are provided, renderKey will be used.
	 * @example ```js
	 *  import MyCustomCell from './MyCustomCell.svelte';
	 * {
	 *  renderTemplate: MyCustomCell
	 * }
	 * ```
	 */
	renderTemplate?: Component | never;
};

export type TableRenderContext<T> = {
	row: T;
	rowIndex: number;
	columnDefinition: TableColumnDefinition<T>;
};

export type TableProps<T> = {
	columns: TableColumnDefinition<T>[];
	rows: T[];
};
