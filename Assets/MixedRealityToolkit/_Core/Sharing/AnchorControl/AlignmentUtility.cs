using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AnchorControl
{
    // Adapted from zalo's open source math library, which implements the following technique:
    // https://animation.rwth-aachen.de/media/papers/2016-MIG-StableRotation.pdf
    public static class AlignmentUtility
    {
        private static Vector3[] sourcePoints = new Vector3[1];
        private static Vector4[] targetPoints = new Vector4[1];
        private static Vector3[] quaternionBasis = new Vector3[3];
        private static Vector3[] dataCovariance = new Vector3[3];
        private static Matrix4x4 transformMatrix;

        private static Vector3 scale;
        private static Vector3 position;
        private static Quaternion rotation;
        private static readonly Matrix4x4 identityMatrix = Matrix4x4.identity;

        private static Vector3 targetTransformPos;
        private static Vector4 targetPoint;

        private static Vector3 trns;
        private static Vector3 fwd;
        private static Vector3 up;
        private static Vector3 scl;

        public static void Align(Transform sceneRoot, List<Transform> sourceTransforms, List<Transform> targetTransforms, bool applyRotation = true, bool applyScale = false, bool yRotationOnly = true)
        {
            Debug.Assert(sceneRoot != null);
            Debug.Assert(sourceTransforms != null);
            Debug.Assert(targetTransforms != null);
            Debug.Assert(sourceTransforms.Count > 0 && targetTransforms.Count > 0);
            Debug.Assert(sourceTransforms.Count == targetTransforms.Count);

            if (sourcePoints.Length != sourceTransforms.Count)
            {
                Array.Resize<Vector3>(ref sourcePoints, sourceTransforms.Count);
                Array.Resize<Vector4>(ref targetPoints, sourceTransforms.Count);
            }

            for (int i = 0; i < sourceTransforms.Count; i++)
            {
                sourcePoints[i] = sourceTransforms[i].position;
            }

            for (int j = 0; j < sourceTransforms.Count; j++)
            {
                targetTransformPos = targetTransforms[j].position;
                targetPoint.x = targetTransformPos.x;
                targetPoint.y = targetTransformPos.y;
                targetPoint.z = targetTransformPos.z;
                targetPoint.w = targetTransforms[j].localScale.x;
                targetPoints[j] = targetPoint;// new Vector4(targetTransforms[j].position.x, targetTransforms[j].position.y, targetTransforms[j].position.z, targetTransforms[j].localScale.x);
            }

            transformMatrix = FindTransformation(sourcePoints, targetPoints, applyRotation, applyScale);

            DecomposeMatrix(transformMatrix, out position, out rotation, out scale);

            sceneRoot.localScale = scale;
            sceneRoot.localPosition = position;

            if (yRotationOnly)
            {
                // Remove x and z rotation
                Vector3 eulerAngles = rotation.eulerAngles;
                eulerAngles.x = 0f;
                eulerAngles.z = 0f;
                sceneRoot.localEulerAngles = eulerAngles;

                // Adjust position based on the average height of the source and target points
                float sourceYPos = 0f;
                float targetYPos = 0f;

                for (int i = 0; i < sourceTransforms.Count; i++)
                {
                    sourceYPos += sourceTransforms[i].position.y;
                }

                for (int i = 0; i < targetTransforms.Count; i++)
                {
                    targetYPos += targetTransforms[i].position.y;
                }

                sourceYPos /= sourceTransforms.Count;
                targetYPos /= targetTransforms.Count;

                sceneRoot.localPosition += Vector3.up * (targetYPos - sourceYPos);

            }
            else
            {
                sceneRoot.localRotation = rotation;
            }

        }

        public static Matrix4x4 FindTransformation(Vector3[] sourcePoints, Vector4[] targetPoints, bool applyRotation, bool applyScale)
        {

            float OutputScale = 1f;
            Quaternion OutputRotation = Quaternion.identity;

            // Calculate the centroid offset and construct the centroid-shifted point matrices
            Vector3 inCentroid = Vector3.zero;
            Vector3 refCentroid = Vector3.zero;
            float inTotal = 0f;
            float refTotal = 0f;

            for (int i = 0; i < sourcePoints.Length; i++)
            {
                inCentroid += new Vector3(sourcePoints[i].x, sourcePoints[i].y, sourcePoints[i].z) * targetPoints[i].w;
                inTotal += targetPoints[i].w;
                refCentroid += new Vector3(targetPoints[i].x, targetPoints[i].y, targetPoints[i].z) * targetPoints[i].w;
                refTotal += targetPoints[i].w;
            }
            inCentroid /= inTotal;
            refCentroid /= refTotal;

            // Calculate the scale ratio
            if (applyScale)
            {

                float inScale = 0f;
                float refScale = 0f;

                for (int i = 0; i < sourcePoints.Length; i++)
                {
                    inScale += (new Vector3(sourcePoints[i].x, sourcePoints[i].y, sourcePoints[i].z) - inCentroid).magnitude;
                    refScale += (new Vector3(targetPoints[i].x, targetPoints[i].y, targetPoints[i].z) - refCentroid).magnitude;
                }
                OutputScale = (refScale / inScale);
            }

            // Calculate the 3x3 covariance matrix, and the optimal rotation
            if (applyRotation)
            {
                ExtractRotation(TransposeMultSubtract(sourcePoints, targetPoints, inCentroid, refCentroid, dataCovariance), ref OutputRotation);
            }

            return Matrix4x4.TRS(refCentroid, Quaternion.identity, Vector3.one * OutputScale) *
                   Matrix4x4.TRS(Vector3.zero, OutputRotation, Vector3.one) *
                   Matrix4x4.TRS(-inCentroid, Quaternion.identity, Vector3.one);
        }

        private static void ExtractRotation(Vector3[] points, ref Quaternion rotation)
        {
            for (int i = 0; i < 9; i++)
            {

                rotation.FillMatrixFromQuaternion(ref quaternionBasis);

                Vector3 omega = (Vector3.Cross(quaternionBasis[0], points[0]) +
                                 Vector3.Cross(quaternionBasis[1], points[1]) +
                                 Vector3.Cross(quaternionBasis[2], points[2])) *
                 (1f / Mathf.Abs(Vector3.Dot(quaternionBasis[0], points[0]) +
                                 Vector3.Dot(quaternionBasis[1], points[1]) +
                                 Vector3.Dot(quaternionBasis[2], points[2]) + 0.000000001f));

                float w = omega.magnitude;

                if (w < 0.000000001f)
                {
                    break;
                }

                rotation = Quaternion.AngleAxis(w * Mathf.Rad2Deg, omega / w) * rotation;
                rotation = Quaternion.Lerp(rotation, rotation, 0f); // Normalizes the Quaternion; critical for error suppression
            }
        }

        public static Vector3 ExtractTranslationFromMatrix(Matrix4x4 matrix)
        {
            trns.x = matrix.m03;
            trns.y = matrix.m13;
            trns.z = matrix.m23;
            return trns;
        }

        public static Quaternion ExtractRotationFromMatrix(Matrix4x4 matrix)
        {
            fwd.x = matrix.m02;
            fwd.y = matrix.m12;
            fwd.z = matrix.m22;

            up.x = matrix.m01;
            up.y = matrix.m11;
            up.z = matrix.m21;

            return Quaternion.LookRotation(fwd, up);
        }

        public static Vector3 ExtractScaleFromMatrix(Matrix4x4 matrix)
        {
            scl.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scl.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scl.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scl;
        }

        public static void DecomposeMatrix(Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale)
        {
            localPosition = ExtractTranslationFromMatrix(matrix);
            localRotation = ExtractRotationFromMatrix(matrix);
            localScale = ExtractScaleFromMatrix(matrix);
        }

        public static void SetTransformFromMatrix(Transform transform, Matrix4x4 matrix)
        {
            transform.localPosition = ExtractTranslationFromMatrix(matrix);
            transform.localRotation = ExtractRotationFromMatrix(matrix);
            transform.localScale = ExtractScaleFromMatrix(matrix);
        }

        public static Matrix4x4 TranslationMatrix(Vector3 offset)
        {
            Matrix4x4 matrix = identityMatrix;
            matrix.m03 = offset.x;
            matrix.m13 = offset.y;
            matrix.m23 = offset.z;
            return matrix;
        }

        public static Vector3[] TransposeMultSubtract(Vector3[] sourcePoints, Vector4[] targetPoints, Vector3 sourceCentroid, Vector3 targetCentroid, Vector3[] covariance)
        {

            for (int i = 0; i < 3; i++)
            { // i is the row in this matrix
                covariance[i] = Vector3.zero;
            }

            for (int k = 0; k < sourcePoints.Length; k++)
            {// k is the column in this matrix
                Vector3 left = (sourcePoints[k] - sourceCentroid) * targetPoints[k].w;
                Vector3 right = (new Vector3(targetPoints[k].x, targetPoints[k].y, targetPoints[k].z) - targetCentroid) * Mathf.Abs(targetPoints[k].w);

                covariance[0][0] += left[0] * right[0];
                covariance[1][0] += left[1] * right[0];
                covariance[2][0] += left[2] * right[0];
                covariance[0][1] += left[0] * right[1];
                covariance[1][1] += left[1] * right[1];
                covariance[2][1] += left[2] * right[1];
                covariance[0][2] += left[0] * right[2];
                covariance[1][2] += left[1] * right[2];
                covariance[2][2] += left[2] * right[2];
            }

            return covariance;
        }
    }
}