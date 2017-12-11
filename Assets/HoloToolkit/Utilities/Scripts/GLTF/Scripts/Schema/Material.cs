using GLTF.JsonExtensions;
using Newtonsoft.Json;
using UnityEngine;

namespace GLTF
{
	/// <summary>
	/// The material appearance of a primitive.
	/// </summary>
	public class Material : GLTFChildOfRootProperty
	{
		/// <summary>
		/// A set of parameter values that are used to define the metallic-roughness
		/// material model from Physically-Based Rendering (PBR) methodology.
		/// </summary>
		public PbrMetallicRoughness PbrMetallicRoughness;

		/// <summary>
		/// A set of parameter values used to light flat-shaded materials
		/// </summary>
		public MaterialCommonConstant CommonConstant;

		/// <summary>
		/// A tangent space normal map. Each texel represents the XYZ components of a
		/// normal vector in tangent space.
		/// </summary>
		public NormalTextureInfo NormalTexture;

		/// <summary>
		/// The occlusion map is a gray-scale texture, with white indicating areas that
		/// should receive full indirect lighting and black indicating no indirect
		/// lighting.
		/// </summary>
		public OcclusionTextureInfo OcclusionTexture;

		/// <summary>
		/// The emissive map controls the color and intensity of the light being emitted
		/// by the material. This texture contains RGB components in sRGB color space.
		/// If a fourth component (A) is present, it is ignored.
		/// </summary>
		public TextureInfo EmissiveTexture;

		/// <summary>
		/// The RGB components of the emissive color of the material.
		/// If an emissiveTexture is specified, this value is multiplied with the texel
		/// values.
		/// <items>
		///	 <minimum>0.0</minimum>
		///	 <maximum>1.0</maximum>
		/// </items>
		/// <minItems>3</minItems>
		/// <maxItems>3</maxItems>
		/// </summary>
		public Color EmissiveFactor = Color.black;

		/// <summary>
		/// The material's alpha rendering mode enumeration specifying the interpretation of the
		/// alpha value of the main factor and texture. In `OPAQUE` mode, the alpha value is
		/// ignored and the rendered output is fully opaque. In `MASK` mode, the rendered output
		/// is either fully opaque or fully transparent depending on the alpha value and the
		/// specified alpha cutoff value. In `BLEND` mode, the alpha value is used to composite
		/// the source and destination areas. The rendered output is combined with the background
		/// using the normal painting operation (i.e. the Porter and Duff over operator).
		/// </summary>
		public AlphaMode AlphaMode = AlphaMode.OPAQUE;

		/// <summary>
		/// Specifies the cutoff threshold when in `MASK` mode. If the alpha value is greater than
		/// or equal to this value then it is rendered as fully opaque, otherwise, it is rendered
		/// as fully transparent. This value is ignored for other modes.
		/// </summary>
		public double AlphaCutoff = 0.5;

		/// <summary>
		/// Specifies whether the material is double sided. When this value is false, back-face
		/// culling is enabled. When this value is true, back-face culling is disabled and double
		/// sided lighting is enabled. The back-face must have its normals reversed before the
		/// lighting equation is evaluated.
		/// </summary>
		public bool DoubleSided;

		public static Material Deserialize(GLTFRoot root, JsonReader reader)
		{
			var material = new Material();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "pbrMetallicRoughness":
						material.PbrMetallicRoughness = PbrMetallicRoughness.Deserialize(root, reader);
						break;
					case "commonConstant":
						material.CommonConstant = MaterialCommonConstant.Deserialize(root, reader);
						break;
					case "normalTexture":
						material.NormalTexture = NormalTextureInfo.Deserialize(root, reader);
						break;
					case "occlusionTexture":
						material.OcclusionTexture = OcclusionTextureInfo.Deserialize(root, reader);
						break;
					case "emissiveTexture":
						material.EmissiveTexture = TextureInfo.Deserialize(root, reader);
						break;
					case "emissiveFactor":
						material.EmissiveFactor = reader.ReadAsRGBColor();
						break;
					case "alphaMode":
						material.AlphaMode = reader.ReadStringEnum<AlphaMode>();
						break;
					case "alphaCutoff":
						material.AlphaCutoff = reader.ReadAsDouble().Value;
						break;
					case "doubleSided":
						material.DoubleSided = reader.ReadAsBoolean().Value;
						break;
					default:
						material.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return material;
		}

		public override void Serialize(JsonWriter writer) {
			writer.WriteStartObject();

			if (PbrMetallicRoughness != null)
			{
				writer.WritePropertyName("pbrMetallicRoughness");
				PbrMetallicRoughness.Serialize(writer);
			}

			if (CommonConstant != null)
			{
				writer.WritePropertyName("commonConstant");
				CommonConstant.Serialize(writer);
			}

			if (NormalTexture != null)
			{
				writer.WritePropertyName("normalTexture");
				NormalTexture.Serialize(writer);
			}

			if (OcclusionTexture != null)
			{
				writer.WritePropertyName("occlusionTexture");
				OcclusionTexture.Serialize(writer);
			}

			if (EmissiveTexture != null)
			{
				writer.WritePropertyName("emissiveTexture");
				EmissiveTexture.Serialize(writer);
			}

			if (EmissiveFactor != Color.black)
			{
				writer.WritePropertyName("emissiveFactor");
				writer.WriteStartArray();
				writer.WriteValue(EmissiveFactor.r);
				writer.WriteValue(EmissiveFactor.g);
				writer.WriteValue(EmissiveFactor.b);
				writer.WriteEndArray();
			}

			if (AlphaMode != AlphaMode.OPAQUE)
			{
				writer.WritePropertyName("alphaMode");
				writer.WriteValue(AlphaMode.ToString());
			}

			if (AlphaCutoff != 0.5)
			{
				writer.WritePropertyName("alphaCutoff");
				writer.WriteValue(AlphaCutoff);
			}

			if (DoubleSided) {
				writer.WritePropertyName("doubleSided");
				writer.WriteValue(true);
			}

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}

	public enum AlphaMode
	{
		OPAQUE,
		MASK,
		BLEND
	}
}
