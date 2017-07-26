using System.Collections.Generic;
using Newtonsoft.Json;
using GLTF.JsonExtensions;

namespace GLTF
{
	/// <summary>
	/// A set of primitives to be rendered. A node can contain one or more meshes.
	/// A node's transform places the mesh in the scene.
	/// </summary>
	public class Mesh : GLTFChildOfRootProperty
	{
		/// <summary>
		/// An array of primitives, each defining geometry to be rendered with
		/// a material.
		/// <minItems>1</minItems>
		/// </summary>
		public List<MeshPrimitive> Primitives;

		/// <summary>
		/// Array of weights to be applied to the Morph Targets.
		/// <minItems>0</minItems>
		/// </summary>
		public List<double> Weights;

		public static Mesh Deserialize(GLTFRoot root, JsonReader reader)
		{
			var mesh = new Mesh();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "primitives":
						mesh.Primitives = reader.ReadList(() => MeshPrimitive.Deserialize(root, reader));
						break;
					case "weights":
						mesh.Weights = reader.ReadDoubleList();
						break;
					default:
						mesh.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return mesh;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			if (Primitives != null && Primitives.Count > 0)
			{
				writer.WritePropertyName("primitives");
				writer.WriteStartArray();
				foreach (var primitive in Primitives)
				{
					primitive.Serialize(writer);
				}
				writer.WriteEndArray();
			}

			if (Weights != null && Weights.Count > 0)
			{
				writer.WritePropertyName("weights");
				writer.WriteStartArray();
				foreach (var weight in Weights)
				{
					writer.WriteValue(weight);
				}
				writer.WriteEndArray();
			}

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
