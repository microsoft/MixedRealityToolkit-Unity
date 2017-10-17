using UnityEngine;
using System.Collections;
using System;
using System.IO;
using Newtonsoft.Json;

namespace GLTF
{
	public class GLTFParser
	{

		private enum ChunkFormat : uint
		{
			JSON = 0x4e4f534a,
			BIN = 0x004e4942
		}

		public static GLTFRoot ParseBinary(byte[] gltfBinary, out byte[] glbBuffer)
		{
			string gltfContent;
			glbBuffer = null;

			// Check for binary format magic bytes
			if (BitConverter.ToUInt32(gltfBinary, 0) == 0x46546c67)
			{
				// Parse header information

				var version = BitConverter.ToUInt32(gltfBinary, 4);
				if (version != 2)
				{
					throw new GLTFHeaderInvalidException("Unsupported glTF version");
				}

				var length = BitConverter.ToUInt32(gltfBinary, 8);
				if (length != gltfBinary.Length)
				{
					throw new GLTFHeaderInvalidException("File length does not match header.");
				}

				var chunkLength = BitConverter.ToUInt32(gltfBinary, 12);
				var chunkType = BitConverter.ToUInt32(gltfBinary, 16);
				if (chunkType != (uint)ChunkFormat.JSON)
				{
					throw new GLTFHeaderInvalidException("First chunk must be of type JSON");
				}

				// Load JSON chunk
				gltfContent = System.Text.Encoding.UTF8.GetString(gltfBinary, 20, (int)chunkLength);

				// Load Binary Chunk
				if (20 + chunkLength < length)
				{
					var start = 20 + (int)chunkLength;
					chunkLength = BitConverter.ToUInt32(gltfBinary, start);
					if (start + chunkLength > length)
					{
						throw new GLTFHeaderInvalidException("File length does not match chunk header.");
					}

					chunkType = BitConverter.ToUInt32(gltfBinary, start + 4);
					if (chunkType != (uint)ChunkFormat.BIN)
					{
						throw new GLTFHeaderInvalidException("Second chunk must be of type BIN if present");
					}

					glbBuffer = new byte[chunkLength];
					System.Buffer.BlockCopy(gltfBinary, start + 8, glbBuffer, 0, (int)chunkLength);
				}
			}
			else
			{
				gltfContent = System.Text.Encoding.UTF8.GetString(gltfBinary);
			}

			return ParseString(gltfContent);
		}

		public static GLTFRoot ParseString(string gltfContent)
		{
			var stringReader = new StringReader(gltfContent);
			return GLTFRoot.Deserialize(new JsonTextReader(stringReader));
		}
	}
}

