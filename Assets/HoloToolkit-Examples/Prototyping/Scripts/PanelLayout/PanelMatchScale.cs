using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelMatchScale : MonoBehaviour {

    public Transform SourceScale;
    public Vector3 ScaleOffsetScale;
	// Use this for initialization

	void Start () {
		
	}

    private void UpdateScale()
    {
        transform.localScale = Vector3.Scale(SourceScale.localScale, ScaleOffsetScale);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateScale();

    }
}
