import type { TableColumnDefinition } from '$lib/ui/table/table/tableTypes';
import TestCustomRenderCell from '$lib/ui/table/table/testing/renderCells/TestCustomRenderCell.svelte';

export type TestTableData = {
	id: number;
	name: string;
	age: number;
};

export function getTestTableDefinitions(): TableColumnDefinition<TestTableData>[] {
	return [
		{
			key: 'id',
			title: 'ID',
			renderKey: 'id'
		},
		{
			key: 'name',
			title: 'Name',
			renderKey: 'name'
		},
		{
			key: 'age',
			title: 'Age',
			renderKey: 'age'
		},
		{
			key: 'custom',
			title: 'Custom',
			renderTemplate: TestCustomRenderCell
		}
	];
}

export function getTestTableData(): TestTableData[] {
	return [
		{
			id: 1,
			name: 'Rathalos',
			age: 25
		},
		{
			id: 2,
			name: 'Rathian',
			age: 30
		},
		{
			id: 3,
			name: 'Kushala Daora',
			age: 35
		},
		{
			id: 4,
			name: 'Teostra',
			age: 40
		},
		{
			id: 5,
			name: 'Lunastra',
			age: 45
		},
		{
			id: 6,
			name: 'Nergigante',
			age: 50
		},
		{
			id: 7,
			name: 'Vaal Hazak',
			age: 55
		},
		{
			id: 8,
			name: "Xeno'jiiva",
			age: 60
		},
		{
			id: 9,
			name: 'Zorah Magdaros',
			age: 65
		},
		{
			id: 10,
			name: 'Kulve Taroth',
			age: 70
		},
		{
			id: 11,
			name: 'Namielle',
			age: 75
		},
		{
			id: 12,
			name: 'Shara Ishvalda',
			age: 80
		},
		{
			id: 13,
			name: 'Yian Garuga',
			age: 85
		},
		{
			id: 14,
			name: 'Rajang',
			age: 90
		},
		{
			id: 15,
			name: 'Bazelgeuse',
			age: 95
		},
		{
			id: 16,
			name: 'Deviljho',
			age: 100
		},
		{
			id: 17,
			name: 'Glavenus',
			age: 105
		},
		{
			id: 18,
			name: 'Tigrex',
			age: 110
		},
		{
			id: 19,
			name: 'Nargacuga',
			age: 115
		},
		{
			id: 20,
			name: 'Zinogre',
			age: 120
		}
	];
}
