using System;
using Newtonsoft.Json;

namespace GLTF
{
	/// <summary>
	/// A perspective camera containing properties to create a perspective projection
	/// matrix.
	/// </summary>
	public class CameraPerspective : GLTFProperty
	{
		/// <summary>
		/// The floating-point aspect ratio of the field of view.
		/// When this is undefined, the aspect ratio of the canvas is used.
		/// <minimum>0.0</minimum>
		/// </summary>
		public double AspectRatio;

		/// <summary>
		/// The floating-point vertical field of view in radians.
		/// <minimum>0.0</minimum>
		/// </summary>
		public double YFov;

		/// <summary>
		/// The floating-point distance to the far clipping plane. When defined,
		/// `zfar` must be greater than `znear`.
		/// If `zfar` is undefined, runtime must use infinite projection matrix.
		/// <minimum>0.0</minimum>
		/// </summary>
		public double ZFar = double.PositiveInfinity;

		/// <summary>
		/// The floating-point distance to the near clipping plane.
		/// <minimum>0.0</minimum>
		/// </summary>
		public double ZNear;

		public static CameraPerspective Deserialize(GLTFRoot root, JsonReader reader)
		{
			var cameraPerspective = new CameraPerspective();

			if (reader.Read() && reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Perspective camera must be an object.");
			}

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "aspectRatio":
						cameraPerspective.AspectRatio = reader.ReadAsDouble().Value;
						break;
					case "yfov":
						cameraPerspective.YFov = reader.ReadAsDouble().Value;
						break;
					case "zfar":
						cameraPerspective.ZFar = reader.ReadAsDouble().Value;
						break;
					case "znear":
						cameraPerspective.ZNear = reader.ReadAsDouble().Value;
						break;
					default:
						cameraPerspective.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return cameraPerspective;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			if (AspectRatio != 0)
			{
				writer.WritePropertyName("aspectRatio");
				writer.WriteValue(AspectRatio);
			}

			writer.WritePropertyName("yfov");
			writer.WriteValue(YFov);

			if (ZFar != double.PositiveInfinity)
			{
				writer.WritePropertyName("zfar");
				writer.WriteValue(ZFar);
			}

			writer.WritePropertyName("ZNear");
			writer.WriteValue(ZNear);

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
