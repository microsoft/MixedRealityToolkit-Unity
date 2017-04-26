using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PanelGrid : MonoBehaviour {

    public enum AlignmentTypes { None, TopLeft, Top, TopRight, CenterLeft, Center, CenterRight, BottomLeft, Bottom, BottomRight}
    public float BasePixelSize = 2048;
    public GameObject[] GridItems;
    public int Columns;
    public Vector3 ItemSize;
    public Vector2 Buffer;
    public AlignmentTypes Alignment;

    public Transform Anchor;
    public Vector3 AnchorOffset;

    protected Vector3 mAnchorScale;
    protected Vector3 mAnchorPosition;

    protected virtual void BuildGrid()
    {
        SetScale();

        int row = 0, column = 0;
        // clear current grid

        for (int i = 0; i < GridItems.Length; ++i)
        {
            // create new prefab
            GameObject tile = GridItems[i];
            
            tile.transform.localPosition = GetTilePosition(column, row);
            tile.transform.localRotation = Quaternion.identity;

            ++column;
            if (column >= Columns)
            {
                column = 0;
                ++row;
            }
        }
    }

    protected virtual void SetScale()
    {
        mAnchorScale = Anchor.localScale;
        mAnchorPosition = Anchor.localPosition;
    }

    protected virtual Vector3 GetTilePosition(int column, int row)
    {
        Vector3 horizontalVector = Vector3.right;
        Vector3 verticalVector = Vector3.down;
        Vector3 startPosition = mAnchorPosition;

        float xMagnitude = (AnchorOffset.x + ItemSize.x * column + Buffer.x * column) / BasePixelSize;
        float yMagnitude = (AnchorOffset.y + ItemSize.y * row + Buffer.y * row) / BasePixelSize;
        float zMagnitude = AnchorOffset.z / BasePixelSize;
        
        Vector3 newPosition = startPosition + horizontalVector * xMagnitude + verticalVector * yMagnitude + Vector3.forward * zMagnitude;
        if (Alignment == AlignmentTypes.None)
        {
            return newPosition;
        }

        switch (Alignment)
        {
            case AlignmentTypes.TopLeft:
                horizontalVector = Vector3.right;
                verticalVector = Vector3.down;
                startPosition.x = mAnchorPosition.x - mAnchorScale.x * 0.5f;
                startPosition.y = mAnchorPosition.y + mAnchorScale.y * 0.5f;
                break;
            case AlignmentTypes.Top:
                horizontalVector = Vector3.right;
                verticalVector = Vector3.down;
                startPosition.x = mAnchorPosition.x;
                startPosition.y = mAnchorPosition.y + mAnchorScale.y * 0.5f;
                break;
            case AlignmentTypes.TopRight:
                horizontalVector = Vector3.left;
                verticalVector = Vector3.down;
                startPosition.x = mAnchorPosition.x + mAnchorScale.x * 0.5f;
                startPosition.y = mAnchorPosition.y + mAnchorScale.y * 0.5f;
                break;
            case AlignmentTypes.CenterLeft:
                horizontalVector = Vector3.right;
                verticalVector = Vector3.up;
                startPosition.x = mAnchorPosition.x - mAnchorScale.x * 0.5f;
                startPosition.y = mAnchorPosition.y;
                break;
            case AlignmentTypes.Center:
                horizontalVector = Vector3.right;
                verticalVector = Vector3.up;
                startPosition.x = mAnchorPosition.x;
                startPosition.y = mAnchorPosition.y;
                break;
            case AlignmentTypes.CenterRight:
                horizontalVector = Vector3.left;
                verticalVector = Vector3.up;
                startPosition.x = mAnchorPosition.x + mAnchorScale.x * 0.5f;
                startPosition.y = mAnchorPosition.y;
                break;
            case AlignmentTypes.BottomLeft:
                horizontalVector = Vector3.right;
                verticalVector = Vector3.up;
                startPosition.x = mAnchorPosition.x - mAnchorScale.x * 0.5f;
                startPosition.y = mAnchorPosition.y - mAnchorScale.y * 0.5f;
                break;
            case AlignmentTypes.Bottom:
                horizontalVector = Vector3.right;
                verticalVector = Vector3.up;
                startPosition.x = mAnchorPosition.x;
                startPosition.y = mAnchorPosition.y - mAnchorScale.y * 0.5f;
                break;
            case AlignmentTypes.BottomRight:
                horizontalVector = Vector3.left;
                verticalVector = Vector3.up;
                startPosition.x = mAnchorPosition.x + mAnchorScale.x * 0.5f;
                startPosition.y = mAnchorPosition.y - mAnchorScale.y * 0.5f;
                break;
            default:
                break;
        }

        //startPosition.z = mAnchorPosition.z - mAnchorScale.z * 0.5f;

        newPosition = startPosition + horizontalVector * xMagnitude + verticalVector * yMagnitude + Vector3.forward * zMagnitude;

        print(newPosition + " / " + xMagnitude + " / " + column);
        if (column == Columns-1)
        {
            print("---------------------");
        }

        return newPosition;
    }

    // Update is called once per frame
    protected virtual void Update () {
        BuildGrid();

    }
}
