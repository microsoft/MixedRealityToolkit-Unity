//
// Copyright (C) Microsoft. All rights reserved.
//

using UnityEngine;

namespace HoloToolkit.Sharing.Utilities
{
    public static class Utils
    {
        public static void SetLayerRecursively(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            for (int i = 0; i < gameObject.transform.childCount; ++i)
            {
                SetLayerRecursively(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }

        /// <summary>
        /// This takes a value, whose proportions are defined by minA and maxA,
        /// and scales it to a space defined by minB and maxB. For example, 5 in
        /// the space of 0 to 10 will map to 50 in the space of 0 to 100.
        /// </summary>
        /// <param name="minA">lower bound for the original scale</param>
        /// <param name="maxA">upper bound for the original scale</param>
        /// <param name="minB">lower bound for new scale</param>
        /// <param name="maxB">upper bound for new scale</param>
        /// <param name="value">the original scale value to convert to the new
        /// scale.</param>
        /// <returns>"value" mapped to the space defined by minB and maxB.</returns>
        public static float Map(float minA, float maxA, float minB, float maxB, float value)
        {
            return (value - minA) * (maxB / maxA) + minB;
        }

        /// <summary>
        /// This wrapper performs a map, but sets the results upper and lower limit
        /// based on the upper and lower bounds of the new scale.
        /// </summary>
        /// <param name="minA">lower bound for the original scale</param>
        /// <param name="maxA">upper bound for the original scale</param>
        /// <param name="minB">lower bound for new scale</param>
        /// <param name="maxB">upper bound for new scale</param>
        /// <param name="value">the original scale value to convert to the new
        /// scale.</param>
        /// <returns>"value" mapped to the space defined by minB and maxB,
        /// constrained by minB and maxB.</returns>
        public static float MapAndClamp(float minA, float maxA, float minB, float maxB, float value)
        {
            return Mathf.Clamp(Map(minA, maxA, minB, maxB, value), minB, maxB);
        }

        /// <summary>
        /// Position transform along the gaze direction and orient yaw to match, with the specified offset
        /// </summary>
        /// <param name="stageTransform">transform of higher-level space where 0,1,0 is up</param>
        /// <param name="tran">transform to be repositioned</param>
        /// <param name="offset">translation offset, Z is forward</param>
        /// <param name="yawOffset">yaw offset</param>
        public static void MoveObjectInFrontOfUser(Transform stageTransform, Transform tran, Vector3 offset, float yawOffset)
        {
            // have obj track head position with translation offset
            Vector3 stageHeadPos = MathUtils.TransformPointFromTo(null, stageTransform, Camera.main.transform.transform.position);
            Vector3 stageHeadDir = MathUtils.TransformDirectionFromTo(null, stageTransform, Camera.main.transform.transform.forward);
            stageHeadDir.y = 0.0f; // ignore head pitch - use head position to set height
            stageHeadDir.Normalize();
            Vector3 sideDir = Vector3.Cross(stageHeadDir, Vector3.up).normalized;
            Vector3 stageNewPos = stageHeadPos + stageHeadDir * offset.z + Vector3.up * offset.y + sideDir * offset.x;
            tran.localPosition = MathUtils.TransformPointFromTo(stageTransform, tran.parent, stageNewPos);

            // also track head yaw
            Vector3 toDir = MathUtils.TransformDirectionFromTo(stageTransform, tran.parent, stageHeadDir);
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, toDir.normalized) * Quaternion.Euler(0.0f, yawOffset, 0.0f);
            tran.localRotation = rot;
        }

        /// <summary>
        /// Given two points and the transform that they are in, set position, rotation, and scale of a cylinder transform to connect it between the two points
        /// </summary>
        /// <param name="endPointSpace"></param>
        /// <param name="a">One end point in the space of endPointSpace</param>
        /// <param name="b">Other end point in the space of endPointSpace</param>
        /// <param name="cylTransform">transform for the cylinder primitive to connect</param>
        public static void ConnectCylinderBetweenPoints(Transform endPointSpace, Vector3 a, Vector3 b, Transform cylTransform)
        {
            cylTransform.localPosition = MathUtils.TransformPointFromTo(endPointSpace, cylTransform.parent, 0.5f * (a + b));
            Vector3 dir = MathUtils.TransformDirectionFromTo(endPointSpace, cylTransform.parent, (b - a).normalized);
            cylTransform.localRotation = Quaternion.LookRotation(dir) * Quaternion.FromToRotation(Vector3.up, Vector3.forward);
            Vector3 scale = cylTransform.localScale;
            scale.y = (a - b).magnitude * 0.5f;
            cylTransform.localScale = scale;
        }

        /// <summary>
        /// Change material for every object in hierarchy
        /// </summary>
        /// <param name="t">root transform to start looking for renderers</param>
        /// <param name="mat">material to set everything to</param>
        public static void SetMaterialRecursive(Transform t, Material mat)
        {
            if (t.gameObject && t.gameObject.GetComponent<Renderer>())
            {
                t.gameObject.GetComponent<Renderer>().material = mat;
            }

            for (int ii = 0; ii < t.childCount; ++ii)
            {
                SetMaterialRecursive(t.GetChild(ii), mat);
            }
        }

        /// <summary>
        /// Change material for every object in hierarchy which has a name equal to nameToTest.  WARNING! 
        /// <see cref="http://answers.unity3d.com/questions/548420/material-memory-leak.html">See Community Answer</see>
        /// This function automatically instantiates the materials and makes them unique to this renderer.
        /// It is your responsibility to destroy the materials when the game object is being destroyed.
        /// Resources.UnloadUnusedAssets also destroys the materials but it is usually only called when loading a new level.
        /// <see cref="https://docs.unity3d.com/ScriptReference/Renderer-material.html">See Unity Documentation</see>
        /// </summary>
        /// <param name="t">root transform to start looking for renderers</param>
        /// <param name="mat">material to set everything to</param>
        /// <param name="nameToTest">ignore GameObjects with this name</param>
        public static void SetMaterialRecursiveForName(Transform t, Material mat, string nameToTest)
        {
            if (t.gameObject && t.gameObject.GetComponent<Renderer>() && t.gameObject.name == nameToTest)
            {
                t.gameObject.GetComponent<Renderer>().material = mat;
            }

            for (int ii = 0; ii < t.childCount; ++ii)
            {
                SetMaterialRecursiveForName(t.GetChild(ii), mat, nameToTest);
            }

            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// helper for detecting if running in editor
        /// </summary>
        /// <returns>true if running in editor, false if windows store app</returns>
        public static bool IsInEditor()
        {
#if UNITY_METRO && !UNITY_EDITOR
        return false;
#else
            return true;
#endif
        }

        /// <summary>
        /// walk hierarchy looking for named transform
        /// </summary>
        /// <param name="t">root transform to start searching from</param>
        /// <param name="name">name to look for</param>
        /// <returns>returns found transform or null if none found</returns>
        public static Transform GetChildRecursive(Transform t, string name)
        {
            int numChildren = t.childCount;
            for (int ii = 0; ii < numChildren; ++ii)
            {
                Transform child = t.GetChild(ii);
                if (child.name == name)
                {
                    return child;
                }
                Transform foundIt = GetChildRecursive(child, name);
                if (foundIt != null)
                {
                    return foundIt;
                }
            }
            return null;
        }
    }
}