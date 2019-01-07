// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Preview.Examples.Demos
{
	public class Center : MonoBehaviour
	{
		[SerializeField] private float     dist       = 1;
		[SerializeField] private Transform from       = null;
		[SerializeField] private bool      editorOnly = true;

		private void Awake()
		{
			if (!Application.isEditor && editorOnly)
			{
				enabled = false;
			}
		}
		private void LateUpdate ()
		{
			transform.position = (from == null? Vector3.zero : from.position) - transform.forward * dist;
		}
	}
}