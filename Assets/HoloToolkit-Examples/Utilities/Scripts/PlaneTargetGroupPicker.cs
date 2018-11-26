// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity.Tests
{
    public class PlaneTargetGroupPicker : Singleton<PlaneTargetGroupPicker>
    {
        [Tooltip("In degrees")] public float AngleOfAcceptance = 45.0f;
        public PlaneTargetGroup[] Groups;

        public TextMesh DisplayText;
        public float TextDisplayTime = 5.0f;

        private PlaneTargetGroup currentGroup;

        private Coroutine displayForSecondsCoroutine;

        public void PickNewTarget()
        {
            PlaneTargetGroup newGroup = null;
            float smallestAngle = float.PositiveInfinity;
            Transform cameraTransform = CameraCache.Main.transform;
            // Figure out which group we're looking at
            foreach (PlaneTargetGroup group in Groups)
            {
                Vector3 camToGroup = group.transform.position - cameraTransform.position;
                float gazeObjectAngle = Vector3.Angle(camToGroup, cameraTransform.forward);
                if (group.Targets.Length > 0 && gazeObjectAngle < AngleOfAcceptance && gazeObjectAngle < smallestAngle)
                {
                    smallestAngle = gazeObjectAngle;
                    newGroup = group;
                }
            }

            // Looking at a group!
            if (newGroup != null)
            {
                // If we're already in this group, switch targets
                if (newGroup == currentGroup)
                {
                    newGroup.PickNewTarget();
                }
                currentGroup = newGroup;
                StabilizationPlaneModifier.Instance.TargetOverride = currentGroup.CurrentTarget.transform;
                StabilizationPlaneModifier.Instance.TrackVelocity = currentGroup.UseVelocity;
                UpdateText();
            }
        }

        private void UpdateText()
        {
            DisplayText.text = StabilizationPlaneModifier.Instance.TargetOverride.name;
            if (StabilizationPlaneModifier.Instance.TrackVelocity)
            {
                DisplayText.text += "\r\nvelocity";
            }

            if (displayForSecondsCoroutine != null)
            {
                StopCoroutine(displayForSecondsCoroutine);
            }
            displayForSecondsCoroutine = StartCoroutine(DisplayForSeconds(TextDisplayTime));
        }

        private IEnumerator DisplayForSeconds(float displayTime)
        {
            yield return new WaitForSeconds(displayTime);
            DisplayText.text = "";
        }
    }
}
