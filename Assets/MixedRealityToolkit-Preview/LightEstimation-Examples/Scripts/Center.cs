// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Preview.Examples.LightEstimation
{
	public class Center : MonoBehaviour
	{
		[Tooltip("Distance to keep from the center, Unity units.")]
		[SerializeField] private float     dist       = 1;
		[Tooltip("If not empty, this will be used as the 'center'.")]
		[SerializeField] private Transform from       = null;
		[Tooltip("If false, this component will self-disable when outside of the Editor.")]
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