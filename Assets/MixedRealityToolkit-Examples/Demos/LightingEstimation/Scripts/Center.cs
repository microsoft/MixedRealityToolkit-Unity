using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Center : MonoBehaviour {
	[SerializeField] float _dist = 1;
	[SerializeField] Transform _from = null;
	[SerializeField] bool _editorOnly = true;

	private void Awake() {
		if (_editorOnly) {
			#if !UNITY_EDITOR
			enabled = false;
			#endif
		}
	}
	void LateUpdate () {
		transform.position = (_from == null? Vector3.zero : _from.position) - transform.forward * _dist;
	}
}
