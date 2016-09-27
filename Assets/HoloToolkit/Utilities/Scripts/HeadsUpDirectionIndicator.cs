using UnityEngine;

namespace HoloToolkit.Unity
{
    public class HeadsUpDirectionIndicator : MonoBehaviour
    {
        public GameObject targetObject;

        void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (gameObject == null)
            {
                // no target
                return;
            }

            Vector3 position = Vector3.zero;

            var targetPosition = targetObject.GetComponent<Transform>().position;

            if (this.PointInInfiniteFrustum(targetPosition))
            {
                position = GetIndicatorPositionInView(targetPosition, Camera.main, Screen.width, Screen.height);
            }
            else
            {
                position = GetIndicatorPositionNotInView(targetPosition, Camera.main, Screen.width, Screen.height);
            }

            var alignment = position;
            alignment.z = 0.0f;

            this.GetComponent<RectTransform>().anchoredPosition = position;
            this.GetComponent<RectTransform>().up = alignment.normalized;
        }

        // Provides the GUI space position of the indicator assuming it should be point off-screen.
        private Vector3 GetIndicatorPositionNotInView(Vector3 targetPosition, Camera camera, int viewportWidth, int viewportHeight)
        {
            var screenPoint = camera.WorldToScreenPoint(targetPosition);

            // negative z means the point is behind the camera and the coordinates flip.
            if (screenPoint.z < 0.0f)
            {
                screenPoint = -screenPoint;
            }

            // screen space maps 0,0 to the lower left corner but canvase coordinates
            // map 0,0 to the center of the view.
            screenPoint.x = screenPoint.x - viewportWidth / 2;
            screenPoint.y = screenPoint.y - viewportHeight / 2;

            // used to determine which edge of the screen the indicator vector
            // would exit through.
            float hRatio = screenPoint.x / (float)viewportWidth;
            float vRatio = screenPoint.y / (float)viewportHeight;

            if (Mathf.Abs(hRatio) > Mathf.Abs(vRatio))
            {
                // in this case, the vector collides with the sides first.
                float slope = screenPoint.y / screenPoint.x;
                if (screenPoint.x > 0.0)
                {
                    screenPoint.x = viewportWidth / 2;
                }
                else
                {
                    screenPoint.x = -viewportWidth / 2;
                }

                screenPoint.y = slope * screenPoint.x;
            }
            else
            {
                // in this case, the vector collides with the top or bottom first;
                float invSlope = screenPoint.x / screenPoint.y;
                if (screenPoint.y > 0.0f)
                {
                    screenPoint.y = viewportHeight / 2;
                }
                else
                {
                    screenPoint.y = -viewportHeight / 2;
                }

                screenPoint.x = invSlope * screenPoint.y;
            }

            return screenPoint;
        }

        // Provides the GUI space position of the indicator assuming it should point to a position on-screen.
        private Vector3 GetIndicatorPositionInView(Vector3 targetPosition, Camera camera, int viewportWidth, int viewportHeight)
        {
            var screenPoint = camera.WorldToScreenPoint(targetPosition);

            // screen space maps 0,0 to the lower left corner but canvase coordinates
            // map 0,0 to the center of the view.
            screenPoint.x = screenPoint.x - viewportWidth / 2;
            screenPoint.y = screenPoint.y - viewportHeight / 2;

            return screenPoint;
        }

        private bool PointInInfiniteFrustum(Vector3 point)
        {
            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

            // Don't test against the far plane, which is the last in the array.
            for (int i = 0; i < 5; ++i)
            {
                var plane = frustumPlanes[i];
                if (!plane.GetSide(point))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
