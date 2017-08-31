using Newtonsoft.Json;

namespace GLTF
{
	/// <summary>
	/// A texture and its sampler.
	/// </summary>
	public class Texture : GLTFChildOfRootProperty
	{
		/// <summary>
		/// The index of the sampler used by this texture.
		/// </summary>
		public SamplerId Sampler;

		/// <summary>
		/// The index of the image used by this texture.
		/// </summary>
		public ImageId Source;

		public static Texture Deserialize(GLTFRoot root, JsonReader reader)
		{
			var texture = new Texture();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "sampler":
						texture.Sampler = SamplerId.Deserialize(root, reader);
						break;
					case "source":
						texture.Source = ImageId.Deserialize(root, reader);
						break;
					default:
						texture.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return texture;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			if (Sampler != null)
			{
				writer.WritePropertyName("sampler");
				writer.WriteValue(Sampler.Id);
			}

			if (Source != null)
			{
				writer.WritePropertyName("source");
				writer.WriteValue(Source.Id);
			}

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
