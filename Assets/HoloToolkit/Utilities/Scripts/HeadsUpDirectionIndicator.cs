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
        enum FrustumPlanes
        {
            Left = 0,
            Right,
            Bottom,
            Top,
            Near,
            Far
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

                var indicatorVolume = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                for (int i = 0; i < 4; ++i)
                {
                    var angle = Mathf.Acos(Vector3.Dot(indicatorVolume[i].normal.normalized, Camera.main.transform.forward));
                    float angleStep = marginFactor * (0.5f * Mathf.PI - angle);
                    var normal = Vector3.RotateTowards(indicatorVolume[i].normal, Camera.main.transform.forward, -angleStep, 0.0f);
                    indicatorVolume[i] = new Plane(normal.normalized, indicatorVolume[i].distance);
                }

                UpdatePointerTransform(Camera.main, indicatorVolume, targetObject.transform.position);
            }
        }

        private FrustumPlanes GetExitPlane(Vector3 targetPosition, Camera camera)
        {
            var aspect = camera.aspect;
            var fovy = 0.5f * camera.fieldOfView;
            var near = camera.nearClipPlane;
            var far = camera.farClipPlane;

            float tanFovy = Mathf.Tan(Mathf.Deg2Rad * fovy);
            float tanFovx = aspect * tanFovy;

            Vector3 nearTop = near * tanFovy * camera.transform.up;
            Vector3 nearRight = near * tanFovx * camera.transform.right;
            Vector3 nearBottom = -nearTop;
            Vector3 nearLeft = -nearRight;

            Vector3 farTop = far * tanFovy * camera.transform.up;
            Vector3 farRight = far * tanFovx * camera.transform.right;
            Vector3 farLeft = -farRight;
            Vector3 nearBase = near * camera.transform.forward;
            Vector3 farBase = far * camera.transform.forward;

            Vector3 nearUpperLeft = nearBase + nearTop + nearLeft;
            Vector3 nearLowerRight = nearBase + nearBottom + nearRight;
            Vector3 farUpperLeft = farBase + farTop + farLeft;

            Debug.DrawLine(nearUpperLeft, nearLowerRight);
            Debug.DrawLine(nearLowerRight, farUpperLeft);
            Debug.DrawLine(farUpperLeft, nearUpperLeft);

            Plane d = new Plane(nearUpperLeft, nearLowerRight, farUpperLeft);

            Vector3 nearUpperRight = nearBase + nearTop + nearRight;
            Vector3 nearLowerLeft = nearBase + nearBottom + nearLeft;
            Vector3 farUpperRight = farBase + farTop + farRight;

            Debug.DrawLine(nearUpperRight, nearLowerLeft);
            Debug.DrawLine(nearLowerLeft, farUpperRight);
            Debug.DrawLine(farUpperRight, nearUpperRight);

            Plane e = new Plane(nearUpperRight, nearLowerLeft, farUpperRight);

            float dDistance = d.GetDistanceToPoint(targetPosition);
            float eDistance = e.GetDistanceToPoint(targetPosition);

            if (dDistance > 0.0f)
            {
                if (eDistance > 0.0f)
                {
                    return FrustumPlanes.Left;
                }
                else
                {
                    return FrustumPlanes.Bottom;
                }
            }
            else
            {
                if (eDistance > 0.0f)
                {
                    return FrustumPlanes.Top;
                }
                else
                {
                    return FrustumPlanes.Right;
                }
            }
        }

        private bool TryGetIndicatorPosition(Vector3 targetPosition, Camera camera, Plane frustumWall, out Ray r)
        {
            Vector3 cameraToTarget = targetPosition - camera.transform.position;
            Vector3 normal = Vector3.Cross(cameraToTarget.normalized, camera.transform.forward);

            if (normal == Vector3.zero)
            {
                normal = -Vector3.right;
            }

            Plane q = new Plane(normal, targetPosition);
            return TryIntersectPlanes(frustumWall, q, out r);
        }

        private bool TryIntersectPlanes(Plane p, Plane q, out Ray intersection)
        {
            Vector3 rNormal = Vector3.Cross(p.normal, q.normal);
            float det = rNormal.sqrMagnitude;

            if (det != 0.0f)
            {
                Vector3 rPoint = ((Vector3.Cross(rNormal, q.normal) * p.distance) +
                    (Vector3.Cross(p.normal, rNormal) * q.distance)) / det;
                intersection = new Ray(rPoint, rNormal);
                return true;
            }
            else
            {
                intersection = new Ray();
                return false;
            }
        }

        private void UpdatePointerTransform(Camera camera, Plane []planes, Vector3 targetPosition)
        {
            Vector3 indicatorPosition = camera.transform.position + depth * (targetPosition - camera.transform.position).normalized;

            bool pointNotInsideIndicatorField = false;
            for (int i = 0; i < 5; ++i)
            {
                var dot = Vector3.Dot(planes[i].normal, (targetPosition - camera.transform.position).normalized);
                if (dot <= 0.0f)
                {
                    pointNotInsideIndicatorField = true;
                }
            }
            
            // if the target object appears outside the indicator area...
            if (pointNotInsideIndicatorField)
            {
                // ...then we need to do some geometry calculations to lock it to the edge.

                // used to determine which edge of the screen the indicator vector
                // would exit through.
                var exitPlane = GetExitPlane(targetPosition, camera);
                Debug.Log(exitPlane);

                Ray r;
                if (TryGetIndicatorPosition(targetPosition, camera, planes[(int)exitPlane], out r))
                {
                    indicatorPosition = camera.transform.position + depth * r.direction.normalized;
                }
            }

            this.transform.position = indicatorPosition;

            Vector3 proj = indicatorPosition - camera.transform.position;
            proj = Vector3.Dot(proj, camera.transform.forward) * camera.transform.forward;
            Vector3 indicatorFieldCenter = camera.transform.position + proj;
            Vector3 pointerDirection = (indicatorPosition - indicatorFieldCenter).normalized;

            // allign this object's up vector with the pointerDirection
            this.transform.rotation = Quaternion.LookRotation(camera.transform.forward, pointerDirection);
        }
    }
}
