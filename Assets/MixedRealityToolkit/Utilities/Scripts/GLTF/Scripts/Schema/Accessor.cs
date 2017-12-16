using System;
using System.Collections.Generic;
using GLTF.JsonExtensions;
using Newtonsoft.Json;
using UnityEngine;

namespace GLTF
{
	public class Accessor : GLTFChildOfRootProperty
	{
		/// <summary>
		/// The index of the bufferView.
		/// If this is undefined, look in the sparse object for the index and value buffer views.
		/// </summary>
		public BufferViewId BufferView;

		/// <summary>
		/// The offset relative to the start of the bufferView in bytes.
		/// This must be a multiple of the size of the component datatype.
		/// <minimum>0</minimum>
		/// </summary>
		public int ByteOffset;

		/// <summary>
		/// The datatype of components in the attribute.
		/// All valid values correspond to WebGL enums.
		/// The corresponding typed arrays are: `Int8Array`, `Uint8Array`, `Int16Array`,
		/// `Uint16Array`, `Uint32Array`, and `Float32Array`, respectively.
		/// 5125 (UNSIGNED_INT) is only allowed when the accessor contains indices
		/// i.e., the accessor is only referenced by `primitive.indices`.
		/// </summary>
		public GLTFComponentType ComponentType;

		/// <summary>
		/// Specifies whether integer data values should be normalized
		/// (`true`) to [0, 1] (for unsigned types) or [-1, 1] (for signed types),
		/// or converted directly (`false`) when they are accessed.
		/// Must be `false` when accessor is used for animation data.
		/// </summary>
		public bool Normalized;

		/// <summary>
		/// The number of attributes referenced by this accessor, not to be confused
		/// with the number of bytes or number of components.
		/// <minimum>1</minimum>
		/// </summary>
		public int Count;

		/// <summary>
		/// Specifies if the attribute is a scalar, vector, or matrix,
		/// and the number of elements in the vector or matrix.
		/// </summary>
		public GLTFAccessorAttributeType Type;

		/// <summary>
		/// Maximum value of each component in this attribute.
		/// Both min and max arrays have the same length.
		/// The length is determined by the value of the type property;
		/// it can be 1, 2, 3, 4, 9, or 16.
		///
		/// When `componentType` is `5126` (FLOAT) each array value must be stored as
		/// double-precision JSON number with numerical value which is equal to
		/// buffer-stored single-precision value to avoid extra runtime conversions.
		///
		/// `normalized` property has no effect on array values: they always correspond
		/// to the actual values stored in the buffer. When accessor is sparse, this
		/// property must contain max values of accessor data with sparse substitution
		/// applied.
		/// <minItems>1</minItems>
		/// <maxItems>16</maxItems>
		/// </summary>
		public List<double> Max;

		/// <summary>
		/// Minimum value of each component in this attribute.
		/// Both min and max arrays have the same length.  The length is determined by
		/// the value of the type property; it can be 1, 2, 3, 4, 9, or 16.
		///
		/// When `componentType` is `5126` (FLOAT) each array value must be stored as
		/// double-precision JSON number with numerical value which is equal to
		/// buffer-stored single-precision value to avoid extra runtime conversions.
		///
		/// `normalized` property has no effect on array values: they always correspond
		/// to the actual values stored in the buffer. When accessor is sparse, this
		/// property must contain min values of accessor data with sparse substitution
		/// applied.
		/// <minItems>1</minItems>
		/// <maxItems>16</maxItems>
		/// </summary>
		public List<double> Min;

		/// <summary>
		/// Sparse storage of attributes that deviate from their initialization value.
		/// </summary>
		public AccessorSparse Sparse;

		public static Accessor Deserialize(GLTFRoot root, JsonReader reader)
		{
			var accessor = new Accessor();

			while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
			{
				var curProp = reader.Value.ToString();

				switch (curProp)
				{
					case "bufferView":
						accessor.BufferView = BufferViewId.Deserialize(root, reader);
						break;
					case "byteOffset":
						accessor.ByteOffset = reader.ReadAsInt32().Value;
						break;
					case "componentType":
						accessor.ComponentType = (GLTFComponentType)reader.ReadAsInt32().Value;
						break;
					case "normalized":
						accessor.Normalized = reader.ReadAsBoolean().Value;
						break;
					case "count":
						accessor.Count = reader.ReadAsInt32().Value;
						break;
					case "type":
						accessor.Type = reader.ReadStringEnum<GLTFAccessorAttributeType>();
						break;
					case "max":
						accessor.Max = reader.ReadDoubleList();
						break;
					case "min":
						accessor.Min = reader.ReadDoubleList();
						break;
					case "sparse":
						accessor.Sparse = AccessorSparse.Deserialize(root, reader);
						break;
					default:
						accessor.DefaultPropertyDeserializer(root, reader);
						break;
				}
			}

			return accessor;
		}

		public override void Serialize(JsonWriter writer)
		{
			writer.WriteStartObject();

			if (BufferView != null)
			{
				writer.WritePropertyName("bufferView");
				writer.WriteValue(BufferView.Id);
			}

			if (ByteOffset != 0)
			{
				writer.WritePropertyName("byteOffset");
				writer.WriteValue(ByteOffset);
			}

			writer.WritePropertyName("componentType");
			writer.WriteValue(ComponentType);

			if (Normalized != false)
			{
				writer.WritePropertyName("normalized");
				writer.WriteValue(true);
			}

			writer.WritePropertyName("count");
			writer.WriteValue(Count);

			writer.WritePropertyName("type");
			writer.WriteValue(Type.ToString());

			writer.WritePropertyName("max");
			writer.WriteStartArray();
			foreach (var item in Max)
			{
				writer.WriteValue(item);
			}
			writer.WriteEndArray();

			writer.WritePropertyName("min");
			writer.WriteStartArray();
			foreach (var item in Min)
			{
				writer.WriteValue(item);
			}
			writer.WriteEndArray();

			if (Sparse != null)
			{
				writer.WritePropertyName("sparse");
				Sparse.Serialize(writer);
			}

			base.Serialize(writer);

			writer.WriteEndObject();
		}

		public int[] AsIntArray(byte[] bufferData)
		{
			var arr = new int[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			var stride = 0;

			switch (ComponentType)
			{
				case GLTFComponentType.Byte:
					stride = BufferView.Value.ByteStride + sizeof(sbyte);
					for (var idx = 0; idx < Count; idx++)
					{
						arr[idx] = (sbyte)bufferData[totalByteOffset + (idx * stride)];
					}
					break;
				case GLTFComponentType.UnsignedByte:
					stride = BufferView.Value.ByteStride + sizeof(byte);
					for (var idx = 0; idx < Count; idx++)
					{
						arr[idx] = bufferData[totalByteOffset + (idx * stride)];
					}
					break;
				case GLTFComponentType.Short:
					stride = BufferView.Value.ByteStride + sizeof(short);
					for (var idx = 0; idx < Count; idx++)
					{
						arr[idx] = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride));
					}
					break;
				case GLTFComponentType.UnsignedShort:
					if (BufferView.Value.ByteStride == 0)
					{
						var intermediateArr = new ushort[Count];
						System.Buffer.BlockCopy(bufferData, totalByteOffset, intermediateArr, 0, Count * sizeof(ushort));
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = (int)intermediateArr[idx];
						}
					}
					else
					{
						stride = BufferView.Value.ByteStride + sizeof(ushort);
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride));
						}
					}
					break;
				case GLTFComponentType.UnsignedInt:
					if (BufferView.Value.ByteStride == 0)
					{
						var intermediateArr = new uint[Count];
						System.Buffer.BlockCopy(bufferData, totalByteOffset, intermediateArr, 0, Count * sizeof(uint));
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = (int)intermediateArr[idx];
						}
					}
					else
					{
						stride = BufferView.Value.ByteStride + sizeof(uint);
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = (int)BitConverter.ToUInt32(bufferData, totalByteOffset + (idx * stride));
						}
					}
					break;
				case GLTFComponentType.Float:
					if (BufferView.Value.ByteStride == 0)
					{
						var intermediateArr = new int[Count];
						System.Buffer.BlockCopy(bufferData, totalByteOffset, intermediateArr, 0, Count * sizeof(float));
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = intermediateArr[idx];
						}
					}
					else
					{
						stride = BufferView.Value.ByteStride + sizeof(float);
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = (int)BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride));
						}
					}
					break;
				default:
					throw new Exception("Unsupported component type.");
			}

			return arr;
		}

		public Vector2[] AsVector2Array(byte[] bufferData)
		{
			var arr = new Vector2[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			var stride = 0;
			const int numComponents = 2;

			switch (ComponentType)
			{
				case GLTFComponentType.Byte:
					stride = numComponents * sizeof(sbyte) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = (sbyte)bufferData[totalByteOffset + (idx * stride)];
						var y = (sbyte)bufferData[totalByteOffset + (idx * stride) + sizeof(sbyte)];
						arr[idx] = new Vector2(x, -y);
					}
					break;
				case GLTFComponentType.UnsignedByte:
					stride = numComponents * sizeof(byte) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = bufferData[totalByteOffset + (idx * stride)];
						var y = bufferData[totalByteOffset + (idx * stride) + sizeof(byte)];
						arr[idx] = new Vector2(x, -y);
					}
					break;
				case GLTFComponentType.Short:
					stride = numComponents * sizeof(short) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride));
						float y = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + sizeof(short));
						arr[idx] = new Vector2(x, -y);
					}
					break;
				case GLTFComponentType.UnsignedShort:
					stride = numComponents * sizeof(ushort) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride));
						var y = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + sizeof(ushort));
						arr[idx] = new Vector2(x, -y);
					}
					break;
				case GLTFComponentType.UnsignedInt:
					stride = numComponents * sizeof(uint) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = BitConverter.ToUInt32(bufferData, totalByteOffset + (idx * stride));
						var y = BitConverter.ToUInt32(bufferData, totalByteOffset + (idx * stride) + sizeof(uint));
						arr[idx] = new Vector2(x, -y);
					}
					break;
				case GLTFComponentType.Float:
					if (BufferView.Value.ByteStride == 0)
					{
						var totalComponents = Count * 2;
						var intermediateArr = new float[totalComponents];
						System.Buffer.BlockCopy(bufferData, totalByteOffset, intermediateArr, 0, totalComponents * sizeof(float));
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = new Vector2(
								intermediateArr[idx * 2],
								-intermediateArr[idx * 2 + 1]
							);
						}
					}
					else
					{
						stride = numComponents * sizeof(float) + BufferView.Value.ByteStride;
						for (var idx = 0; idx < Count; idx++)
						{
							var x = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride));
							var y = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + sizeof(float));
							arr[idx] = new Vector2(x, -y);
						}
					}
					break;
				default:
					throw new Exception("Unsupported component type.");
			}

			return arr;
		}

		public Vector3[] AsVector3Array(byte[] bufferData)
		{
			var arr = new Vector3[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			int stride;
			const int numComponents = 3;

			switch (ComponentType)
			{
				case GLTFComponentType.Byte:
					stride = numComponents * sizeof(sbyte) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						float x = (sbyte)bufferData[totalByteOffset + (idx * stride)];
						float y = (sbyte)bufferData[totalByteOffset + (idx * stride) + sizeof(sbyte)];
						float z = (sbyte)bufferData[totalByteOffset + (idx * stride) + (2 * sizeof(sbyte))];
						arr[idx] = new Vector3(x, y, -z);
					}
					break;
				case GLTFComponentType.UnsignedByte:
					stride = numComponents * sizeof(byte) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						float x = bufferData[totalByteOffset + (idx * stride)];
						float y = bufferData[totalByteOffset + (idx * stride) + sizeof(byte)];
						float z = bufferData[totalByteOffset + (idx * stride) + (2 * sizeof(byte))];
						arr[idx] = new Vector3(x, y, -z);
					}
					break;
				case GLTFComponentType.Short:
					stride = numComponents * sizeof(short) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						float x = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride));
						float y = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + sizeof(short));
						float z = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(short)));
						arr[idx] = new Vector3(x, y, -z);
					}
					break;
				case GLTFComponentType.UnsignedShort:
					stride = numComponents * sizeof(ushort) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						float x = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride));
						float y = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + sizeof(ushort));
						float z = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(ushort)));
						arr[idx] = new Vector3(x, y, -z);
					}
					break;
				case GLTFComponentType.UnsignedInt:
					stride = numComponents * sizeof(uint) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						float x = BitConverter.ToUInt32(bufferData, totalByteOffset + (idx * stride));
						float y = BitConverter.ToUInt32(bufferData, totalByteOffset + (idx * stride) + sizeof(uint));
						float z = BitConverter.ToUInt32(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(uint)));
						arr[idx] = new Vector3(x, y, -z);
					}
					break;
				case GLTFComponentType.Float:
					if (BufferView.Value.ByteStride == 0)
					{
						var totalComponents = Count * 3;
						var intermediateArr = new float[totalComponents];
						System.Buffer.BlockCopy(bufferData, totalByteOffset, intermediateArr, 0, totalComponents * sizeof(float));
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = new Vector3(
							   intermediateArr[idx * 3],
							   intermediateArr[idx * 3 + 1],
							   -intermediateArr[idx * 3 + 2]
							);
						}
					}
					else
					{
						stride = numComponents * sizeof(float) + BufferView.Value.ByteStride;
						for (var idx = 0; idx < Count; idx++)
						{
							var x = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride));
							var y = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + sizeof(float));
							var z = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(float)));
							arr[idx] = new Vector3(x, y, -z);
						}
					}
					break;
				default:
					throw new Exception("Unsupported component type.");
			}

			return arr;
		}

		public Vector4[] AsVector4Array(byte[] bufferData)
		{
			var arr = new Vector4[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			int stride;
			const int numComponents = 4;

			switch (ComponentType)
			{
				case GLTFComponentType.Byte:
					stride = numComponents * sizeof(sbyte) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = (sbyte)bufferData[totalByteOffset + (idx * stride)];
						var y = (sbyte)bufferData[totalByteOffset + (idx * stride) + sizeof(sbyte)];
						var z = (sbyte)bufferData[totalByteOffset + (idx * stride) + (2 * sizeof(sbyte))];
						var w = (sbyte)bufferData[totalByteOffset + (idx * stride) + (3 * sizeof(sbyte))];
						arr[idx] = new Vector4(x, y, z, -w);
					}
					break;
				case GLTFComponentType.UnsignedByte:
					stride = numComponents * sizeof(byte) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = bufferData[totalByteOffset + (idx * stride)];
						var y = bufferData[totalByteOffset + (idx * stride) + sizeof(byte)];
						var z = bufferData[totalByteOffset + (idx * stride) + (2 * sizeof(byte))];
						var w = bufferData[totalByteOffset + (idx * stride) + (3 * sizeof(byte))];
						arr[idx] = new Vector4(x, y, z, -w);
					}
					break;
				case GLTFComponentType.Short:
					stride = numComponents * sizeof(short) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride));
						var y = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + sizeof(short));
						var z = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(short)));
						var w = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + (3 * sizeof(short)));
						arr[idx] = new Vector4(x, y, z, -w);
					}
					break;
				case GLTFComponentType.UnsignedShort:
					stride = numComponents * sizeof(ushort) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var x = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride));
						var y = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + sizeof(ushort));
						var z = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(ushort)));
						var w = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + (3 * sizeof(ushort)));
						arr[idx] = new Vector4(x, y, z, -w);
					}
					break;
				case GLTFComponentType.Float:
					if (BufferView.Value.ByteStride == 0)
					{
						var totalComponents = Count * 4;
						var intermediateArr = new float[totalComponents];
						System.Buffer.BlockCopy(bufferData, totalByteOffset, intermediateArr, 0, totalComponents * sizeof(float));
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = new Vector4(
								intermediateArr[idx * 4],
								intermediateArr[idx * 4 + 1],
								intermediateArr[idx * 4 + 2],
								-intermediateArr[idx * 4 + 3]
							);
						}
					}
					else
					{
						stride = numComponents * sizeof(float) + BufferView.Value.ByteStride;
						for (var idx = 0; idx < Count; idx++)
						{
							var x = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride));
							var y = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + sizeof(float));
							var z = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(float)));
							var w = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + (3 * sizeof(float)));
							arr[idx] = new Vector4(x, y, z, -w);
						}
					}
					break;
				default:
					throw new Exception("Unsupported component type.");
			}

			return arr;
		}

		public Color[] AsColorArray(byte[] bufferData)
		{
			var arr = new Color[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			int stride;
			const int numComponents = 4;

			switch (ComponentType)
			{
				case GLTFComponentType.Byte:
					stride = numComponents * sizeof(sbyte) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var r = (sbyte)bufferData[totalByteOffset + (idx * stride)];
						var g = (sbyte)bufferData[totalByteOffset + (idx * stride) + sizeof(sbyte)];
						var b = (sbyte)bufferData[totalByteOffset + (idx * stride) + (2 * sizeof(sbyte))];
						var a = (sbyte)bufferData[totalByteOffset + (idx * stride) + (3 * sizeof(sbyte))];
						arr[idx] = new Color(r, g, b, a);
					}
					break;
				case GLTFComponentType.UnsignedByte:
					stride = numComponents * sizeof(byte) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var r = bufferData[totalByteOffset + (idx * stride)];
						var g = bufferData[totalByteOffset + (idx * stride) + sizeof(byte)];
						var b = bufferData[totalByteOffset + (idx * stride) + (2 * sizeof(byte))];
						var a = bufferData[totalByteOffset + (idx * stride) + (3 * sizeof(byte))];
						arr[idx] = new Color(r, g, b, a);
					}
					break;
				case GLTFComponentType.Short:
					stride = numComponents * sizeof(short) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var r = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride));
						var g = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + sizeof(short));
						var b = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(short)));
						var a = BitConverter.ToInt16(bufferData, totalByteOffset + (idx * stride) + (3 * sizeof(short)));
						arr[idx] = new Color(r, g, b, a);
					}
					break;
				case GLTFComponentType.UnsignedShort:
					stride = numComponents * sizeof(ushort) + BufferView.Value.ByteStride;
					for (var idx = 0; idx < Count; idx++)
					{
						var r = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride));
						var g = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + sizeof(ushort));
						var b = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(ushort)));
						var a = BitConverter.ToUInt16(bufferData, totalByteOffset + (idx * stride) + (3 * sizeof(ushort)));
						arr[idx] = new Color(r, g, b, a);
					}
					break;
				case GLTFComponentType.Float:
					if (BufferView.Value.ByteStride == 0)
					{
						var totalComponents = Count * 4;
						var intermediateArr = new float[totalComponents];
						System.Buffer.BlockCopy(bufferData, totalByteOffset, intermediateArr, 0, totalComponents * sizeof(float));
						for (var idx = 0; idx < Count; idx++)
						{
							arr[idx] = new Color(
								intermediateArr[idx * 4],
								intermediateArr[idx * 4 + 1],
								intermediateArr[idx * 4 + 2],
								intermediateArr[idx * 4 + 3]
							);
						}
					}
					else
					{
						stride = numComponents * sizeof(float) + BufferView.Value.ByteStride;
						for (var idx = 0; idx < Count; idx++)
						{
							var r = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride));
							var g = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + sizeof(float));
							var b = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + (2 * sizeof(float)));
							var a = BitConverter.ToSingle(bufferData, totalByteOffset + (idx * stride) + (3 * sizeof(float)));
							arr[idx] = new Color(r, g, b, a);
						}
					}
					break;
				default:
					throw new Exception("Unsupported component type.");
			}

			return arr;
		}
	}

	public enum GLTFComponentType
	{
		Byte = 5120,
		UnsignedByte = 5121,
		Short = 5122,
		UnsignedShort = 5123,
		UnsignedInt = 5125,
		Float = 5126
	}

	public enum GLTFAccessorAttributeType
	{
		SCALAR,
		VEC2,
		VEC3,
		VEC4,
		MAT2,
		MAT3,
		MAT4
	}
}
