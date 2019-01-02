using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
	public class Center : MonoBehaviour
	{
		[SerializeField] float     dist       = 1;
		[SerializeField] Transform from       = null;
		[SerializeField] bool      editorOnly = true;

		private void Awake()
		{
			if (editorOnly)
			{
				#if !UNITY_EDITOR
				enabled = false;
				#endif
			}
		}
		void LateUpdate ()
		{
			transform.position = (from == null? Vector3.zero : from.position) - transform.forward * dist;
		}
	}
}