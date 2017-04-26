using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelMultiSizeAlign : MonoBehaviour {

    public enum AlignmentTypes { Horizontal, Vertical, Depth}
    public float BasePixelSize = 2048;
    public GameObject[] GridItems;
    public float Buffer;
    public AlignmentTypes Alignment;

    public Transform Anchor;
    public Vector3 AnchorOffset;

    private float mCurrentSpan;

    protected virtual void Awake()
    {
        if (Anchor == null)
        {
            Anchor = this.transform;
        }
    }

    protected virtual void BuildGrid()
    {
        mCurrentSpan = 0;

        for (int i = 0; i < GridItems.Length; ++i)
        {
            // create new prefab
            GameObject tile = GridItems[i];
            tile.transform.localPosition = GetTilePosition(i, tile.transform);
            tile.transform.localRotation = Quaternion.identity;
            
        }
    }

    protected virtual Vector3 GetTilePosition(int index, Transform transform)
    {
        Vector3 directionVector = Vector3.right;
        Vector3 startPosition = Anchor.localPosition;


        float magnitude = mCurrentSpan + transform.localScale.x * 0.5f + Buffer / BasePixelSize;

        switch (Alignment)
        {
            case AlignmentTypes.Horizontal:
                directionVector = Vector3.right;
                startPosition.x = Anchor.localPosition.x - Anchor.localScale.x * 0.5f;
                mCurrentSpan += transform.localScale.x + Buffer / BasePixelSize;
                break;
            case AlignmentTypes.Vertical:
                directionVector = Vector3.up;
                startPosition.y = Anchor.localPosition.y - Anchor.localScale.y * 0.5f;
                magnitude = mCurrentSpan + transform.localScale.y * 0.5f + Buffer / BasePixelSize;
                mCurrentSpan += transform.localScale.x + Buffer / BasePixelSize;
                break;
            case AlignmentTypes.Depth:
                directionVector = Vector3.back;
                startPosition.z = Anchor.localPosition.z - Anchor.localScale.z * 0.5f;
                magnitude = mCurrentSpan + transform.localScale.z * 0.5f + Buffer / BasePixelSize;
                mCurrentSpan += transform.localScale.x + Buffer / BasePixelSize;
                break;
            default:
                break;
        }

        return startPosition + AnchorOffset/BasePixelSize + directionVector * magnitude;
    }

    // Update is called once per frame
    protected virtual void Update () {
        BuildGrid();

    }
}
