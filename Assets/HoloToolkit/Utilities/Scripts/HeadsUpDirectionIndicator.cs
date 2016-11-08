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
        // Use as a named indexer for Unity's frustum planes. The order follows that layed
        // out in the API documentation. DO NOT CHANGE ORDER unless a corresponding change
        // has been made in the Unity API.
        private enum FrustumPlanes
        {
            Left = 0,
            Right,
            Bottom,
            Top,
            Near,
            Far
        }

        [Tooltip("The object the direction indicator will point to.")]
        public GameObject TargetObject;

        [Tooltip("The camera depth at which the indicator rests.")]
        public float Depth;

        [Tooltip("The point around which the indicator pivots. Should be placed at the model's 'tip'.")]
        public Vector3 Pivot;

        [Tooltip("The object used to 'point' at the target.")]
        public GameObject PointerPrefab;

        [Tooltip("Determines what percentage of the visible field should be margin.")]
        [Range(0.0f, 1.0f)]
        public float IndicatorMarginPercent;

        [Tooltip("Debug draw the planes used to calculate the pointer lock location.")]
        public bool DebugDrawPointerOrientationPlanes;

        private GameObject pointer;

        private void Start()
        {
            Depth = Mathf.Clamp(Depth, Camera.main.nearClipPlane, Camera.main.farClipPlane);

            if (PointerPrefab == null)
            {
                this.gameObject.SetActive(false);
                return;
            }

            pointer = GameObject.Instantiate(PointerPrefab);

            // We create the effect of pivoting rotations by parenting the pointer and
            // offsetting its position.
            pointer.transform.parent = transform;
            pointer.transform.position = -Pivot;
        }

        // Update the direction indicator's position and orientation every frame.
        private void Update()
        {
            // No object to track?
            if (TargetObject == null || pointer == null)
            {
                // bail out early.
                return;
            }
            else
            {
                // The top, bottom and side frustum planes are used to restrict the movement
                // of the pointer.

                // Here we adjust the Camera's frustum planes to place the cursor in a smaller
                // volume, thus creating the effect of a "margin"
                Plane[] indicatorVolume = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                for (int i = 0; i < 4; ++i)
                {
                    // We can make the frustum smaller by rotating the walls "in" toward the
                    // camera's forward vector.

                    // First find the angle between the Camera's forward and the plane's normal
                    float angle = Mathf.Acos(Vector3.Dot(indicatorVolume[i].normal.normalized, Camera.main.transform.forward));

                    // Then we calculate how much we should rotate the plane in based on the
                    // user's setting. 90 degrees is our maximum as at that point we no longer
                    // have a valid frustum.
                    float angleStep = IndicatorMarginPercent * (0.5f * Mathf.PI - angle);

                    // Because the frustum plane normal's face in must actually rotate away from the forward to vector
                    // to narrow the frustum.
                    Vector3 normal = Vector3.RotateTowards(indicatorVolume[i].normal, Camera.main.transform.forward, -angleStep, 0.0f);
                    indicatorVolume[i].normal = normal.normalized;
                }

                UpdatePointerTransform(Camera.main, indicatorVolume, TargetObject.transform.position);
            }
        }

        // Assuming the target object is outside the view which of the four "wall" planes should
        // the pointer snap to.
        private FrustumPlanes GetExitPlane(Vector3 targetPosition, Camera camera)
        {
            // To do this we first create two planes that diagonally bisect the frustum
            // These panes create four quadrants. We then infer the exit plane based on
            // which quadrant the target position is in.

            // Calculate a set of vectors that can be used to build the frustum corners in world
            // space.
            float aspect = camera.aspect;
            float fovy = 0.5f * camera.fieldOfView;
            float near = camera.nearClipPlane;
            float far = camera.farClipPlane;

            float tanFovy = Mathf.Tan(Mathf.Deg2Rad * fovy);
            float tanFovx = aspect * tanFovy;

            // Calculate the edges of the frustum as world space offsets from the middle of the
            // frustum in world space.
            Vector3 nearTop = near * tanFovy * camera.transform.up;
            Vector3 nearRight = near * tanFovx * camera.transform.right;
            Vector3 nearBottom = -nearTop;
            Vector3 nearLeft = -nearRight;
            Vector3 farTop = far * tanFovy * camera.transform.up;
            Vector3 farRight = far * tanFovx * camera.transform.right;
            Vector3 farLeft = -farRight;

            // Caclulate the center point of the near plane and the far plane as offsets from the
            // camera in world space.
            Vector3 nearBase = near * camera.transform.forward;
            Vector3 farBase = far * camera.transform.forward;

            // Calculate the frustum corners needed to create 'd'
            Vector3 nearUpperLeft = nearBase + nearTop + nearLeft;
            Vector3 nearLowerRight = nearBase + nearBottom + nearRight;
            Vector3 farUpperLeft = farBase + farTop + farLeft;

            Plane d = new Plane(nearUpperLeft, nearLowerRight, farUpperLeft);

            // Calculate the frustum corners needed to create 'e'
            Vector3 nearUpperRight = nearBase + nearTop + nearRight;
            Vector3 nearLowerLeft = nearBase + nearBottom + nearLeft;
            Vector3 farUpperRight = farBase + farTop + farRight;

            Plane e = new Plane(nearUpperRight, nearLowerLeft, farUpperRight);

#if UNITY_EDITOR
            if (DebugDrawPointerOrientationPlanes)
            {
                // Debug draw a tringale coplanar with 'd'
                Debug.DrawLine(nearUpperLeft, nearLowerRight);
                Debug.DrawLine(nearLowerRight, farUpperLeft);
                Debug.DrawLine(farUpperLeft, nearUpperLeft);

                // Debug draw a triangle coplanar with 'e'
                Debug.DrawLine(nearUpperRight, nearLowerLeft);
                Debug.DrawLine(nearLowerLeft, farUpperRight);
                Debug.DrawLine(farUpperRight, nearUpperRight);
            }
#endif

            // We're not actually interested in the "distance" to the planes. But the sign
            // of the distance tells us which quadrant the target position is in.
            float dDistance = d.GetDistanceToPoint(targetPosition);
            float eDistance = e.GetDistanceToPoint(targetPosition);

            //     d              e
            //     +\-          +/-
            //       \  -d +e   /
            //        \        /
            //         \      /
            //          \    /
            //           \  /   
            //  +d +e     \/
            //            /\    -d -e
            //           /  \ 
            //          /    \
            //         /      \
            //        /        \
            //       /  +d -e   \
            //     +/-          +\-

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
 
        // given a frustum wall we wish to snap the pointer to, this function returns a ray
        // along which the pointer should be placed to appear at the appropiate point along
        // the edge of the indicator field.
        private bool TryGetIndicatorPosition(Vector3 targetPosition, Camera camera, Plane frustumWall, out Ray r)
        {
            // Think of the pointer as pointing the shortest rotation a user must make to see a
            // target. The shortest rotation can be obtained by finding the great circle defined
            // be the target, the camera position and the center position of the view. The tangent
            // vector of the great circle points the direction of the shortest rotation. This
            // great circle and thus any of it's tangent vectors are coplanar with the plane
            // defined by these same three points.
            Vector3 cameraToTarget = targetPosition - camera.transform.position;
            Vector3 normal = Vector3.Cross(cameraToTarget.normalized, camera.transform.forward);

            // In the case that the three points are colinear we cannot form a plane but we'll
            // assume the target is directly behind us and we'll use a prechosen plane.
            if (normal == Vector3.zero)
            {
                normal = -Vector3.right;
            }

            Plane q = new Plane(normal, targetPosition);
            return TryIntersectPlanes(frustumWall, q, out r);
        }

        // Obtain the line of intersection of two planes. This is based on a method
        // described in the GPU Gems series.
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

        // Modify the pointer location and orientation to point along the shortest rotation,
        // toward tergetPosition, keeping the pointer confined inside the frustum defined by
        // planes.
        private void UpdatePointerTransform(Camera camera, Plane[] planes, Vector3 targetPosition)
        {
            // Start by assuming the pointer should be placed at the target position.
            Vector3 indicatorPosition = camera.transform.position + Depth * (targetPosition - camera.transform.position).normalized;

            // Test the target position with the frustum planes except the "far" plane since
            // far away objects should be considered in view.
            bool pointNotInsideIndicatorField = false;
            for (int i = 0; i < 5; ++i)
            {
                float dot = Vector3.Dot(planes[i].normal, (targetPosition - camera.transform.position).normalized);
                if (dot <= 0.0f)
                {
                    pointNotInsideIndicatorField = true;
                    break;
                }
            }
            
            // if the target object appears outside the indicator area...
            if (pointNotInsideIndicatorField)
            {
                // ...then we need to do some geometry calculations to lock it to the edge.

                // used to determine which edge of the screen the indicator vector
                // would exit through.
                FrustumPlanes exitPlane = GetExitPlane(targetPosition, camera);

                Ray r;
                if (TryGetIndicatorPosition(targetPosition, camera, planes[(int)exitPlane], out r))
                {
                    indicatorPosition = camera.transform.position + Depth * r.direction.normalized;
                }
            }

            this.transform.position = indicatorPosition;

            // The pointer's direction should always appear pointing away from the user's center
            // of view. Thus we find the center point of the user's view in world space.

            // But the pointer should also appear perpendicular to the viewer so we find the
            // center position of the view that is on the same plane as the pointer position.
            // We do this by projecting the vector from the pointer to the camera onto the
            // the camera's forward vector.
            Vector3 indicatorFieldOffset = indicatorPosition - camera.transform.position;
            indicatorFieldOffset = Vector3.Dot(indicatorFieldOffset, camera.transform.forward) * camera.transform.forward;

            Vector3 indicatorFieldCenter = camera.transform.position + indicatorFieldOffset;
            Vector3 pointerDirection = (indicatorPosition - indicatorFieldCenter).normalized;

            // allign this object's up vector with the pointerDirection
            this.transform.rotation = Quaternion.LookRotation(camera.transform.forward, pointerDirection);
        }
    }
}
