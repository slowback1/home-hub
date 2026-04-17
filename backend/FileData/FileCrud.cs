using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Interfaces;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace FileData;

public abstract class FileCrud<T> : ICrud<T> where T : class, IIdentifyable
{
	private static int _nextId = 1;
	private readonly string _fileName;
	private readonly object _lock = new();

	protected FileCrud(string dataDirectory = "data")
	{
		_fileName = Path.Combine(dataDirectory, $"{typeof(T).Name}.json");

		// Ensure data directory exists
		Directory.CreateDirectory(dataDirectory);
	}

	public virtual Task<T> CreateAsync(T item)
	{
		lock (_lock)
		{
			var items = LoadItemsFromFile();
			item.Id = (_nextId++).ToString();
			items.Add(item);
			SaveItemsToFile(items);
			return Task.FromResult(item);
		}
	}

	public Task<T?> GetByIdAsync(string id)
	{
		lock (_lock)
		{
			var items = LoadItemsFromFile();
			var item = items.FirstOrDefault(i => i.Id == id);
			return Task.FromResult(item);
		}
	}

	public Task<T?> UpdateAsync(string id, T item)
	{
		lock (_lock)
		{
			var items = LoadItemsFromFile();
			var existingItem = items.FirstOrDefault(i => i.Id == id);
			if (existingItem == null) return Task.FromResult<T?>(null);

			var index = items.IndexOf(existingItem);
			if (index < 0) return Task.FromResult<T?>(null);

			item.Id = id; // Ensure the ID is preserved
			items[index] = item;
			SaveItemsToFile(items);
			return Task.FromResult(item)!;
		}
	}

	public Task<bool> DeleteAsync(string id)
	{
		lock (_lock)
		{
			var items = LoadItemsFromFile();
			var item = items.FirstOrDefault(i => i.Id == id);
			if (item == null) return Task.FromResult(false);

			items.Remove(item);
			SaveItemsToFile(items);
			return Task.FromResult(true);
		}
	}

	public Task<T?> GetByQueryAsync(Func<T, bool> query)
	{
		lock (_lock)
		{
			var items = LoadItemsFromFile();
			var item = items.FirstOrDefault(query);
			return Task.FromResult(item);
		}
	}

	public Task<IEnumerable<T>> QueryAsync(Func<T, bool> query)
	{
		lock (_lock)
		{
			var items = LoadItemsFromFile();
			var results = items.Where(query).ToList();
			return Task.FromResult(results.AsEnumerable());
		}
	}

	private List<T> LoadItemsFromFile()
	{
		if (!File.Exists(_fileName))
			return new List<T>();

		try
		{
			var json = File.ReadAllText(_fileName);
			if (string.IsNullOrWhiteSpace(json))
				return new List<T>();

			var items = JsonSerializer.Deserialize<List<T>>(json);
			return items ?? new List<T>();
		}
		catch (JsonException)
		{
			// If JSON is corrupted, start with empty list
			return new List<T>();
		}
	}

	private void SaveItemsToFile(List<T> items)
	{
		var options = new JsonSerializerOptions
		{
			WriteIndented = true
		};

		var json = JsonSerializer.Serialize(items, options);
		File.WriteAllText(_fileName, json);
	}

	public void ClearData()
	{
		lock (_lock)
		{
			if (File.Exists(_fileName)) File.Delete(_fileName);
			_nextId = 1;
		}
	}
}