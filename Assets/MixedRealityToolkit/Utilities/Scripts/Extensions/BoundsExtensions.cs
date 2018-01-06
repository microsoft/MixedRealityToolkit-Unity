//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;

public static class BoundsExtensions
{
    // Corners
    public const int LBF = 0;
    public const int LBB = 1;
    public const int LTF = 2;
    public const int LTB = 3;
    public const int RBF = 4;
    public const int RBB = 5;
    public const int RTF = 6;
    public const int RTB = 7;

    // X axis
    public const int LTF_RTF = 8;
    public const int LBF_RBF = 9;
    public const int RTB_LTB = 10;
    public const int RBB_LBB = 11;

    // Y axis
    public const int LTF_LBF = 12;
    public const int RTB_RBB = 13;
    public const int LTB_LBB = 14;
    public const int RTF_RBF = 15;

    // Z axis
    public const int RBF_RBB = 16;
    public const int RTF_RTB = 17;
    public const int LBF_LBB = 18;
    public const int LTF_LTB = 19;

    // 2D corners
    public const int LT = 0;
    public const int LB = 1;
    public const int RT = 2;
    public const int RB = 3;

    // 2D midpoints
    public const int LT_RT = 4;
    public const int RT_RB = 5;
    public const int RB_LB = 6;
    public const int LB_LT = 7;

    // Face points
    public const int TOP = 0;
    public const int BOT = 1;
    public const int LFT = 2;
    public const int RHT = 3;
    public const int FWD = 4;
    public const int BCK = 5;

    public enum Axis
    {
        X,
        Y,
        Z
    }

    #region Public Static Functions
    /// <summary>
    /// Returns an instance of the 'Bounds' class which is invalid. An invalid 'Bounds' instance 
    /// is one which has its size vector set to 'float.MaxValue' for all 3 components. The center
    /// of an invalid bounds instance is the zero vector.
    /// </summary>
    public static Bounds GetInvalidBoundsInstance()
    {
        return new Bounds(Vector3.zero, GetInvalidBoundsSize());
    }

    /// <summary>
    /// Checks if the specified bounds instance is valid. A valid 'Bounds' instance is
    /// one whose size vector does not have all 3 components set to 'float.MaxValue'.
    /// </summary>
    public static bool IsValid(this Bounds bounds)
    {
        return bounds.size != GetInvalidBoundsSize();
    }

    /// <summary>
    /// Gets all the corner points of the bounds in world space
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="positions"></param>
    /// <remarks>
    /// Use BoxColliderExtensions.{Left|Right}{Bottom|Top}{Front|Back} consts to index into the output
    /// corners array.
    /// </remarks>
    public static void GetCornerPositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
    {
        // Calculate the local points to transform.
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        float leftEdge = center.x - extents.x;
        float rightEdge = center.x + extents.x;
        float bottomEdge = center.y - extents.y;
        float topEdge = center.y + extents.y;
        float frontEdge = center.z - extents.z;
        float backEdge = center.z + extents.z;

        // Allocate the array if needed.
        const int numPoints = 8;
        if (positions == null || positions.Length != numPoints)
        {
            positions = new Vector3[numPoints];
        }

        // Transform all the local points to world space.
        positions[BoundsExtensions.LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
        positions[BoundsExtensions.LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
        positions[BoundsExtensions.LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
        positions[BoundsExtensions.LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
        positions[BoundsExtensions.RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
        positions[BoundsExtensions.RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
        positions[BoundsExtensions.RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
        positions[BoundsExtensions.RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);
    }

    /// <summary>
    /// Gets all the corner points from Renderer's Bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="positions"></param>
    public static void GetCornerPositionsFromRendererBounds(this Bounds bounds, ref Vector3[] positions)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        float leftEdge = center.x - extents.x;
        float rightEdge = center.x + extents.x;
        float bottomEdge = center.y - extents.y;
        float topEdge = center.y + extents.y;
        float frontEdge = center.z - extents.z;
        float backEdge = center.z + extents.z;

        const int numPoints = 8;
        if (positions == null || positions.Length != numPoints)
        {
            positions = new Vector3[numPoints];
        }

        positions[BoundsExtensions.LBF] = new Vector3(leftEdge, bottomEdge, frontEdge);
        positions[BoundsExtensions.LBB] = new Vector3(leftEdge, bottomEdge, backEdge);
        positions[BoundsExtensions.LTF] = new Vector3(leftEdge, topEdge, frontEdge);
        positions[BoundsExtensions.LTB] = new Vector3(leftEdge, topEdge, backEdge);
        positions[BoundsExtensions.RBF] = new Vector3(rightEdge, bottomEdge, frontEdge);
        positions[BoundsExtensions.RBB] = new Vector3(rightEdge, bottomEdge, backEdge);
        positions[BoundsExtensions.RTF] = new Vector3(rightEdge, topEdge, frontEdge);
        positions[BoundsExtensions.RTB] = new Vector3(rightEdge, topEdge, backEdge);
    }

    public static void GetFacePositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        const int numPoints = 6;
        if (positions == null || positions.Length != numPoints)
        {
            positions = new Vector3[numPoints];
        }

        positions[BoundsExtensions.TOP] = transform.TransformPoint(center + Vector3.up * extents.y);
        positions[BoundsExtensions.BOT] = transform.TransformPoint(center + Vector3.down * extents.y);
        positions[BoundsExtensions.LFT] = transform.TransformPoint(center + Vector3.left * extents.x);
        positions[BoundsExtensions.RHT] = transform.TransformPoint(center + Vector3.right * extents.x);
        positions[BoundsExtensions.FWD] = transform.TransformPoint(center + Vector3.forward * extents.z);
        positions[BoundsExtensions.BCK] = transform.TransformPoint(center + Vector3.back * extents.z);
    }

    /// <summary>
    /// Gets all the corner points and mid points from Renderer's Bounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="positions"></param>
    public static void GetCornerAndMidPointPositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
    {
        // Calculate the local points to transform.
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        float leftEdge = center.x - extents.x;
        float rightEdge = center.x + extents.x;
        float bottomEdge = center.y - extents.y;
        float topEdge = center.y + extents.y;
        float frontEdge = center.z - extents.z;
        float backEdge = center.z + extents.z;

        // Allocate the array if needed.
        const int numPoints = BoundsExtensions.LTF_LTB + 1;
        if (positions == null || positions.Length != numPoints)
        {
            positions = new Vector3[numPoints];
        }

        // Transform all the local points to world space.
        positions[BoundsExtensions.LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
        positions[BoundsExtensions.LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
        positions[BoundsExtensions.LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
        positions[BoundsExtensions.LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
        positions[BoundsExtensions.RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
        positions[BoundsExtensions.RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
        positions[BoundsExtensions.RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
        positions[BoundsExtensions.RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);

        positions[BoundsExtensions.LTF_RTF] = Vector3.Lerp(positions[BoundsExtensions.LTF], positions[BoundsExtensions.RTF], 0.5f);
        positions[BoundsExtensions.LBF_RBF] = Vector3.Lerp(positions[BoundsExtensions.LBF], positions[BoundsExtensions.RBF], 0.5f);
        positions[BoundsExtensions.RTB_LTB] = Vector3.Lerp(positions[BoundsExtensions.RTB], positions[BoundsExtensions.LTB], 0.5f);
        positions[BoundsExtensions.RBB_LBB] = Vector3.Lerp(positions[BoundsExtensions.RBB], positions[BoundsExtensions.LBB], 0.5f);

        positions[BoundsExtensions.LTF_LBF] = Vector3.Lerp(positions[BoundsExtensions.LTF], positions[BoundsExtensions.LBF], 0.5f);
        positions[BoundsExtensions.RTB_RBB] = Vector3.Lerp(positions[BoundsExtensions.RTB], positions[BoundsExtensions.RBB], 0.5f);
        positions[BoundsExtensions.LTB_LBB] = Vector3.Lerp(positions[BoundsExtensions.LTB], positions[BoundsExtensions.LBB], 0.5f);
        positions[BoundsExtensions.RTF_RBF] = Vector3.Lerp(positions[BoundsExtensions.RTF], positions[BoundsExtensions.RBF], 0.5f);

        positions[BoundsExtensions.RBF_RBB] = Vector3.Lerp(positions[BoundsExtensions.RBF], positions[BoundsExtensions.RBB], 0.5f);
        positions[BoundsExtensions.RTF_RTB] = Vector3.Lerp(positions[BoundsExtensions.RTF], positions[BoundsExtensions.RTB], 0.5f);
        positions[BoundsExtensions.LBF_LBB] = Vector3.Lerp(positions[BoundsExtensions.LBF], positions[BoundsExtensions.LBB], 0.5f);
        positions[BoundsExtensions.LTF_LTB] = Vector3.Lerp(positions[BoundsExtensions.LTF], positions[BoundsExtensions.LTB], 0.5f);
    }

    /// <summary>
    /// Gets all the corner points and mid points from Renderer's Bounds, ignoring the z axis
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="positions"></param>
    public static void GetCornerAndMidPointPositions2D(this Bounds bounds, Transform transform, ref Vector3[] positions, Axis flattenAxis)
    {
        // Calculate the local points to transform.
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        float leftEdge = 0;
        float rightEdge = 0;
        float bottomEdge = 0;
        float topEdge = 0;

        // Allocate the array if needed.
        const int numPoints = BoundsExtensions.LB_LT + 1;
        if (positions == null || positions.Length != numPoints)
        {
            positions = new Vector3[numPoints];
        }

        switch (flattenAxis)
        {
            case Axis.X:
            default:
                leftEdge = center.z - extents.z;
                rightEdge = center.z + extents.z;
                bottomEdge = center.y - extents.y;
                topEdge = center.y + extents.y;
                // Transform all the local points to world space.
                positions[BoundsExtensions.LT] = transform.TransformPoint(0, topEdge, leftEdge);
                positions[BoundsExtensions.LB] = transform.TransformPoint(0, bottomEdge, leftEdge);
                positions[BoundsExtensions.RT] = transform.TransformPoint(0, topEdge, rightEdge);
                positions[BoundsExtensions.RB] = transform.TransformPoint(0, bottomEdge, rightEdge);
                break;

            case Axis.Y:
                leftEdge = center.z - extents.z;
                rightEdge = center.z + extents.z;
                bottomEdge = center.x - extents.x;
                topEdge = center.x + extents.x;
                // Transform all the local points to world space.
                positions[BoundsExtensions.LT] = transform.TransformPoint(topEdge, 0, leftEdge);
                positions[BoundsExtensions.LB] = transform.TransformPoint(bottomEdge, 0, leftEdge);
                positions[BoundsExtensions.RT] = transform.TransformPoint(topEdge, 0, rightEdge);
                positions[BoundsExtensions.RB] = transform.TransformPoint(bottomEdge, 0, rightEdge);
                break;

            case Axis.Z:
                leftEdge = center.x - extents.x;
                rightEdge = center.x + extents.x;
                bottomEdge = center.y - extents.y;
                topEdge = center.y + extents.y;
                // Transform all the local points to world space.
                positions[BoundsExtensions.LT] = transform.TransformPoint(leftEdge, topEdge, 0);
                positions[BoundsExtensions.LB] = transform.TransformPoint(leftEdge, bottomEdge, 0);
                positions[BoundsExtensions.RT] = transform.TransformPoint(rightEdge, topEdge, 0);
                positions[BoundsExtensions.RB] = transform.TransformPoint(rightEdge, bottomEdge, 0);
                break;
        }

        positions[BoundsExtensions.LT_RT] = Vector3.Lerp(positions[BoundsExtensions.LT], positions[BoundsExtensions.RT], 0.5f);
        positions[BoundsExtensions.RT_RB] = Vector3.Lerp(positions[BoundsExtensions.RT], positions[BoundsExtensions.RB], 0.5f);
        positions[BoundsExtensions.RB_LB] = Vector3.Lerp(positions[BoundsExtensions.RB], positions[BoundsExtensions.LB], 0.5f);
        positions[BoundsExtensions.LB_LT] = Vector3.Lerp(positions[BoundsExtensions.LB], positions[BoundsExtensions.LT], 0.5f);
    }

    /// <summary>
    /// Transforms 'bounds' using the specified transform matrix.
    /// </summary>
    /// <remarks>
    /// Transforming a 'Bounds' instance means that the function will construct a new 'Bounds' 
    /// instance which has its center translated using the translation information stored in
    /// the specified matrix and its size adjusted to account for rotation and scale. The size
    /// of the new 'Bounds' instance will be calculated in such a way that it will contain the
    /// old 'Bounds'.
    /// </remarks>
    /// <param name="bounds">
    /// The 'Bounds' instance which must be transformed.
    /// </param>
    /// <param name="transformMatrix">
    /// The specified 'Bounds' instance will be transformed using this transform matrix. The function
    /// assumes that the matrix doesn't contain any projection or skew transformation.
    /// </param>
    /// <returns>
    /// The transformed 'Bounds' instance.
    /// </returns>
    public static Bounds Transform(this Bounds bounds, Matrix4x4 transformMatrix)
    {
        // We will need access to the right, up and look vector which are encoded inside the transform matrix
        Vector3 rightAxis = transformMatrix.GetColumn(0);
        Vector3 upAxis = transformMatrix.GetColumn(1);
        Vector3 lookAxis = transformMatrix.GetColumn(2);

        // We will 'imagine' that we want to rotate the bounds' extents vector using the rotation information
        // stored inside the specified transform matrix. We will need these when calculating the new size if
        // the transformed bounds.
        Vector3 rotatedExtentsRight = rightAxis * bounds.extents.x;
        Vector3 rotatedExtentsUp = upAxis * bounds.extents.y;
        Vector3 rotatedExtentsLook = lookAxis * bounds.extents.z;

        // Calculate the new bounds size along each axis. The size on each axis is calculated by summing up the 
        // corresponding vector component values of the rotated extents vectors. We multiply by 2 because we want
        // to get a size and curently we are working with extents which represent half the size.
        float newSizeX = (Mathf.Abs(rotatedExtentsRight.x) + Mathf.Abs(rotatedExtentsUp.x) + Mathf.Abs(rotatedExtentsLook.x)) * 2.0f;
        float newSizeY = (Mathf.Abs(rotatedExtentsRight.y) + Mathf.Abs(rotatedExtentsUp.y) + Mathf.Abs(rotatedExtentsLook.y)) * 2.0f;
        float newSizeZ = (Mathf.Abs(rotatedExtentsRight.z) + Mathf.Abs(rotatedExtentsUp.z) + Mathf.Abs(rotatedExtentsLook.z)) * 2.0f;

        // Construct the transformed 'Bounds' instance
        var transformedBounds = new Bounds();
        transformedBounds.center = transformMatrix.MultiplyPoint(bounds.center);
        transformedBounds.size = new Vector3(newSizeX, newSizeY, newSizeZ);

        // Return the instance to the caller
        return transformedBounds;
    }

    /// <summary>
    /// Returns the screen space corner points of the specified 'Bounds' instance.
    /// </summary>
    /// <param name="camera">
    /// The camera used for rendering to the screen. This is needed to perform the
    /// transformation to screen space.
    /// </param>
    public static Vector2[] GetScreenSpaceCornerPoints(this Bounds bounds, Camera camera)
    {
        Vector3 aabbCenter = bounds.center;
        Vector3 aabbExtents = bounds.extents;

        //  Return the screen space point array
        return new Vector2[]
        {
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z - aabbExtents.z)),

            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z + aabbExtents.z))
        };
    }

    /// <summary>
    /// Returns the rectangle which encloses the specifies 'Bounds' instance in screen space.
    /// </summary>
    public static Rect GetScreenRectangle(this Bounds bounds, Camera camera)
    {
        // Retrieve the bounds' corner points in screen space
        Vector2[] screenSpaceCornerPoints = bounds.GetScreenSpaceCornerPoints(camera);

        // Identify the minimum and maximum points in the array
        Vector3 minScreenPoint = screenSpaceCornerPoints[0], maxScreenPoint = screenSpaceCornerPoints[0];
        for (int screenPointIndex = 1; screenPointIndex < screenSpaceCornerPoints.Length; ++screenPointIndex)
        {
            minScreenPoint = Vector3.Min(minScreenPoint, screenSpaceCornerPoints[screenPointIndex]);
            maxScreenPoint = Vector3.Max(maxScreenPoint, screenSpaceCornerPoints[screenPointIndex]);
        }

        // Return the screen space rectangle
        return new Rect(minScreenPoint.x, minScreenPoint.y, maxScreenPoint.x - minScreenPoint.x, maxScreenPoint.y - minScreenPoint.y);
    }

    /// <summary>
    /// Returns the volume of the bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public static float Volume(this Bounds bounds)
    {
        return bounds.size.x * bounds.size.y * bounds.size.z;
    }

    /// <summary>
    /// Returns bounds that contain both this bounds and the bounds passed in.
    /// </summary>
    /// <param name="originalBounds"></param>
    /// <param name="otherBounds"></param>
    /// <returns></returns>
    public static Bounds ExpandToContain(this Bounds originalBounds, Bounds otherBounds)
    {
        Bounds tmpBounds = originalBounds;

        tmpBounds.Encapsulate(otherBounds);

        return tmpBounds;
    }

    /// <summary>
    /// Checks to see if bounds contains the other bounds completely.
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="otherBounds"></param>
    /// <returns></returns>
    public static bool ContainsBounds(this Bounds bounds, Bounds otherBounds)
    {
        return bounds.Contains(otherBounds.min) && bounds.Contains(otherBounds.max);
    }

    /// <summary>
    /// Checks to see whether point is closer to bounds or otherBounds
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="point"></param>
    /// <param name="otherBounds"></param>
    /// <returns></returns>
    public static bool CloserToPoint(this Bounds bounds, Vector3 point, Bounds otherBounds)
    {
        Vector3 distToClosestPoint1 = bounds.ClosestPoint(point) - point;
        Vector3 distToClosestPoint2 = otherBounds.ClosestPoint(point) - point;

        if (distToClosestPoint1.magnitude == distToClosestPoint2.magnitude)
        {
            Vector3 toCenter1 = point - bounds.center;
            Vector3 toCenter2 = point - otherBounds.center;
            return (toCenter1.magnitude <= toCenter2.magnitude);

        }
        else
        {
            return (distToClosestPoint1.magnitude <= distToClosestPoint2.magnitude);
        }
    }

    #endregion

    #region Private Static Functions
    /// <summary>
    /// Returns the vector which is used to represent and invalid bounds size.
    /// </summary>
    private static Vector3 GetInvalidBoundsSize()
    {
        return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }
    #endregion
}