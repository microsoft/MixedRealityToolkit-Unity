// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities
{
    /// <summary>
    /// This class has handy inspector utilities and functions.
    /// </summary>
    public static class MixedRealityInspectorUtility
    {
        public const float DottedLineScreenSpace = 4.65f;

        #region Colors

        public static readonly Color DisabledColor = new Color(0.6f, 0.6f, 0.6f);
        public static readonly Color WarningColor = new Color(1f, 0.85f, 0.6f);
        public static readonly Color ErrorColor = new Color(1f, 0.55f, 0.5f);
        public static readonly Color SuccessColor = new Color(0.8f, 1f, 0.75f);
        public static readonly Color SectionColor = new Color(0.85f, 0.9f, 1f);
        public static readonly Color DarkColor = new Color(0.1f, 0.1f, 0.1f);
        public static readonly Color HandleColorSquare = new Color(0.0f, 0.9f, 1f);
        public static readonly Color HandleColorCircle = new Color(1f, 0.5f, 1f);
        public static readonly Color HandleColorSphere = new Color(1f, 0.5f, 1f);
        public static readonly Color HandleColorAxis = new Color(0.0f, 1f, 0.2f);
        public static readonly Color HandleColorRotation = new Color(0.0f, 1f, 0.2f);
        public static readonly Color HandleColorTangent = new Color(0.1f, 0.8f, 0.5f, 0.7f);

        #endregion Colors

        #region Handles

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="handleSize"></param>
        /// <param name="recordUndo"></param>
        /// <param name="autoSize"></param>
        /// <returns></returns>
        public static float AxisMoveHandle(Object target, Vector3 origin, Vector3 direction, float distance, float handleSize = 0.2f, bool recordUndo = true, bool autoSize = true)
        {
            Vector3 position = origin + (direction.normalized * distance);

            Handles.color = HandleColorAxis;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            Handles.DrawDottedLine(origin, position, DottedLineScreenSpace);
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(direction), handleSize * 2, EventType.Repaint);
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, Vector3.zero, Handles.CircleHandleCap);

            if (recordUndo)
            {
                float newDistance = Vector3.Distance(origin, newPosition);

                if (!distance.Equals(newDistance))
                {
                    Undo.RegisterCompleteObjectUndo(target, target.name);
                    distance = newDistance;
                }
            }

            return distance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="handleSize"></param>
        /// <param name="autoSize"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <param name="recordUndo"></param>
        /// <returns></returns>
        public static Vector3 CircleMoveHandle(Object target, Vector3 position, float handleSize = 0.2f, bool autoSize = true, float xScale = 1f, float yScale = 1f, float zScale = 1f, bool recordUndo = true)
        {
            Handles.color = HandleColorCircle;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, Vector3.zero, Handles.CircleHandleCap);

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="handleSize"></param>
        /// <param name="autoSize"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <param name="recordUndo"></param>
        /// <returns></returns>
        public static Vector3 SquareMoveHandle(Object target, Vector3 position, float handleSize = 0.2f, bool autoSize = true, float xScale = 1f, float yScale = 1f, float zScale = 1f, bool recordUndo = true)
        {
            Handles.color = HandleColorSquare;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Multiply square handle to match other types
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize * 0.8f, Vector3.zero, Handles.RectangleHandleCap);

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="handleSize"></param>
        /// <param name="autoSize"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <param name="recordUndo"></param>
        /// <returns></returns>
        public static Vector3 SphereMoveHandle(Object target, Vector3 position, float handleSize = 0.2f, bool autoSize = true, float xScale = 1f, float yScale = 1f, float zScale = 1f, bool recordUndo = true)
        {
            Handles.color = HandleColorSphere;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Multiply sphere handle size to match other types
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize * 2, Vector3.zero, Handles.SphereHandleCap);

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="origin"></param>
        /// <param name="vector"></param>
        /// <param name="normalize"></param>
        /// <param name="handleLength"></param>
        /// <param name="clamp"></param>
        /// <param name="handleSize"></param>
        /// <param name="recordUndo"></param>
        /// <param name="autoSize"></param>
        /// <returns></returns>
        public static Vector3 VectorHandle(Object target, Vector3 origin, Vector3 vector, bool normalize = true, float handleLength = 1f, bool clamp = true, float handleSize = 0.1f, bool recordUndo = true, bool autoSize = true)
        {
            Handles.color = HandleColorTangent;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(origin) * handleSize, 0.75f);
            }

            Vector3 handlePosition = origin + (vector * handleLength);
            float distanceToOrigin = Vector3.Distance(origin, handlePosition) / handleLength;

            if (normalize)
            {
                vector.Normalize();
            }
            else
            {
                // If the handle isn't normalized, brighten based on distance to origin
                Handles.color = Color.Lerp(Color.gray, HandleColorTangent, distanceToOrigin * 0.85f);
                if (clamp)
                {
                    // To indicate that we're at the clamped limit, make the handle 'pop' slightly larger
                    if (distanceToOrigin >= 0.98f)
                    {
                        Handles.color = Color.Lerp(HandleColorTangent, Color.white, 0.5f);
                        handleSize *= 1.5f;
                    }
                }
            }

            // Draw a line from origin to origin + direction
            Handles.DrawLine(origin, handlePosition);

            Quaternion rotation = Quaternion.identity;
            if (vector != Vector3.zero)
            {
                rotation = Quaternion.LookRotation(vector);
            }

            Vector3 newPosition = Handles.FreeMoveHandle(handlePosition, rotation, handleSize, Vector3.zero, Handles.DotHandleCap);

            if (recordUndo && handlePosition != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);
                vector = (newPosition - origin).normalized;

                // If we normalize, we're done
                // Otherwise, multiply the vector by the distance between origin and target
                if (!normalize)
                {
                    distanceToOrigin = Vector3.Distance(origin, newPosition) / handleLength;
                    if (clamp)
                    {
                        distanceToOrigin = Mathf.Clamp01(distanceToOrigin);
                    }

                    vector *= distanceToOrigin;
                }
            }

            return vector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="handleSize"></param>
        /// <param name="autoSize"></param>
        /// <param name="recordUndo"></param>
        /// <returns></returns>
        public static Quaternion RotationHandle(Object target, Vector3 position, Quaternion rotation, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorRotation;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Make rotation handles larger so they can overlay movement handles
            Quaternion newRotation = Handles.FreeRotateHandle(rotation, position, handleSize * 2);

            if (recordUndo)
            {
                Handles.color = Handles.zAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.forward), handleSize * 2, EventType.Repaint);
                Handles.color = Handles.xAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.right), handleSize * 2, EventType.Repaint);
                Handles.color = Handles.yAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.up), handleSize * 2, EventType.Repaint);

                if (rotation != newRotation)
                {
                    Undo.RegisterCompleteObjectUndo(target, target.name);

                    rotation = newRotation;
                }
            }

            return rotation;
        }

        #endregion Handles
    }
}
