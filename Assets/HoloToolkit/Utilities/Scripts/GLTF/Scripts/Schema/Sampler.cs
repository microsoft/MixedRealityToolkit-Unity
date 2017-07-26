using Newtonsoft.Json;

namespace GLTF
{
	/// <summary>
	/// Texture sampler properties for filtering and wrapping modes.
	/// </summary>
	public class Sampler : GLTFChildOfRootProperty
	{
		/// <summary>
		/// Magnification filter.
		/// Valid values correspond to WebGL enums: `9728` (NEAREST) and `9729` (LINEAR).
		/// </summary>
		public MagFilterMode MagFilter = MagFilterMode.Linear;

		/// <summary>
		/// Minification filter. All valid values correspond to WebGL enums.
		/// </summary>
		public MinFilterMode MinFilter = MinFilterMode.NearestMipmapLinear;

		/// <summary>
		/// s wrapping mode.  All valid values correspond to WebGL enums.
		/// </summary>
		public WrapMode WrapS = WrapMode.Repeat;

		/// <summary>
		/// t wrapping mode.  All valid values correspond to WebGL enums.
		/// </summary>
		public WrapMode WrapT = WrapMode.Repeat;

		public static Sampler Deserialize(GLTFRoot root, JsonReader reader)
		{
			var sampler = new Sampler();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "magFilter":
						sampler.MagFilter = (MagFilterMode) reader.ReadAsInt32();
						break;
					case "minFilter":
						sampler.MinFilter = (MinFilterMode)reader.ReadAsInt32();
						break;
					case "wrapS":
						sampler.WrapS = (WrapMode)reader.ReadAsInt32();
						break;
					case "wrapT":
						sampler.WrapT = (WrapMode)reader.ReadAsInt32();
						break;
					default:
						sampler.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return sampler;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			if (MagFilter != MagFilterMode.Linear)
			{
				writer.WritePropertyName("magFilter");
				writer.WriteValue((int)MagFilter);
			}

			if (MinFilter != MinFilterMode.NearestMipmapLinear)
			{
				writer.WritePropertyName("minFilter");
				writer.WriteValue((int)MinFilter);
			}

			if (WrapS != WrapMode.Repeat)
			{
				writer.WritePropertyName("WrapS");
				writer.WriteValue((int)WrapS);
			}

			if (WrapT != WrapMode.Repeat)
			{
				writer.WritePropertyName("WrapT");
				writer.WriteValue((int)WrapT);
			}

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}

	/// <summary>
	/// Magnification filter mode.
	/// </summary>
	public enum MagFilterMode
	{
		None = 0,
		Nearest = 9728,
		Linear = 9729,
	}

	/// <summary>
	/// Minification filter mode.
	/// </summary>
	public enum MinFilterMode
	{
		None = 0,
		Nearest = 9728,
		Linear = 9729,
		NearestMipmapNearest = 9984,
		LinearMipmapNearest = 9985,
		NearestMipmapLinear = 9986,
		LinearMipmapLinear = 9987
	}

	/// <summary>
	/// Texture wrap mode.
	/// </summary>
	public enum WrapMode
	{
		None = 0,
		ClampToEdge = 33071,
		MirroredRepeat = 33648,
		Repeat = 10497
	}
}
