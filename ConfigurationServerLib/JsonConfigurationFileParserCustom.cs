using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConfigurationServerLib
{
	internal class JsonConfigurationFileParserCustom
	{
		private JsonConfigurationFileParserCustom() { }

		private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private readonly Stack<string> _context = new Stack<string>();
		private string _currentPath;
		private string _keyField;

		public static IDictionary<string, string> Parse(Stream input, string keyField)
			=> new JsonConfigurationFileParserCustom { _keyField = keyField }.ParseStream(input);

		private IDictionary<string, string> ParseStream(Stream input)
		{
			_data.Clear();

			var jsonDocumentOptions = new JsonDocumentOptions
			{
				CommentHandling = JsonCommentHandling.Skip,
				AllowTrailingCommas = true,
			};

			using (var reader = new StreamReader(input))
			using (JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
			{
				if (doc.RootElement.ValueKind != JsonValueKind.Object)
				{
					throw new FormatException($"Unsupported JSON token '{doc.RootElement.ValueKind}' was found");
				}
				VisitElement(doc.RootElement);
			}

			return _data;
		}

		private void VisitElement(JsonElement element)
		{
			foreach (var property in element.EnumerateObject())
			{
				EnterContext(property.Name);
				VisitValue(property.Value);
				ExitContext();
			}
		}

		private void VisitValue(JsonElement value)
		{
			switch (value.ValueKind)
			{
				case JsonValueKind.Object:
					VisitElement(value);
					break;

				case JsonValueKind.Array:
					var index = 0;
					foreach (var arrayElement in value.EnumerateArray())
					{
						var propertyName = arrayElement.ValueKind == JsonValueKind.Object && arrayElement.TryGetProperty(this._keyField, out var property) ? property.GetString() : index++.ToString();

						EnterContext(propertyName);
						VisitValue(arrayElement);
						ExitContext();
					}
					break;

				case JsonValueKind.Number:
				case JsonValueKind.String:
				case JsonValueKind.True:
				case JsonValueKind.False:
				case JsonValueKind.Null:
					var key = _currentPath;
					if (_data.ContainsKey(key))
					{
						throw new FormatException($"A duplicate key '{key}' was found.");
					}
					_data[key] = value.ToString();
					break;

				default:
					throw new FormatException($"Unsupported JSON token '{value}' was found");
			}
		}

		private void EnterContext(string context)
		{
			_context.Push(context);
			_currentPath = ConfigurationPath.Combine(_context.Reverse());
		}

		private void ExitContext()
		{
			_context.Pop();
			_currentPath = ConfigurationPath.Combine(_context.Reverse());
		}
	}
}
