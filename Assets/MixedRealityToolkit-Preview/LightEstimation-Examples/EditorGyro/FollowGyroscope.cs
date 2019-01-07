// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Preview.LightEstimation
{
	public class FollowGyroscope : MonoBehaviour
	{
		[SerializeField] bool onlyRunInEditor = true;

		private void Awake()
		{
			if (!Application.isEditor && onlyRunInEditor)
				enabled = false;
		}
		private void Update ()
		{
			transform.localRotation = EditorGyro.GetRotation();
		}
	}
}