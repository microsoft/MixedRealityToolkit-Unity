using System;
using Newtonsoft.Json;

namespace GLTF
{
	/// <summary>
	/// Reference to a texture.
	/// </summary>
	public class TextureInfo : GLTFProperty
	{
		/// <summary>
		/// The index of the texture.
		/// </summary>
		public TextureId Index;

		/// <summary>
		/// This integer value is used to construct a string in the format
		/// TEXCOORD_<set index> which is a reference to a key in
		/// mesh.primitives.attributes (e.g. A value of 0 corresponds to TEXCOORD_0).
		/// </summary>
		public int TexCoord = 0;

		public static TextureInfo Deserialize(GLTFRoot root, JsonReader reader)
		{
			var textureInfo = new TextureInfo();

			if (reader.Read() && reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Asset must be an object.");
			}

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "index":
						textureInfo.Index = TextureId.Deserialize(root, reader);
						break;
					case "texCoord":
						textureInfo.TexCoord = reader.ReadAsInt32().Value;
						break;
					default:
						textureInfo.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return textureInfo;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			SerializeProperties(writer);

			writer.WriteEndObject();
		}

		public void SerializeProperties(JsonWriter writer)
		{
			writer.WritePropertyName("index");
			writer.WriteValue(Index.Id);

			if (TexCoord != 0)
			{
				writer.WritePropertyName("texCoord");
				writer.WriteValue(TexCoord);
			}

			base.Serialize(writer);
		}
	}
}
