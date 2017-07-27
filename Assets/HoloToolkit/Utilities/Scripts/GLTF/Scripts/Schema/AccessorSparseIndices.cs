using Newtonsoft.Json;

namespace GLTF
{
	public class AccessorSparseIndices : GLTFProperty
	{
		/// <summary>
		/// The index of the bufferView with sparse indices.
		/// Referenced bufferView can't have ARRAY_BUFFER or ELEMENT_ARRAY_BUFFER target.
		/// </summary>
		public BufferViewId BufferView;

		/// <summary>
		/// The offset relative to the start of the bufferView in bytes. Must be aligned.
		/// <minimum>0</minimum>
		/// </summary>
		public int ByteOffset;

		/// <summary>
		/// The indices data type. Valid values correspond to WebGL enums:
		/// `5121` (UNSIGNED_BYTE)
		/// `5123` (UNSIGNED_SHORT)
		/// `5125` (UNSIGNED_INT)
		/// </summary>
		public GLTFComponentType ComponentType;

		public static AccessorSparseIndices Deserialize(GLTFRoot root, JsonReader reader)
		{
			var indices = new AccessorSparseIndices();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "bufferView":
						indices.BufferView = BufferViewId.Deserialize(root, reader);
						break;
					case "byteOffset":
						indices.ByteOffset = reader.ReadAsInt32().Value;
						break;
					case "componentType":
						indices.ComponentType = (GLTFComponentType) reader.ReadAsInt32().Value;
						break;
					default:
						indices.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return indices;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName("bufferView");
			writer.WriteValue(BufferView.Id);

			if (ByteOffset != 0)
			{
				writer.WritePropertyName("byteOffset");
				writer.WriteValue(ByteOffset);
			}

			writer.WritePropertyName("componentType");
			writer.WriteValue((int)ComponentType);

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
