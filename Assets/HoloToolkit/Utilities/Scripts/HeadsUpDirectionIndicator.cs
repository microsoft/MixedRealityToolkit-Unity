using UnityEngine;

namespace HoloToolkit.Unity
{
    // The easiest way to use this script is to drop in the HeadsUpDirectionIndicator prefab
    // from the HoloToolKit. If you're having issues with the prefab or can't find it,
    // you can simply create a UI->Image object and attach this script on the Image child
    // of the Canvas. In that case you'll need to pick your own sprite. You'll also need to
    // choose an appropriate "pivot" point on the RectTransform. After that you simply
    // need to specify the "targetObject" and then you should be set.
    // 
    // This script takes for granted that your UI is rendering in Camera space and that the
    // up vector is aligned with the "pointing" direction of your image.
    [RequireComponent(typeof(RectTransform))]
    public class HeadsUpDirectionIndicator : MonoBehaviour
    {
        [Tooltip("The object the direction indicator will point to.")]
        [SerializeField]
        private GameObject targetObject;

        [Tooltip("Determines what percentage of the visible field should be margin.")]
        [Range(0.0f, 100.0f)]
        [SerializeField]
        private float indicatorMarginPercent;

        private RectTransform rectTransform;

        private const string MissingRectTransformErrorMessage = "The use of the HeadsUpDirectionIndicator component requires a RectTransform attached to the same game object.";

        private void Start()
        {
            this.rectTransform = this.GetComponent<RectTransform>();
            
            if (this.rectTransform == null)
            {
                this.gameObject.SetActive(false);
                Debug.LogError(MissingRectTransformErrorMessage);
            }
        }

        // Update the direction indicator's position and orientation every frame.
        private void Update()
        {
            if (targetObject == null)
            {
                // no target, bail out
                return;
            }

            float marginFactor = (100.0f - indicatorMarginPercent) / 100.0f;
            int indicatorFieldWidth = (int)(marginFactor * (float)Screen.width);
            int indicatorFieldHeight = (int)(marginFactor * (float)Screen.height);

            Vector3 targetPosition = targetObject.transform.position;
            Vector3 position = GetIndicatorPositionInView(targetPosition, Camera.main, indicatorFieldWidth, indicatorFieldHeight);

            Vector3 alignment = position;
            alignment.z = 0.0f;

            // With camera space UI we can use the canvas space position
            this.rectTransform.anchoredPosition = position;

            // but the orientation is in world space. So here we first apply the screen space
            // rotation and then rotate it into alignment with the camera.
            this.rectTransform.rotation = Camera.main.transform.rotation * Quaternion.FromToRotation(Vector3.up, alignment.normalized);
        }

        // Provides the GUI space position of the indicator
        private Vector3 GetIndicatorPositionInView(Vector3 targetPosition, Camera camera, int indicatorAreaWidth, int indicatorAreaHeight)
        {
            Vector3 screenPoint = camera.WorldToScreenPoint(targetPosition);

            // screen space maps 0,0 to the lower left corner but canvase coordinates
            // map 0,0 to the center of the view.
            screenPoint.x = screenPoint.x - Screen.width / 2;
            screenPoint.y = screenPoint.y - Screen.height / 2;

            bool pointNotInsideIndicatorField = screenPoint.x < -indicatorAreaWidth / 2
                || screenPoint.y < -indicatorAreaHeight / 2
                || screenPoint.x > indicatorAreaWidth / 2
                || screenPoint.y > indicatorAreaHeight / 2;

            // negative z means the point is behind the camera and the coordinates flip.
            if (screenPoint.z < 0.0f)
            {
                screenPoint = -screenPoint;
                pointNotInsideIndicatorField = true;
            }

            // If the point is outside the display region...
            if (pointNotInsideIndicatorField)
            {
                // ...then we need to do some geometry calculations to lock it to the edge.

                // used to determine which edge of the screen the indicator vector
                // would exit through.
                float hRatio = screenPoint.x / (float)indicatorAreaWidth;
                float vRatio = screenPoint.y / (float)indicatorAreaHeight;

                if (Mathf.Abs(hRatio) > Mathf.Abs(vRatio))
                {
                    // in this case, the vector collides with the sides first.
                    float slope = screenPoint.y / screenPoint.x;
                    if (screenPoint.x > 0.0f)
                    {
                        screenPoint.x = indicatorAreaWidth / 2;
                    }
                    else
                    {
                        screenPoint.x = -indicatorAreaWidth / 2;
                    }

                    screenPoint.y = slope * screenPoint.x;
                }
                else
                {
                    // in this case, the vector collides with the top or bottom first.
                    float invSlope = screenPoint.x / screenPoint.y;
                    if (screenPoint.y > 0.0f)
                    {
                        screenPoint.y = indicatorAreaHeight / 2;
                    }
                    else
                    {
                        screenPoint.y = -indicatorAreaHeight / 2;
                    }

                    screenPoint.x = invSlope * screenPoint.y;
                }

                return screenPoint;
            }
            else
            {
                // the point is inside the indicator display region and we should use the point unaltered.
                return screenPoint;
            }
        }
    }
}
