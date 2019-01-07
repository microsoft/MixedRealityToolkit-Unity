// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
			if (!Application.isEditor && editorOnly)
			{
				enabled = false;
			}
		}
		void LateUpdate ()
		{
			transform.position = (from == null? Vector3.zero : from.position) - transform.forward * dist;
		}
	}
}