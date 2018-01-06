using System.Collections.Generic;
using Newtonsoft.Json;

namespace GLTF
{
	/// <summary>
	/// The root nodes of a scene.
	/// </summary>
	public class Scene : GLTFChildOfRootProperty
	{
		/// <summary>
		/// The indices of each root node.
		/// </summary>
		public List<NodeId> Nodes;

		public static Scene Deserialize(GLTFRoot root, JsonReader reader)
		{
			var scene = new Scene();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "nodes":
						scene.Nodes = NodeId.ReadList(root, reader);
						break;
					default:
						scene.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return scene;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			if (Nodes != null && Nodes.Count > 0)
			{
				writer.WritePropertyName("nodes");
				writer.WriteStartArray();
				foreach (var node in Nodes)
				{
					writer.WriteValue(node.Id);
				}
				writer.WriteEndArray();
			}

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
