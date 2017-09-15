using Newtonsoft.Json;

namespace GLTF
{
	public class AccessorSparseValues : GLTFProperty
	{
		/// <summary>
		/// The index of the bufferView with sparse values.
		/// Referenced bufferView can't have ARRAY_BUFFER or ELEMENT_ARRAY_BUFFER target.
		/// </summary>
		public BufferViewId BufferView;

		/// <summary>
		/// The offset relative to the start of the bufferView in bytes. Must be aligned.
		/// <minimum>0</minimum>
		/// </summary>
		public int ByteOffset = 0;

		public static AccessorSparseValues Deserialize(GLTFRoot root, JsonReader reader)
		{
			var values = new AccessorSparseValues();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "bufferView":
						values.BufferView = BufferViewId.Deserialize(root, reader);
						break;
					case "byteOffset":
						values.ByteOffset = reader.ReadAsInt32().Value;
						break;
					default:
						values.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return values;
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

			base.Serialize(writer);

			writer.WriteEndObject();
		}
	}
}
