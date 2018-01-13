// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.UX.BoundingBoxes
{
    public class Duplicator : MonoBehaviour
    {
        public enum StateEnum
        {
            Empty,
            Idle,
            Activated,
            Duplicating
        }

        public enum ActivateModeEnum
        {
            Auto,
            Manual,
        }

        public Action OnTargetIdle;
        public Action OnTargetActive;
        public Action OnTargetEmpty;
        public Action OnTargetDuplicateStart;
        public Action OnTargetDuplicateEnd;

        public StateEnum State {
            get {
                return state;
            }
            protected set {
                state = value;
            }
        }

        public GameObject Target {
            get {
                if (transform.childCount > 0)
                {
                    return transform.GetChild(0).gameObject;
                }
                return null;
            }
        }

        [Header("Idle & Activation settings")]
        [SerializeField]
        protected ActivateModeEnum activateMode = ActivateModeEnum.Auto;
        [SerializeField]
        protected float autoActivateRadius = 0.05f;
        [SerializeField]
        protected float removeRadius = 0.25f;
        [SerializeField]
        protected float restorePosSpeed = 1f;
        [SerializeField]
        protected float activeTimeout = 0.5f;
        [SerializeField]
        protected AnimationCurve restoreCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Duplication settings")]
        [SerializeField]
        private bool limitDuplicates = false;
        [SerializeField]
        protected int maxDuplicates = 10;
        [SerializeField]
        protected float duplicateSpeed = 0.5f;
        [SerializeField]
        protected AnimationCurve duplicateGrowCurve = AnimationCurve.EaseInOut(0f, 0.001f, 1f, 1f);

        [Header("Current state")]
        [SerializeField]
        protected StateEnum state = StateEnum.Idle;

        protected Vector3 targetPos;
        protected Vector3 targetScale;
        protected Quaternion targetRot;
        protected int numDuplicates;

        protected void OnEnable() {
            StartCoroutine(UpdateTarget());
        }

        protected virtual void StoreTarget() {
            if (Target == null) {
                Debug.LogError("No target found in duplicator.");
                return;
            }

            targetScale = Target.transform.localScale;
            targetPos = Target.transform.localPosition;
            targetRot = Target.transform.localRotation;
        }

        protected virtual void RestoreTarget(float normalizedTime) {
            normalizedTime = restoreCurve.Evaluate(normalizedTime);
            Target.transform.localPosition = Vector3.Lerp(Target.transform.localPosition, targetPos, normalizedTime);
            Target.transform.localRotation = Quaternion.Lerp(Target.transform.localRotation, targetRot, normalizedTime);
        }

        protected virtual IEnumerator UpdateTarget() {

            while (Target == null) {
                // Wait for a target to be added
                yield return null;
            }
            // Store the target's position
            StoreTarget();

            state = StateEnum.Idle;
            float normalizedTime = 0f;

            while (isActiveAndEnabled) {

                float idleStartTime = Time.unscaledTime;
                while (state == StateEnum.Idle) {
                    // While we're idle, move the target back into position
                    if (Target.transform.hasChanged) {
                        Target.transform.hasChanged = false;
                        idleStartTime = Time.unscaledTime;
                        // If our activate mode is auto, activate once the target is moved
                        if (activateMode == ActivateModeEnum.Auto && Vector3.Distance(Target.transform.position, transform.TransformPoint(targetPos)) > autoActivateRadius) {
                            state = StateEnum.Activated;

                            if (OnTargetActive != null)
                                OnTargetActive();
                        }
                    } else {
                        normalizedTime = (Time.unscaledTime - idleStartTime) / restorePosSpeed;
                        RestoreTarget(normalizedTime);
                    }
                    yield return null;
                }

                float lastTimeChanged = Time.unscaledTime;

                // While the target is activated, check to see if it's being moved
                while (state == StateEnum.Activated) {
                    // If the target has moved, check to see if it has exited the radius
                    if (Target.transform.hasChanged) {
                        lastTimeChanged = Time.unscaledTime;
                        Target.transform.hasChanged = false;
                        // If the target has exited the radius, duplicate it
                        if (Vector3.Distance (Target.transform.position, transform.TransformPoint(targetPos)) > removeRadius) {
                            state = StateEnum.Duplicating;
                        }
                    // If the target hasn't moved for the timeout period, go back to idle
                    } else if (Time.unscaledTime > (lastTimeChanged + activeTimeout)) {
                        // Move it back to the target pos
                        float timedOutTime = Time.unscaledTime;
                        while (Time.unscaledTime < timedOutTime + restorePosSpeed) {
                            normalizedTime = (Time.unscaledTime - timedOutTime) / restorePosSpeed;
                            RestoreTarget(normalizedTime);
                            yield return null;
                        }
                        state = StateEnum.Idle;

                        if(OnTargetIdle != null)
                            OnTargetIdle();
                    }
                    yield return null;
                }

                if (state == StateEnum.Duplicating) {
                    // Unparent the object - at this point it's not our problem
                    GameObject originalTarget = Target;
                    originalTarget.transform.parent = null;
                    if (limitDuplicates && numDuplicates >= maxDuplicates) {
                        // No more items to duplicate
                        state = StateEnum.Empty;

                        if (OnTargetEmpty != null)
                            OnTargetEmpty();

                    } else {
                        // Duplicating!
                        if (OnTargetDuplicateStart != null)
                            OnTargetDuplicateStart();

                        GameObject.Instantiate(originalTarget, transform);
                        Target.name = originalTarget.name;
                        Target.transform.localPosition = targetPos;
                        Target.transform.localRotation = targetRot;
                        float duplicateStartTime = Time.unscaledTime;
                        while (Time.unscaledTime < duplicateStartTime + duplicateSpeed) {
                            normalizedTime = (Time.unscaledTime - duplicateStartTime) / duplicateSpeed;
                            Target.transform.localScale = targetScale * duplicateGrowCurve.Evaluate(normalizedTime);
                            yield return null;
                        }
                        Target.transform.localScale = targetScale;
                        numDuplicates++;

                        if (OnTargetDuplicateEnd != null)
                            OnTargetDuplicateEnd();

                        state = StateEnum.Idle;

                        if (OnTargetIdle != null)
                            OnTargetIdle();
                    }
                }
                yield return null;
            }
        }

        private void OnDrawGizmos() {

            switch (state) {
                case StateEnum.Idle:
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(transform.TransformPoint(targetPos), autoActivateRadius);
                    break;

                case StateEnum.Activated:
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(transform.TransformPoint(targetPos), removeRadius);
                    break;

                case StateEnum.Duplicating:
                    break;
            }

            if (Application.isPlaying) {
                return;
            }

            if (Target != null) {
                StoreTarget();
            }
        }
    }
}