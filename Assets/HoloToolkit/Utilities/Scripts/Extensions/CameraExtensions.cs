using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Get the horizontal FOV from the stereo camera
        /// </summary>
        /// <returns></returns>
        public static float GetHorizontalFieldOfViewRadians(this Camera camera)
        {
            float horizontalFovRadians = 2 * Mathf.Atan(Mathf.Tan((camera.fieldOfView * Mathf.Deg2Rad) / 2) * camera.aspect);
            return horizontalFovRadians;
        }

        /// <summary>
        /// Returns if a point will be rendered on the screen in either eye
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool IsInFOV(this Camera camera, Vector3 position)
        {
            float verticalFovHalf = camera.fieldOfView / 2;
            float horizontalFovHalf = camera.GetHorizontalFieldOfViewRadians() * Mathf.Rad2Deg / 2;

            Vector3 deltaPos = position - camera.transform.position;
            Vector3 headDeltaPos = MathUtils.TransformDirectionFromTo(null, camera.transform, deltaPos).normalized;

            float yaw = Mathf.Asin(headDeltaPos.x) * Mathf.Rad2Deg;
            float pitch = Mathf.Asin(headDeltaPos.y) * Mathf.Rad2Deg;

            return (Mathf.Abs(yaw) < horizontalFovHalf && Mathf.Abs(pitch) < verticalFovHalf);
        }
    }
}