// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Examples.Prototyping
{
    public class ScaleByDistance : MonoBehaviour
    {
        public GameObject DistanceObject;
        public GameObject ScaleObject;
        public GameObject ReferenceInteractive;
        public float ScaleDistance = 1;
        public bool IsScaling;
        public float ScaleSpeed = 3;
        public float MinimumScale = 0.3f;

        private Vector3 mStartScale;
        private float mCurrentScale = 1;
        private float mDeltaScale;
        private float mStartDistance;

        private NetworkIdentity mNetworkRoot = null;

        private void Awake()
        {
            mNetworkRoot = GetComponentInParent<NetworkIdentity>();
        }

        // Use this for initialization
        void Start()
        {
            if (ScaleObject == null)
            {
                ScaleObject = this.gameObject;
            }

            if (DistanceObject == null)
            {
                DistanceObject = Camera.main.gameObject;
            }
        }

        public void StartScaling(bool state)
        {
            mStartScale = ScaleObject.transform.localScale;
            mStartDistance = Vector3.Distance(ScaleObject.transform.position, DistanceObject.transform.position);
            IsScaling = state;

            if (!state)
            {
                mCurrentScale = mDeltaScale;
            }
        }

        public void StartRunning()
        {

            StartScaling(true);
            if (ReferenceInteractive != null)
            {
                InputManager.Instance.PushModalInputHandler(ReferenceInteractive);
            }
        }

        public void StopRunning()
        {

            if (ReferenceInteractive != null)
            {
                InputManager.Instance.PopModalInputHandler();
            }
            IsScaling = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsScaling)
            {
                float ratio = (Vector3.Distance(ScaleObject.transform.position, DistanceObject.transform.position) - mStartDistance) / ScaleDistance;
                mDeltaScale = Mathf.Max(mCurrentScale + ratio, MinimumScale);
                Vector3 targetScale = mStartScale * mDeltaScale;
                ScaleObject.transform.localScale = Vector3.Lerp(ScaleObject.transform.localScale, targetScale, Time.deltaTime * ScaleSpeed);

            }
        }
    }
}
