using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelItem : PanelItemPosition
{
    public Vector3 ItemSize = new Vector3(140, 120, 20);
    
    private void UpdateSize()
    {
        Vector3 newScale = new Vector3(ItemSize.x / BasePixelSize, ItemSize.y / BasePixelSize, ItemSize.z / BasePixelSize);

        transform.localScale = newScale;
    }
    // Update is called once per frame
    protected override void Update()
    {
        if (Anchor != null)
        {
            UpdateSize();
        }

        base.Update();
    }
}
