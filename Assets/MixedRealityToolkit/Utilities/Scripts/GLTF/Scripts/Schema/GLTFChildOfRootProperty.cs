using Newtonsoft.Json;
using System.Collections.Generic;

namespace GLTF
{
	public class GLTFChildOfRootProperty : GLTFProperty
	{
		/// <summary>
		/// The user-defined name of this object.
		/// This is not necessarily unique, e.g., an accessor and a buffer could have the same name,
		/// or two accessors could even have the same name.
		/// </summary>
		public string Name;

		public new void DefaultPropertyDeserializer(GLTFRoot root, JsonReader reader)
		{
			switch (reader.Value.ToString())
			{
				case "name":
					Name = reader.ReadAsString();
					break;
				default:
					base.DefaultPropertyDeserializer(root, reader);
					break;
			}
		}

		public override void Serialize(JsonWriter writer)
		{

			if (Name != null)
			{
				writer.WritePropertyName("name");
				writer.WriteValue(Name);
			}

			base.Serialize(writer);
		}
	}
}
