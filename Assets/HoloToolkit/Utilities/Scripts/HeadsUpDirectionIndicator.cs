using UnityEngine;
using System.Collections;

public class HeadsUpDirectionIndicator : MonoBehaviour {
    public GameObject targetObject;

	// Use this for initialization
	void Start () {
	}

    void UpdateAsPlane()
    {
        var targetPosition = targetObject.GetComponent<Transform>().position;
        var vecToTarget = targetPosition - Camera.main.transform.position;
        var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        float hit = 0.0f;
        if (planes[4].Raycast(new Ray(Camera.main.transform.position, vecToTarget), out hit))
        {
            var hitPosition = Camera.main.transform.position + hit * vecToTarget;
            var screenPoint = Camera.main.WorldToScreenPoint(hitPosition);

            var screenPointInView = screenPoint;
            screenPointInView.x = Mathf.Clamp(screenPoint.x, 0, Screen.width);
            screenPointInView.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);

            screenPoint.x = screenPoint.x - Screen.width / 2;
            screenPoint.y = screenPoint.y - Screen.height / 2;

            screenPointInView.x = screenPointInView.x - Screen.width / 2;
            screenPointInView.y = screenPointInView.y - Screen.height / 2;

            var adjustDirection = screenPoint - screenPointInView;
            //if (adjustDirection == Vector3.zero)
            {
                adjustDirection = screenPointInView - Vector3.zero;
                adjustDirection.z = 0.0f;
            }

            this.GetComponent<RectTransform>().anchoredPosition = screenPointInView;
            this.GetComponent<RectTransform>().up = adjustDirection.normalized;
        }
    }

    Vector3 ProjectPointToPlane(Vector3 planeNormal, Vector3 pointOnPlane, Vector3 v) {
        var vRelative = v - pointOnPlane;
        float normalBias = Vector3.Dot(planeNormal, vRelative);
        var vProjected = vRelative - normalBias * planeNormal;
        var vDisplaced = pointOnPlane + vProjected;
        return vDisplaced;
    }

    bool PointInFrustum(Vector3 point)
    {
        var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        // Don't test against the far plane, which is the last in the array.
        for (int i = 0; i < 5; ++i) {
            var plane = frustumPlanes[i];
            if (!plane.GetSide(point)) {
                return false;
            }
        }

        return true;
    }

    Vector3 GetIndicatorDispositionNotInView() {
        var targetPosition = targetObject.GetComponent<Transform>().position;
        var screenPoint = Camera.main.WorldToScreenPoint(targetPosition);

        // negative z means the point is behind the camera and the coordinates flip.
        if (screenPoint.z < 0.0f) {
            screenPoint = -screenPoint;
        }

        var screenPointInView = screenPoint;
        screenPointInView.x = screenPointInView.x - Screen.width / 2;
        screenPointInView.y = screenPointInView.y - Screen.height / 2;

        float hRatio = screenPointInView.x / (float)Screen.width;
        float vRatio = screenPointInView.y / (float)Screen.height;

        if (Mathf.Abs(hRatio) > Mathf.Abs(vRatio)) {
            // in this case, the vector collides with the sides first.
            float slope = screenPointInView.y / screenPointInView.x;
            if (screenPointInView.x > 0.0) {
                screenPointInView.x = Screen.width / 2;
            }
            else {
                screenPointInView.x = -Screen.width / 2;
            }

            screenPointInView.y = slope * screenPointInView.x;
        }
        else {
            // in this case, the vector collides with the top or bottom first;
            float invSlope = screenPointInView.x / screenPointInView.y;
            if (screenPointInView.y > 0.0f) {
                screenPointInView.y = Screen.height / 2;
            }
            else {
                screenPointInView.y = -Screen.height / 2;
            }

            screenPointInView.x = invSlope * screenPointInView.y;
        }

        return screenPointInView;
    }

    Vector3 GetIndicatorDispositionInView() {
        var targetPosition = targetObject.GetComponent<Transform>().position;
        var screenPoint = Camera.main.WorldToScreenPoint(targetPosition);

        var screenPointInView = screenPoint;
        screenPointInView.x = screenPointInView.x - Screen.width / 2;
        screenPointInView.y = screenPointInView.y - Screen.height / 2;

        var adjustDirection = screenPointInView;
        adjustDirection.z = 0.0f;

        return screenPointInView;
    }

	// Update is called once per frame
	void Update () {
        Vector3 position = Vector3.zero;

        var targetPosition = targetObject.GetComponent<Transform>().position;

        if (this.PointInFrustum(targetPosition)) {
            position = GetIndicatorDispositionInView();
        }
        else {
            position = GetIndicatorDispositionNotInView();
        }

        var alignment = position;
        alignment.z = 0.0f;

        this.GetComponent<RectTransform>().anchoredPosition = position;
        this.GetComponent<RectTransform>().up = alignment.normalized;
	}
}

