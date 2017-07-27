using System;
using Newtonsoft.Json;

namespace GLTF
{
	public class NormalTextureInfo : TextureInfo
	{
		/// <summary>
		/// The scalar multiplier applied to each normal vector of the texture.
		/// This value is ignored if normalTexture is not specified.
		/// This value is linear.
		/// </summary>
		public double Scale = 1.0f;

		public static new NormalTextureInfo Deserialize(GLTFRoot root, JsonReader reader)
		{
			var textureInfo = new NormalTextureInfo();

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
					case "scale":
						textureInfo.Scale = reader.ReadAsDouble().Value;
						break;
					default:
						textureInfo.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return textureInfo;
		}

		public override void Serialize(JsonWriter writer) {
			writer.WriteStartObject();

			if (Scale != 1.0f)
			{
				writer.WritePropertyName("scale");
				writer.WriteValue(Scale);
			}

			// Write the parent class' properties only.
			// Don't accidentally call write start/end object.
			base.SerializeProperties(writer);

			writer.WriteEndObject();
		}
	}
}
