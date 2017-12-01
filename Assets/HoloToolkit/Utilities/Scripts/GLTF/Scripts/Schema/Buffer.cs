using Newtonsoft.Json;

namespace GLTF
{
	/// <summary>
	/// A buffer points to binary geometry, animation, or skins.
	/// </summary>
	public class Buffer : GLTFChildOfRootProperty
	{
		/// <summary>
		/// The URI of the buffer.
		/// Relative paths are relative to the .gltf file.
		/// Instead of referencing an external file, the URI can also be a data-URI.
		/// </summary>
		public string Uri;

		/// <summary>
		/// The length of the buffer in bytes.
		/// <minimum>0</minimum>
		/// </summary>
		public int ByteLength;

		public static Buffer Deserialize(GLTFRoot root, JsonReader reader)
		{
			var buffer = new Buffer();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "uri":
						buffer.Uri = reader.ReadAsString();
						break;
					case "byteLength":
						buffer.ByteLength = reader.ReadAsInt32().Value;
						break;
					default:
						buffer.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return buffer;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			if (Uri != null)
			{
				writer.WritePropertyName("uri");
				writer.WriteValue(Uri);
			}

			writer.WritePropertyName("byteLength");
			writer.WriteValue(ByteLength);

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
