// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using HoloToolkit.Unity;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Button = UnityEngine.UI.Button;

namespace HoloToolkit.Examples.SpatialUnderstandingFeatureOverview
{
    public class SpatialUnderstandingCursor : SpatialUnderstandingBasicCursor
    {
        // Consts
        public const float RayCastLength = 10.0f;

        // Config
        public TextMesh CursorText;
        public LayerMask UILayerMask;

        // Privates
        private SpatialUnderstandingDll.Imports.RaycastResult rayCastResult;

        // Functions
        protected override RaycastResult CalculateRayIntersect()
        {
            RaycastResult result = base.CalculateRayIntersect();

            // Now use the understanding code
            if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding &&
                SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
            {
                Vector3 rayPos = CameraCache.Main.transform.position;
                Vector3 rayVec = CameraCache.Main.transform.forward * RayCastLength;
                IntPtr raycastResultPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResultPtr();
                SpatialUnderstandingDll.Imports.PlayspaceRaycast(
                    rayPos.x, rayPos.y, rayPos.z,
                    rayVec.x, rayVec.y, rayVec.z,
                    raycastResultPtr);
                rayCastResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();

                float rayCastResultDist = Vector3.Distance(rayPos, rayCastResult.IntersectPoint);
                float resultDist = Vector3.Distance(rayPos, result.Position);

                // Override
                if (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Invalid &&
                    rayCastResultDist < resultDist)
                {
                    result.Hit = true;
                    result.Position = rayCastResult.IntersectPoint;
                    result.Normal = rayCastResult.IntersectNormal;

                    return result;
                }
            }

            return result;
        }

        public bool RayCastUI(out Vector3 hitPos, out Vector3 hitNormal, out Button hitButton)
        {
            // Defaults
            hitPos = Vector3.zero;
            hitNormal = Vector3.zero;
            hitButton = null;

            // Do the raycast
            RaycastHit hitInfo;
            Vector3 uiRayCastOrigin = CameraCache.Main.transform.position;
            Vector3 uiRayCastDirection = CameraCache.Main.transform.forward;
            if (Physics.Raycast(uiRayCastOrigin, uiRayCastDirection, out hitInfo, RayCastLength, UILayerMask))
            {
                Canvas canvas = hitInfo.collider.gameObject.GetComponent<Canvas>();
                if (canvas != null)
                {
                    GraphicRaycaster canvasRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
                    if (canvasRaycaster != null)
                    {
                        // Cast only against this canvas
                        PointerEventData pData = new PointerEventData(EventSystem.current);

                        pData.position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                        pData.delta = Vector2.zero;
                        pData.scrollDelta = Vector2.zero;

                        List<UnityEngine.EventSystems.RaycastResult> canvasHits = new List<UnityEngine.EventSystems.RaycastResult>();
                        canvasRaycaster.Raycast(pData, canvasHits);
                        for (int i = 0; i < canvasHits.Count; ++i)
                        {
                            Button button = canvasHits[i].gameObject.GetComponent<Button>();
                            if (button != null)
                            {
                                hitPos = uiRayCastOrigin + uiRayCastDirection * canvasHits[i].distance;
                                hitNormal = canvasHits[i].gameObject.transform.forward;
                                hitButton = button;
                                return true;
                            }
                        }

                        // No buttons, but hit canvas object
                        hitPos = hitInfo.point;
                        hitNormal = hitInfo.normal;
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void LateUpdate()
        {
            // Base
            base.LateUpdate();

            // Basic checks
            if ((SpatialUnderstanding.Instance == null) ||
               ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) &&
                (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Finishing) &&
                (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Done)))
            {
                CursorText.gameObject.SetActive(false);
                return;
            }
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Report the results
            if ((rayCastResult != null) &&
                (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Invalid))
            {
                CursorText.gameObject.SetActive(true);
                CursorText.text = rayCastResult.SurfaceType.ToString();

                CursorText.transform.rotation = Quaternion.LookRotation(CameraCache.Main.transform.forward, Vector3.up);
                CursorText.transform.position = transform.position + CameraCache.Main.transform.right * 0.05f;
            }
            else
            {
                CursorText.gameObject.SetActive(false);
            }

            // If we're looking at the UI, fade the text
            Vector3 hitPos, hitNormal;
            Button hitButton;
            float textAlpha = RayCastUI(out hitPos, out hitNormal, out hitButton) ? 0.15f : 1.0f;
            CursorText.color = new Color(1.0f, 1.0f, 1.0f, textAlpha);
        }
    }
}
