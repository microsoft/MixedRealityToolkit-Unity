// Author: Nick Klingensmith - @koujaku (Twitter)
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

public class FollowGyroscope : MonoBehaviour {
	[SerializeField] bool _onlyRunInEditor = true;

	private void Awake() {
		if (!Application.isEditor && _onlyRunInEditor)
			enabled = false;
	}
	private void Update () {
		transform.localRotation = EditorGyro.GetRotation();
	}
}
