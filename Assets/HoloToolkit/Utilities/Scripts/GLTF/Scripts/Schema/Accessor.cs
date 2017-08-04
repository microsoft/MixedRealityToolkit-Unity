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

		public NumericArray Contents;

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

		private static unsafe int GetByteElement(byte[] buffer, int byteOffset)
		{
			fixed (byte* offsetBuffer = &buffer[byteOffset])
			{
				return *((sbyte*)offsetBuffer);
			}
		}

		private static unsafe int GetUByteElement(byte[] buffer, int byteOffset)
		{
			fixed (byte* offsetBuffer = &buffer[byteOffset])
			{
				return *((byte*)offsetBuffer);
			}
		}

		private static unsafe int GetShortElement(byte[] buffer, int byteOffset)
		{
			fixed (byte* offsetBuffer = &buffer[byteOffset])
			{
				return *((short*)offsetBuffer);
			}
		}

		private static unsafe int GetUShortElement(byte[] buffer, int byteOffset)
		{
			fixed (byte* offsetBuffer = &buffer[byteOffset])
			{
				return *((ushort*)offsetBuffer);
			}
		}

		private static unsafe int GetUIntElement(byte[] buffer, int byteOffset)
		{
			fixed (byte* offsetBuffer = &buffer[byteOffset])
			{
				return (int) *((uint*)offsetBuffer);
			}
		}

		private static unsafe float GetFloatElement(byte[] buffer, int byteOffset)
		{
			fixed (byte* offsetBuffer = &buffer[byteOffset])
			{
				return *((float*)offsetBuffer);
			}
		}

		private static void GetTypeDetails(GLTFComponentType type, out int componentSize, out float maxValue,
			out Func<byte[], int, int> discreteFunc, out Func<byte[], int, float> continuousFunc)
		{
			componentSize = 1;
			maxValue = byte.MaxValue;
			discreteFunc = GetUByteElement;
			continuousFunc = GetFloatElement;

			switch (type)
			{
				case GLTFComponentType.Byte:
					discreteFunc = GetByteElement;
					componentSize = sizeof(sbyte);
					maxValue = sbyte.MaxValue;
					break;
				case GLTFComponentType.UnsignedByte:
					discreteFunc = GetUByteElement;
					componentSize = sizeof(byte);
					maxValue = byte.MaxValue;
					break;
				case GLTFComponentType.Short:
					discreteFunc = GetShortElement;
					componentSize = sizeof(short);
					maxValue = short.MaxValue;
					break;
				case GLTFComponentType.UnsignedShort:
					discreteFunc = GetUShortElement;
					componentSize = sizeof(ushort);
					maxValue = ushort.MaxValue;
					break;
				case GLTFComponentType.UnsignedInt:
					discreteFunc = GetUIntElement;
					componentSize = sizeof(uint);
					maxValue = uint.MaxValue;
					break;
				case GLTFComponentType.Float:
					continuousFunc = GetFloatElement;
					componentSize = sizeof(float);
					maxValue = float.MaxValue;
					break;
				default:
					throw new Exception("Unsupported component type.");
			}
		}

		public int[] AsIntArray()
		{
			if (Contents.AsInts != null) return Contents.AsInts;

			if (Type != GLTFAccessorAttributeType.SCALAR) return null;

			var arr = new int[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			var bufferData = BufferView.Value.Buffer.Value.Contents;

			int componentSize;
			float maxValue;
			Func<byte[], int, int> getDiscreteElement;
			Func<byte[], int, float> getContinuousElement;
			GetTypeDetails(ComponentType, out componentSize, out maxValue, out getDiscreteElement, out getContinuousElement);

			var stride = BufferView.Value.ByteStride > 0 ? BufferView.Value.ByteStride : componentSize;

			for (var idx = 0; idx < Count; idx++)
			{
				if(ComponentType == GLTFComponentType.Float)
					arr[idx] = Mathf.FloorToInt(getContinuousElement(bufferData, totalByteOffset + idx * stride));
				else
					arr[idx] = getDiscreteElement(bufferData, totalByteOffset + idx*stride);
			}

			Contents.AsInts = arr;
			return arr;
		}

		public Vector2[] AsVector2Array(bool normalizeIntValues = true)
		{
			if (Contents.AsVec2s != null) return Contents.AsVec2s;

			if (Type != GLTFAccessorAttributeType.VEC2) return null;

			var arr = new Vector2[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			var bufferData = BufferView.Value.Buffer.Value.Contents;

			int componentSize;
			float maxValue;
			Func<byte[], int, int> getDiscreteElement;
			Func<byte[], int, float> getContinuousElement;
			GetTypeDetails(ComponentType, out componentSize, out maxValue, out getDiscreteElement, out getContinuousElement);

			var stride = BufferView.Value.ByteStride > 0 ? BufferView.Value.ByteStride : componentSize * 2;
			if (normalizeIntValues) maxValue = 1;

			for (var idx = 0; idx < Count; idx++)
			{
				if (ComponentType == GLTFComponentType.Float)
				{
					arr[idx].x = getContinuousElement(bufferData, totalByteOffset + idx*stride + componentSize * 0);
					arr[idx].y = getContinuousElement(bufferData, totalByteOffset + idx*stride + componentSize * 1);
				}
				else
				{
					arr[idx].x = getDiscreteElement(bufferData, totalByteOffset + idx*stride + componentSize * 0) / maxValue;
					arr[idx].y = getDiscreteElement(bufferData, totalByteOffset + idx*stride + componentSize * 1) / maxValue;
				}
			}

			Contents.AsVec2s = arr;
			return arr;
		}

		public Vector3[] AsVector3Array(bool normalizeIntValues = true)
		{
			if (Contents.AsVec3s != null) return Contents.AsVec3s;

			if (Type != GLTFAccessorAttributeType.VEC3) return null;

			var arr = new Vector3[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			var bufferData = BufferView.Value.Buffer.Value.Contents;

			int componentSize;
			float maxValue;
			Func<byte[], int, int> getDiscreteElement;
			Func<byte[], int, float> getContinuousElement;
			GetTypeDetails(ComponentType, out componentSize, out maxValue, out getDiscreteElement, out getContinuousElement);

			var stride = BufferView.Value.ByteStride > 0 ? BufferView.Value.ByteStride : componentSize * 3;
			if (normalizeIntValues) maxValue = 1;

			for (var idx = 0; idx < Count; idx++)
			{
				if (ComponentType == GLTFComponentType.Float)
				{
					arr[idx].x = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 0);
					arr[idx].y = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 1);
					arr[idx].z = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 2);
				}
				else
				{
					arr[idx].x = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 0) / maxValue;
					arr[idx].y = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 1) / maxValue;
					arr[idx].z = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 2) / maxValue;
				}
			}

			Contents.AsVec3s = arr;
			return arr;
		}

		public Vector4[] AsVector4Array(bool normalizeIntValues = true)
		{
			if (Contents.AsVec4s != null) return Contents.AsVec4s;

			if (Type != GLTFAccessorAttributeType.VEC4) return null;

			var arr = new Vector4[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			var bufferData = BufferView.Value.Buffer.Value.Contents;

			int componentSize;
			float maxValue;
			Func<byte[], int, int> getDiscreteElement;
			Func<byte[], int, float> getContinuousElement;
			GetTypeDetails(ComponentType, out componentSize, out maxValue, out getDiscreteElement, out getContinuousElement);

			var stride = BufferView.Value.ByteStride > 0 ? BufferView.Value.ByteStride : componentSize * 4;
			if (normalizeIntValues) maxValue = 1;

			for (var idx = 0; idx < Count; idx++)
			{
				if (ComponentType == GLTFComponentType.Float)
				{
					arr[idx].x = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 0);
					arr[idx].y = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 1);
					arr[idx].z = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 2);
					arr[idx].w = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 3);
				}
				else
				{
					arr[idx].x = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 0) / maxValue;
					arr[idx].y = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 1) / maxValue;
					arr[idx].z = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 2) / maxValue;
					arr[idx].w = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 3) / maxValue;
				}
			}

			Contents.AsVec4s = arr;
			return arr;
		}

		public Color[] AsColorArray()
		{
			if (Contents.AsColors != null) return Contents.AsColors;

			if (Type != GLTFAccessorAttributeType.VEC3 && Type != GLTFAccessorAttributeType.VEC4)
				return null;

			var arr = new Color[Count];
			var totalByteOffset = BufferView.Value.ByteOffset + ByteOffset;
			var bufferData = BufferView.Value.Buffer.Value.Contents;

			int componentSize;
			float maxValue;
			Func<byte[], int, int> getDiscreteElement;
			Func<byte[], int, float> getContinuousElement;
			GetTypeDetails(ComponentType, out componentSize, out maxValue, out getDiscreteElement, out getContinuousElement);

			var stride = BufferView.Value.ByteStride > 0 ? BufferView.Value.ByteStride : componentSize * (Type == GLTFAccessorAttributeType.VEC3 ? 3 : 4);

			for (var idx = 0; idx < Count; idx++)
			{
				if (ComponentType == GLTFComponentType.Float)
				{
					arr[idx].r = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 0);
					arr[idx].g = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 1);
					arr[idx].b = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 2);
					if(Type == GLTFAccessorAttributeType.VEC4)
						arr[idx].a = getContinuousElement(bufferData, totalByteOffset + idx * stride + componentSize * 3);
					else
						arr[idx].a = 1;
				}
				else
				{
					arr[idx].r = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 0) / maxValue;
					arr[idx].g = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 1) / maxValue;
					arr[idx].b = getDiscreteElement(bufferData, totalByteOffset + idx * stride + componentSize * 2) / maxValue;
					if (Type == GLTFAccessorAttributeType.VEC4)
						arr[idx].a = getDiscreteElement(bufferData, totalByteOffset + idx*stride + componentSize * 3) / maxValue;
					else
						arr[idx].a = 1;
				}
			}

			Contents.AsColors = arr;
			return arr;
		}

		public Vector2[] AsTexcoordArray()
		{
			if (Contents.AsTexcoords != null) return Contents.AsTexcoords;

			var arr = AsVector2Array();
			for (var i=0; i<arr.Length; i++)
			{
				arr[i].y *= -1;
			}

			Contents.AsTexcoords = arr;
			Contents.AsVec2s = null;

			return arr;
		}

		public Vector3[] AsVertexArray()
		{
			if (Contents.AsVertices != null) return Contents.AsVertices;

			var arr = AsVector3Array();
			for (var i = 0; i < arr.Length; i++)
			{
				arr[i].z *= -1;
			}

			Contents.AsVertices = arr;
			Contents.AsVec3s = null;

			return arr;
		}

		public Vector3[] AsNormalArray()
		{
			if (Contents.AsNormals != null) return Contents.AsNormals;

			var arr = AsVector3Array();
			for (var i = 0; i < arr.Length; i++)
			{
				arr[i].z *= -1;
			}

			Contents.AsNormals = arr;
			Contents.AsVec3s = null;

			return arr;
		}

		public Vector4[] AsTangentArray()
		{
			if (Contents.AsTangents != null) return Contents.AsTangents;

			var arr = AsVector4Array();
			for (var i = 0; i < arr.Length; i++)
			{
				arr[i].w *= -1;
			}

			Contents.AsTangents = arr;
			Contents.AsVec4s = null;

			return arr;
		}

		public int[] AsTriangles()
		{
			if (Contents.AsTriangles != null) return Contents.AsTriangles;

			var arr = AsIntArray();
			for (var i = 0; i < arr.Length; i+=3)
			{
				var temp = arr[i];
				arr[i] = arr[i + 2];
				arr[i + 2] = temp;
			}

			Contents.AsTriangles = arr;
			Contents.AsInts = null;

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

	public struct NumericArray
	{
		public int[] AsInts;
		public Vector2[] AsVec2s;
		public Vector3[] AsVec3s;
		public Vector4[] AsVec4s;
		public Color[] AsColors;
		public Vector2[] AsTexcoords;
		public Vector3[] AsVertices;
		public Vector3[] AsNormals;
		public Vector4[] AsTangents;
		public int[] AsTriangles;
	}
}
