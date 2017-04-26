using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelItemPositionMultiAnchor : PanelItemPosition {
    
    public Transform PositionTransform;
    public Transform ScaleTransform;

    protected override void SetScale()
    {
        //base.SetScale();
        mAnchorPosition = this.gameObject.transform.InverseTransformPoint(PositionTransform.position);
        mAnchorScale = ScaleTransform.localScale;
        print(mAnchorPosition);
    }
}
