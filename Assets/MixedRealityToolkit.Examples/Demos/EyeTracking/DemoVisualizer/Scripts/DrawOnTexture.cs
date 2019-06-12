// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    public class DrawOnTexture : MonoBehaviour
    {
        public Texture2D HeatmapLookUpTable;

        [SerializeField]
        private float drawBrushSize = 2000.0f; // aka spread

        [SerializeField]
        private float drawIntensity = 15.0f; // aka amplitude

        [SerializeField]
        private float minThreshDeltaHeatMap = 0.001f; // Mostly for performance to reduce spreading heatmap for small values.

        public bool UseLiveInputStream = false;

        private Texture2D myDrawTex;
        private Renderer myRenderer;

        public Material HeatmapOverlayMaterialTemplate;

        private EyeTrackingTarget eyeTarget = null;

        private EyeTrackingTarget EyeTarget
        {
            get
            {
                if (eyeTarget == null)
                {
                    eyeTarget = this.GetComponent<EyeTrackingTarget>();
                }
                return eyeTarget;
            }
        }

        private void Start()
        {
            if (EyeTarget != null)
            {
                EyeTarget.WhileLookingAtTarget.AddListener(OnLookAt);
            }
        }

        public void OnLookAt()
        {
            if (UseLiveInputStream && (EyeTarget != null) && (EyeTarget.IsLookedAt))
            {
                DrawAtThisHitPos(EyeTrackingTarget.LookedAtPoint);
            }
        }

        public void DrawAtThisHitPos(Vector3 hitPosition)
        {
            Vector2? hitPosUV = GetCursorPosInTexture(hitPosition);
            if (hitPosUV != null)
            {
                StartCoroutine(DrawAt(hitPosUV.Value));
            }
        }

        private void DrawAt(Vector2 posUV, Color col)
        {
            if (MyDrawTexture != null)
            {
                Debug.LogFormat("New draw point at ( {0}; {1} )", posUV.x, posUV.y);
                MyDrawTexture.SetPixel(
                    (int)(posUV.x * MyDrawTexture.width), 
                    (int)(posUV.y * MyDrawTexture.height),
                    col);

                MyDrawTexture.Apply();
            }
        }


        bool neverDrawnOn = true;
        Vector2 prevPos = new Vector2(-1, -1);
        float dynamicRadius = 0;

        private void DrawAt2(Vector2 posUV, int maxRadius, float intensity)
        {
            if (MyDrawTexture != null)
            {
                // Reset on first draw
                if (neverDrawnOn)
                {
                    for (int ix = 0; ix < MyDrawTexture.width; ix++)
                    {
                        for (int iy = 0; iy < MyDrawTexture.height; iy++)
                        {
                            MyDrawTexture.SetPixel((int)(ix), (int)(iy), new Color(0, 0, 0, 0));   
                        }
                    }
                    neverDrawnOn = false;
                }

                // Determine the center of our to be drawn 'blob'
                Vector2 center = new Vector2(posUV.x * MyDrawTexture.width, posUV.y * MyDrawTexture.height);

                // Determine appropriate radius based on viewing duration: The longer looking in a small region the larger the radius
                float dist = maxRadius;
                for (int ix = -(int)(dynamicRadius / 2); ix < dynamicRadius / 2; ix++)
                {
                    for (int iy = -(int)(dynamicRadius / 2); iy < dynamicRadius / 2; iy++)
                    {
                        float tx = posUV.x * MyDrawTexture.width + ix;
                        float ty = posUV.y * MyDrawTexture.height + iy;

                        Vector2 currPnt = new Vector2(tx, ty);

                        float distCenterToCurrPnt = Vector2.Distance(center, currPnt);
                        
                        if (distCenterToCurrPnt <= dynamicRadius / 2)
                        {
                            float normalizedDist = (distCenterToCurrPnt / dynamicRadius / 2); // [0.0, 1.0]
                            float B = 4f;
                            float localNormalizedInterest = Mathf.Clamp(1 / (1 + Mathf.Pow(Mathf.Epsilon, -(B * normalizedDist))), 0, 1);

                            Color baseColor = MyDrawTexture.GetPixel((int)tx, (int)ty);
                            float delta = intensity * (1 - Mathf.Abs(localNormalizedInterest));
           
                            float normalizedInterest = baseColor.a + delta;
                            Color col = Color.red;
                            // Get color from  given heatmap ramp
                            if (HeatmapLookUpTable != null)
                            {
                                col = HeatmapLookUpTable.GetPixel((int)(normalizedInterest * HeatmapLookUpTable.width), 0);
                                col = new Color(col.r, col.g, col.b, normalizedInterest);
                            }
                            else
                                col = new Color(col.r, col.g, col.b, normalizedInterest);


                            MyDrawTexture.SetPixel((int)(tx), (int)(ty), col);

                            Color baseColor2 = MyDrawTexture.GetPixel((int)tx, (int)ty);
                        }
                    }
                }

                MyDrawTexture.Apply();
            }
        }

        private IEnumerator DrawAt(Vector2 posUV)
        {
            if (MyDrawTexture != null)
            {
                // Reset on first draw
                if (neverDrawnOn)
                {
                    for (int ix = 0; ix < MyDrawTexture.width; ix++)
                    {
                        for (int iy = 0; iy < MyDrawTexture.height; iy++)
                        {
                            MyDrawTexture.SetPixel((int)(ix), (int)(iy), new Color(0, 0, 0, 0));
                        }
                    }
                    neverDrawnOn = false;
                }

                // Assign colors
                yield return null;

                StartCoroutine(ComputeHeatmapAt(posUV, true, true));
                yield return null;

                StartCoroutine(ComputeHeatmapAt(posUV, true, false));
                yield return null;

                StartCoroutine(ComputeHeatmapAt(posUV, false, true));
                yield return null;

                StartCoroutine(ComputeHeatmapAt(posUV, false, false));
                yield return null;

                MyDrawTexture.Apply();
            }
        }

        private IEnumerator ComputeHeatmapAt(Vector2 currPosUV, bool positiveX, bool positiveY)
        {
            yield return null;

            // Determine the center of our to be drawn 'blob'
            Vector2 center = new Vector2(currPosUV.x * MyDrawTexture.width, currPosUV.y * MyDrawTexture.height);
            int sign_x = (positiveX) ? 1 : -1;
            int sign_y = (positiveY) ? 1 : -1;
            int start_x = (positiveX) ? 0 : 1;
            int start_y = (positiveY) ? 0 : 1;

            for (int dx = start_x; dx < MyDrawTexture.width; dx++)
            {
                float tx = currPosUV.x * MyDrawTexture.width + dx * sign_x;
                if ((tx < 0) || (tx >= MyDrawTexture.width))
                    break;

                for (int dy = start_y; dy < MyDrawTexture.height; dy++)
                {
                    float ty = currPosUV.y * MyDrawTexture.height + dy * sign_y;
                    if ((ty < 0) || (ty >= MyDrawTexture.height))
                        break;

                    Color? newColor = null;
                    if (ComputeHeatmapColorAt(new Vector2(tx, ty), center, out newColor))
                    {
                        if (newColor.HasValue)
                        {
                            MyDrawTexture.SetPixel((int)(tx), (int)(ty), newColor.Value);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private bool ComputeHeatmapColorAt(Vector2 currPnt, Vector2 origPivot, out Color? col)
        {
            col = null;

            
            float spread = drawBrushSize;
            float amplitude = drawIntensity;
            float distCenterToCurrPnt = Vector2.Distance(origPivot, currPnt) / spread;

            float B = 2f;
            float scaledInterest = 1 / (1 + Mathf.Pow(Mathf.Epsilon, -(B * distCenterToCurrPnt)));
            float delta = scaledInterest / amplitude ;
            if (delta < minThreshDeltaHeatMap)
                return false;

            Color baseColor = MyDrawTexture.GetPixel((int)currPnt.x, (int)currPnt.y);
            float normalizedInterest = Mathf.Clamp(baseColor.a + delta, 0, 1);
            
            // Get color from given heatmap ramp
            if (HeatmapLookUpTable != null)
            {
                col = HeatmapLookUpTable.GetPixel((int)(normalizedInterest * (HeatmapLookUpTable.width-1)), 0);
                col = new Color(col.Value.r, col.Value.g, col.Value.b, normalizedInterest);
            }
            else
            {
                col = Color.blue;
                col = new Color(col.Value.r, col.Value.g, col.Value.b, normalizedInterest);
            }

            return true;
        }

        private Renderer MyRenderer
        {
            get
            {
                if (myRenderer == null)
                {
                    myRenderer = GetComponent<Renderer>();
                }
                return myRenderer;
            }
        }

        private Texture2D MyDrawTexture
        {
            get
            {
                if (myDrawTex == null)
                {
                    if (MyRenderer == null)
                    {
                        return null;
                    }

                    Material[] mats = MyRenderer.sharedMaterials;

                    Material[] mats2 = new Material[mats.Length + 1];
                    for (int i = 0; i < mats.Length; i++)
                    {
                        mats2[i] = mats[i];
                    }
                    mats2[mats.Length] = Instantiate(HeatmapOverlayMaterialTemplate) as Material;
                    MyRenderer.sharedMaterials = mats2;

                    // We don't want to overwrite the original texture. 
                    // By creating an instance, we're not changing the texture itself.
                    // Thus, when restarting the app, everything is set back to how it was.
                    if (mats2 == null)
                    {
                        Debug.Log("mats2 -> null");
                    }
                    else if (mats2[mats.Length] == null)
                    {
                        Debug.Log("mats2.length -> null");
                    }
                    else if (mats2[mats.Length].mainTexture == null)
                    {
                        Debug.Log("mats2.maintex -> null");
                    }
                    else
                    {
                        myDrawTex = Instantiate(mats2[mats.Length].mainTexture) as Texture2D;
                        mats2[mats.Length].mainTexture = myDrawTex;
                    }
                }
                return myDrawTex;
            }
        }

        /// <summary>
        /// Determine the position of the cursor within the texture in UV space.
        /// </summary>
        /// <returns>True if this GameObject is hit.</returns>
        private Vector2? GetCursorPosInTexture(Vector3 hitPosition)
        {
            Vector2? hitPointUV = null;

            try
            {
                Vector3 center = gameObject.transform.position;
                Vector3 halfsize = gameObject.transform.localScale / 2;

                // Let's transform back to the origin: Translate & Rotate
                Vector3 transfHitPnt = hitPosition - center;

                // Rotate around the y axis
                transfHitPnt = Quaternion.AngleAxis(-(this.gameObject.transform.rotation.eulerAngles.y - 180), Vector3.up) * transfHitPnt;

                // Rotate around the x axis
                transfHitPnt = Quaternion.AngleAxis(this.gameObject.transform.rotation.eulerAngles.x, Vector3.right) * transfHitPnt;

                // Normalize the transformed hit point to as UV coordinates are in [0,1].
                float uvx = (Mathf.Clamp(transfHitPnt.x, -halfsize.x, halfsize.x) + halfsize.x) / (2 * halfsize.x);
                float uvy = (Mathf.Clamp(transfHitPnt.y, -halfsize.y, halfsize.y) + halfsize.y) / (2 * halfsize.y);
                hitPointUV = new Vector2(uvx, uvy);
            }
            catch (UnityEngine.Assertions.AssertionException)
            {
                Debug.LogError(">> AssertionException");
            }

            return hitPointUV;
        }
    }
}
