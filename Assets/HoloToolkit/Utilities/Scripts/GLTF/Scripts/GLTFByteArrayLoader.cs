using System;
using System.Collections;
using UnityEngine;

namespace GLTF
{
	public class GLTFByteArrayLoader : GLTFLoader
	{
		protected byte[] _gltfData;

		public GLTFByteArrayLoader(byte[] gltfData, Transform parent = null) : base(null, parent)
		{
			_gltfData = gltfData;
		}

		public override IEnumerator Load(int sceneIndex = -1)
		{
			if (Multithreaded)
			{
				yield return asyncAction.RunOnWorkerThread(() => ParseGLTF(_gltfData));
			}
			else
			{
				ParseGLTF(_gltfData);
			}

			yield return base.Load();
		}
	}
}
