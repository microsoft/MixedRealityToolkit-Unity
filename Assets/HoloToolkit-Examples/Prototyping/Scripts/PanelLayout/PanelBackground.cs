using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelBackground : MonoBehaviour {

    public float BasePixelScale = 2048;
    public Vector3 ItemSize = new Vector3(594, 246, 15);

    private void UpdateSize()
    {
        Vector3 newScale = new Vector3(ItemSize.x / BasePixelScale, ItemSize.y / BasePixelScale, ItemSize.z / BasePixelScale);
        transform.localScale = newScale;
    }

    // Update is called once per frame
    void Update () {
        UpdateSize();
	}
}
