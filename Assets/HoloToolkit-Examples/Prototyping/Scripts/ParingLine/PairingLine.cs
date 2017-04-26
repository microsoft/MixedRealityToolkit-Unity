// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.Events;

public struct ParingTrail
{
    public GameObject Trail;
    public float Time;
}

[RequireComponent(typeof(LineRenderer))]
public class PairingLine : MonoBehaviour
{

    public enum PairingLineType { Default, Trails, Dashed }

    public GameObject StartObject;
    public Vector3? StartPosition;
    public GameObject EndObject;

    [FormerlySerializedAs("color")]
    public Color Color;

    [FormerlySerializedAs("width")]
    public float Width;

    public PairingLineType LineType;

    public bool DestroyLineWhenObjectsDestroyed = true;

    [System.Serializable]
    public class LineDestroyEvent : UnityEvent<PairingLine> { };
    public LineDestroyEvent OnDestroyEvent;

    private LineRenderer mLineRenderer;
    private PairingLineType mOldLineType;

    public bool FadeAndDestory = false;
    public float FadeoutTime = 3f;

    private float t = 0;

    public bool FlashColor = true;
    private float flashT = 0f;
    private float flashTime = 0.5f;
    public bool LoopFlashing = false;

    public Texture DefaultTexture;
    public Texture TrailsTexture;
    public Texture DashedTexture;

    public GameObject LineTrailPrefab;
    private GameObject mLineInstance;
    float mTrailSpeed = 0.06f;
    float mTrailSpeedDistance = 0.1f;
    float mTrailTime = 0.1f;
    float mSpawnCounter = 0;
    Color mTransparentColor = new Color(0, 0, 0, 0);
    List<ParingTrail> mTrailsList;
    float mTrailTimeFactor = 0.8f;
    float mSpawnTimeFactor = 6f;
    float mDashedOffset = 0.5f;

    // Use this for initialization
    void Start()
    {
        mTrailsList = new List<ParingTrail>();
        mLineRenderer = this.GetComponent<LineRenderer>();

        UpdateLineType(LineType);

        // Disable till first update
        mLineRenderer.enabled = false;

        flashTime = flashTime * 0.5f;
        FadeoutTime = FadeoutTime * 0.65f;

        mOldLineType = LineType;
    }

    public void DestroyLine()
    {
        CleanupTrails();
        GameObject.Destroy(this.gameObject);
    }

    private void UpdateLineType(PairingLineType lineType)
    {
        mTransparentColor = new Color(Color.r, Color.g, Color.b, 0.35f);

        switch (lineType)
        {
            case PairingLineType.Default:
                mTransparentColor = Color;
                // set texture
                mLineRenderer.material.SetTexture("_MainTex", DefaultTexture);
                break;
            case PairingLineType.Trails:
                CreateTrail(Color);
                // set texture
                mLineRenderer.material.SetTexture("_MainTex", TrailsTexture);
                break;
            case PairingLineType.Dashed:
                mTransparentColor = Color;
                // set texture
                mLineRenderer.material.SetTexture("_MainTex", DashedTexture);
                break;
            default:
                break;
        }


        mLineRenderer.material.color = mTransparentColor;
        mLineRenderer.startWidth = Width;
        mLineRenderer.endWidth = Width;
        if (FlashColor)
            mLineRenderer.material.color = Color.white;

    }

    private void CreateTrail(Color color)
    {
        mLineInstance = Instantiate(LineTrailPrefab);
        TrailRenderer trailRenderer = mLineInstance.GetComponent<TrailRenderer>();
        Material mat = trailRenderer.material;
        trailRenderer.startWidth = Width * 2;
        trailRenderer.endWidth = Width;

        mat.SetColor("_Color", color);

        ParingTrail trail = new ParingTrail();
        trail.Trail = mLineInstance;
        trail.Time = 0;
        mTrailsList.Add(trail);
    }

    private void CleanupTrails()
    {
        for (int i = mTrailsList.Count - 1; i > -1; --i)
        {
            ParingTrail tempLine = mTrailsList[i];
            if (tempLine.Trail != null)
            {
                GameObject.Destroy(tempLine.Trail);
            }
            mTrailsList.RemoveAt(i);
        }
    }

    private void OnDestroy()
    {
        OnDestroyEvent.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {

        if (mOldLineType != LineType)
        {
            // update line type;
            UpdateLineType(LineType);
            mOldLineType = LineType;
        }

        bool hasEnd = (EndObject != null && EndObject.activeInHierarchy);
        bool hasStart = (StartPosition != null || StartObject != null);
        if (DestroyLineWhenObjectsDestroyed && (!hasStart || !hasEnd))
        {
            DestroyLine();
            return;
        }

        if (mLineRenderer)
        {
            mLineRenderer.enabled = true;

            Vector3 endPosition = new Vector3();
            if (EndObject != null)
                endPosition = EndObject.transform.position;

            Vector3 startPosition;
            if (StartObject != null)
                startPosition = StartObject.transform.position;
            else
                startPosition = StartPosition.Value;

            Color lineColor = Color;
            if (FlashColor)
            {
                flashT += Time.deltaTime;
                lineColor = Color.Lerp(Color.white, Color, flashT / flashTime);
                if (LineType == PairingLineType.Trails)
                {
                    mLineRenderer.material.color = mTransparentColor;
                }
                else
                {
                    mLineRenderer.material.color = lineColor;
                }


                if ((flashT / flashTime >= 1.0f))
                {
                    if (!LoopFlashing)
                    {
                        FlashColor = false;
                    }
                    else
                    {
                        flashT = 0f;
                    }
                }
            }
            else if (FadeAndDestory)
            {
                t += Time.deltaTime;

                lineColor = Color.Lerp(Color, Color.clear, t / FadeoutTime);
                if (LineType == PairingLineType.Trails)
                {
                    mLineRenderer.material.color = mTransparentColor;
                }
                else
                {
                    mLineRenderer.material.color = lineColor;
                }

                if (t >= FadeoutTime)
                {
                    DestroyLine();
                }
            }
            else
            {
                if (LineType == PairingLineType.Trails)
                {
                    mLineRenderer.material.color = mTransparentColor;
                }
                else
                {
                    mLineRenderer.material.color = lineColor;
                }
            }

            if (LineType == PairingLineType.Dashed)
            {
                float distance = Vector3.Distance(endPosition, startPosition);
                float tileFactor = distance / Width * mDashedOffset;
                mLineRenderer.material.SetTextureScale("_MainTex", new Vector2(tileFactor, 1f));
            }
            
            mLineRenderer.startWidth = Width;
            mLineRenderer.endWidth = Width;
            mLineRenderer.SetPosition(0, startPosition);
            mLineRenderer.SetPosition(1, endPosition);

            if (LineType == PairingLineType.Trails)
            {

                float trailDistance = Vector3.Distance(endPosition, startPosition);
                float trailSpeed = trailDistance / mTrailSpeedDistance;
                mTrailTime = trailSpeed * mTrailSpeed;


                for (int i = 0; i < mTrailsList.Count; ++i)
                {
                    ParingTrail tempTrail = mTrailsList[i];

                    if (tempTrail.Trail != null)
                    {
                        float perc = tempTrail.Time / mTrailTime;
                        tempTrail.Trail.transform.position = Vector3.Lerp(startPosition, endPosition, perc);

                        TrailRenderer trail = tempTrail.Trail.GetComponent<TrailRenderer>();
                        trail.time = mTrailTime * mTrailTimeFactor;

                        tempTrail.Time += Time.deltaTime;
                        mTrailsList[i] = tempTrail;
                    }
                }

                if (mSpawnCounter < mTrailSpeed * mSpawnTimeFactor)
                {
                    mSpawnCounter += Time.deltaTime;
                }
                else
                {
                    mSpawnCounter = 0;
                    CreateTrail(lineColor);
                }
            }
        }
    }
}