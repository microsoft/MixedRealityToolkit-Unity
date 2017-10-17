using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace GLTF.JsonExtensions
{
	public static class JsonReaderExtensions
	{
		public static List<string> ReadStringList(this JsonReader reader)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception(string.Format("Invalid array at: {0}", reader.Path));
			}

			var list = new List<string>();

			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				list.Add(reader.Value.ToString());
			}

			return list;
		}

		public static List<double> ReadDoubleList(this JsonReader reader)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception(string.Format("Invalid array at: {0}", reader.Path));
			}

			var list = new List<double>();

			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				list.Add(double.Parse(reader.Value.ToString()));
			}

			return list;
		}

		public static List<T> ReadList<T>(this JsonReader reader, Func<T> deserializerFunc)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception(string.Format("Invalid array at: {0}", reader.Path));
			}

			var list = new List<T>();

			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				list.Add(deserializerFunc());
			}

			return list;
		}

		public static Color ReadAsRGBAColor(this JsonReader reader)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception(string.Format("Invalid color value at: {0}", reader.Path));
			}

			var color = new Color
			{
				r = (float) reader.ReadAsDouble().Value,
				g = (float) reader.ReadAsDouble().Value,
				b = (float) reader.ReadAsDouble().Value,
				a = (float) reader.ReadAsDouble().Value
			};

			if (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				throw new Exception(string.Format("Invalid color value at: {0}", reader.Path));
			}

			return color;
		}

		public static Color ReadAsRGBColor(this JsonReader reader)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception(string.Format("Invalid vector value at: {0}", reader.Path));
			}

			var color = new Color
			{
				r = (float) reader.ReadAsDouble().Value,
				g = (float) reader.ReadAsDouble().Value,
				b = (float) reader.ReadAsDouble().Value,
				a = 1.0f
			};

			if (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				throw new Exception(string.Format("Invalid color value at: {0}", reader.Path));
			}

			return color;
		}

		public static Vector3 ReadAsVector3(this JsonReader reader)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception(string.Format("Invalid vector value at: {0}", reader.Path));
			}

			var vector = new Vector3
			{
				x = (float) reader.ReadAsDouble().Value,
				y = (float) reader.ReadAsDouble().Value,
				z = (float) reader.ReadAsDouble().Value
			};

			if (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				throw new Exception(string.Format("Invalid vector value at: {0}", reader.Path));
			}

			return vector;
		}

		public static Quaternion ReadAsQuaternion(this JsonReader reader)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception(string.Format("Invalid vector value at: {0}", reader.Path));
			}

			var quat = new Quaternion
			{
				x = (float) reader.ReadAsDouble().Value,
				y = (float) reader.ReadAsDouble().Value,
				z = (float) reader.ReadAsDouble().Value,
				w = (float) reader.ReadAsDouble().Value
			};

			if (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				throw new Exception(string.Format("Invalid vector value at: {0}", reader.Path));
			}

			return quat;
		}

		public static Dictionary<string, T> ReadAsDictionary<T>(this JsonReader reader, Func<T> deserializerFunc)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception(string.Format("Dictionary must be an object at: {0}", reader.Path));
			}

			var dict = new Dictionary<string, T>();

			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				dict.Add(reader.Value.ToString(), deserializerFunc());
			}

			return dict;
		}

		public static Dictionary<string, object> ReadAsObjectDictionary(this JsonReader reader, bool skipStartObjectRead = false)
		{
			if (!skipStartObjectRead && reader.Read() && reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception(string.Format("Dictionary must be an object at: {0}", reader.Path));
			}

			var dict = new Dictionary<string, object>();

			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
			   dict.Add(reader.Value.ToString(), ReadDictionaryValue(reader));
			}

			return dict;
		}

		private static object ReadDictionaryValue(JsonReader reader)
		{
			if (!reader.Read())
			{
				return null;
			}

			switch (reader.TokenType)
			{
				case JsonToken.StartArray:
					return reader.ReadObjectList();
				case JsonToken.StartObject:
					return reader.ReadAsObjectDictionary(true);
				default:
					return reader.Value;
			}
		}

		private static List<object> ReadObjectList(this JsonReader reader)
		{

			var list = new List<object>();

			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				list.Add(reader.Value);
			}

			return list;
		}

		public static T ReadStringEnum<T>(this JsonReader reader)
		{
			return (T) Enum.Parse(typeof(T), reader.ReadAsString());
		}
	}
}
