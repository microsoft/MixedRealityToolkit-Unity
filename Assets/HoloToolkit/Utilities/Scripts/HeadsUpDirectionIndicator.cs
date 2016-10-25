using UnityEngine;

namespace HoloToolKit.Unity
{
    // The easiest way to use this script is to drop in the HeadsUpDirectionIndicator prefab
    // from the HoloToolKit. If you're having issues with the prefab or can't find it,
    // you can simply create an empty GameObject and attach this script. You'll need to
    // create your own pointer object which can by any 3D game object. You'll need to adjust
    // the depth, margin and pivot variables to affect the right appearance. After that you
    // simply need to specify the "targetObject" and then you should be set.
    // 
    // This script assumes your point object "aims" along its local up axis and orients the
    // object according to that assumption.
    public class HeadsUpDirectionIndicator : MonoBehaviour
    {
        // an implementation of a rect class that holds it coordinates as integers.
        private struct Rect
        {
            public Rect(int left, int right, int top, int bottom)
            {
                this.left = left;
                this.right = right;
                this.top = top;
                this.bottom = bottom;
            }

            public Rect(float left, float right, float top, float bottom)
            {
                this.left = (int)left;
                this.right = (int)right;
                this.top = (int)top;
                this.bottom = (int)bottom;
            }

            public int left, right, top, bottom;

            public int Width
            {
                get
                {
                    return right - left;
                }
            }

            public int Height
            {
                get
                {
                    return top - bottom;
                }
            }

            public override string ToString()
            {
                return string.Format(@"Left {0}\n Right {1}\n Top{2}\n Bottom{3}", left, right, top, bottom);
            }
        }

        [Tooltip("The object the direction indicator will point to.")]
        public GameObject targetObject;

        [Tooltip("The camera depth at which the indicator rests.")]
        public float depth;

        [Tooltip("The point around which the indicator pivots. Should be placed at the model's 'tip'.")]
        public Vector3 pivot;

        [Tooltip("The object used to 'point' at the target.")]
        public GameObject pointerPrefab;

        [Tooltip("Determines what percentage of the visible field should be margin.")]
        [Range(0.0f, 100.0f)]
        public float indicatorMarginPercent;

        private GameObject pointer;

        private void Start()
        {
            depth = Mathf.Clamp(depth, Camera.main.nearClipPlane, Camera.main.farClipPlane);

            pointer = GameObject.Instantiate(pointerPrefab);

            pointer.transform.parent = transform;
            pointer.transform.position = -pivot;
        }

        // Update the direction indicator's position and orientation every frame.
        private void Update()
        {
            // No object to track?
            if (targetObject == null)
            {
                // bail out early.
                return;
            }
            else
            {
                float marginFactor = indicatorMarginPercent / 100.0f;

                Rect indicatorArea = new Rect(
                    0.5f * marginFactor * Screen.width,
                    (1.0f - 0.5f * marginFactor) * Screen.width,
                    (1.0f - 0.5f * marginFactor) * Screen.height,
                    0.5f * marginFactor * Screen.height);

                Rect screenArea = new Rect(
                    0,
                    Screen.width,
                    Screen.height,
                    0);

                UpdatePointerTransform(Camera.main, targetObject.transform.position, indicatorArea, screenArea);
            }
        }

        private void UpdatePointerTransform(Camera camera, Vector3 targetPosition, Rect indicatorArea, Rect screenArea)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(targetPosition);

            bool pointNotInsideIndicatorField = screenPoint.x < indicatorArea.left
                || screenPoint.y < indicatorArea.bottom
                || screenPoint.x > indicatorArea.right
                || screenPoint.y > indicatorArea.top;

            // negative z means the point is behind the camera and the coordinates flip.
            if (screenPoint.z < 0.0f)
            {
                screenPoint = -screenPoint;
                pointNotInsideIndicatorField = true;
            }

            // transform the point into Cartesian space because it makes the position calculations easier.
            screenPoint.x = screenPoint.x - screenArea.Width / 2;
            screenPoint.y = screenPoint.y - screenArea.Height / 2;

            // if the target object appears outside the indicator area...
            if (pointNotInsideIndicatorField)
            {
                // ...then we need to do some geometry calculations to lock it to the edge.
                if (screenPoint.x == 0.0f && screenPoint.y == 0.0f)
                {
                    Debug.Log("Zero");
                }

                // used to determine which edge of the screen the indicator vector
                // would exit through.
                float hRatio = screenPoint.x / (float)indicatorArea.Width;
                float vRatio = screenPoint.y / (float)indicatorArea.Height;

                if (Mathf.Abs(hRatio) > Mathf.Abs(vRatio))
                {
                    // in this case, the vector collides with the sides first.
                    float slope = screenPoint.y / screenPoint.x;
                    if (screenPoint.x > 0.0f)
                    {
                        screenPoint.x = indicatorArea.Width / 2;
                    }
                    else
                    {
                        screenPoint.x = -indicatorArea.Width / 2;
                    }

                    screenPoint.y = slope * screenPoint.x;
                }
                else
                {
                    // in this case, the vector collides with the top or bottom first.
                    float invSlope = screenPoint.x / screenPoint.y;
                    if (screenPoint.y > 0.0f)
                    {
                        screenPoint.y = indicatorArea.Height / 2;
                    }
                    else
                    {
                        screenPoint.y = -indicatorArea.Height / 2;
                    }

                    screenPoint.x = invSlope * screenPoint.y;
                }
            }

            // store the direction in Cartesian space so we can use it as a local space transformation
            Vector3 pointerDirection = screenPoint.normalized;
            pointerDirection.z = 0.0f;

            // transform the point back to screen space to get the indicator on-screen position.
            screenPoint.x = screenPoint.x + screenArea.Width / 2;
            screenPoint.y = screenPoint.y + screenArea.Height / 2;
            screenPoint.z = depth; // lock the z to the depth specified by the user

            // determine the world space position of the pointer
            Vector3 pointInView = camera.ScreenToWorldPoint(screenPoint);
            this.transform.position = pointInView;

            // allign this object's up vector with the pointerDirection and then apply the same rotation as the camera so the pointer faces the viewer
            this.transform.rotation = camera.transform.rotation * Quaternion.FromToRotation(Vector3.up, pointerDirection);
        }
    }
}