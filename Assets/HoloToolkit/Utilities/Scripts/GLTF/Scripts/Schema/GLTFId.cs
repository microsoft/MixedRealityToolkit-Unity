using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GLTF
{

	/// <summary>
	/// Abstract class that stores a reference to the root GLTF object and an id
	/// of an object of type T inside it.
	/// </summary>
	/// <typeparam name="T">The value type returned by the GLTFId reference.</typeparam>
	public abstract class GLTFId<T>
	{
		public int Id;
		public GLTFRoot Root;
		public abstract T Value { get; }

		public void Serialize(JsonWriter writer)
		{
			writer.WriteValue(Id);
		}
	}

	public class AccessorId : GLTFId<Accessor>
	{
		public override Accessor Value
		{
			get { return Root.Accessors[Id]; }
		}

		public static AccessorId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new AccessorId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class BufferId : GLTFId<Buffer>
	{
		public override Buffer Value
		{
			get { return Root.Buffers[Id]; }
		}

		public static BufferId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new BufferId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class BufferViewId : GLTFId<BufferView>
	{
		public override BufferView Value
		{
			get { return Root.BufferViews[Id]; }
		}

		public static BufferViewId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new BufferViewId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class CameraId : GLTFId<GLTFCamera>
	{
		public override GLTFCamera Value
		{
			get { return Root.Cameras[Id]; }
		}

		public static CameraId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new CameraId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class ImageId : GLTFId<Image>
	{
		public override Image Value
		{
			get { return Root.Images[Id]; }
		}

		public static ImageId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new ImageId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class MaterialId : GLTFId<Material>
	{
		public override Material Value
		{
			get { return Root.Materials[Id]; }
		}

		public static MaterialId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new MaterialId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class MeshId : GLTFId<Mesh>
	{
		public override Mesh Value
		{
			get { return Root.Meshes[Id]; }
		}

		public static MeshId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new MeshId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class NodeId : GLTFId<Node>
	{
		public override Node Value
		{
			get { return Root.Nodes[Id]; }
		}

		public static NodeId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new NodeId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}

		public static List<NodeId> ReadList(GLTFRoot root, JsonReader reader)
		{
			if (reader.Read() && reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception("Invalid array.");
			}

			var list = new List<NodeId>();

			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				var node = new NodeId
				{
					Id = int.Parse(reader.Value.ToString()),
					Root = root
				};

				list.Add(node);
			}

			return list;
		}
	}

	public class SamplerId : GLTFId<Sampler>
	{
		public override Sampler Value
		{
			get { return Root.Samplers[Id]; }
		}

		public static SamplerId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new SamplerId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class SceneId : GLTFId<Scene>
	{
		public override Scene Value
		{
			get { return Root.Scenes[Id]; }
		}

		public static SceneId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new SceneId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class SkinId : GLTFId<Skin>
	{
		public override Skin Value
		{
			get { return Root.Skins[Id]; }
		}

		public static SkinId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new SkinId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}

	public class TextureId : GLTFId<Texture>
	{
		public override Texture Value
		{
			get { return Root.Textures[Id]; }
		}

		public static TextureId Deserialize(GLTFRoot root, JsonReader reader)
		{
			return new TextureId
			{
				Id = reader.ReadAsInt32().Value,
				Root = root
			};
		}
	}
}
