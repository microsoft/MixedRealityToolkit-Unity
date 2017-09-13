//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing.SyncModel;

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Synchronizer to update and broadcast a transform object through our data model.
    /// </summary>
    public class TransformSynchronizer : MonoBehaviour
    {
        protected Vector3Interpolated Position;
        protected QuaternionInterpolated Rotation;
        protected Vector3Interpolated Scale;

        private SyncTransform transformDataModel;

        /// <summary>
        /// Data model to which this transform synchronization is tied to.
        /// </summary>
        public SyncTransform TransformDataModel
        {
            private get { return transformDataModel; }
            set
            {
                if (transformDataModel != value)
                {
                    if (transformDataModel != null)
                    {
                        transformDataModel.PositionChanged -= OnPositionChanged;
                        transformDataModel.RotationChanged -= OnRotationChanged;
                        transformDataModel.ScaleChanged -= OnScaleChanged;
                    }

                    transformDataModel = value;

                    if (transformDataModel != null)
                    {
                        // Set the position, rotation and scale to what they should be
                        transform.localPosition = transformDataModel.Position.Value;
                        transform.localRotation = transformDataModel.Rotation.Value;
                        transform.localScale = transformDataModel.Scale.Value;

                        // Initialize
                        Initialize();

                        // Register to changes
                        transformDataModel.PositionChanged += OnPositionChanged;
                        transformDataModel.RotationChanged += OnRotationChanged;
                        transformDataModel.ScaleChanged += OnScaleChanged;
                    }
                }
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (Position == null)
            {
                Position = new Vector3Interpolated(transform.localPosition);
            }
            if (Rotation == null)
            {
                Rotation = new QuaternionInterpolated(transform.localRotation);
            }
            if (Scale == null)
            {
                Scale = new Vector3Interpolated(transform.localScale);
            }
        }

        private void Update()
        {
            // Apply transform changes, if any
            if (Position.HasUpdate())
            {
                transform.localPosition = Position.GetUpdate(Time.deltaTime);
            }
            if (Rotation.HasUpdate())
            {
                transform.localRotation = Rotation.GetUpdate(Time.deltaTime);
            }
            if (Scale.HasUpdate())
            {
                transform.localScale = Scale.GetUpdate(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            // Determine if the transform has changed locally, in which case we need to update the data model
            if (transform.localPosition != Position.Value ||
                Quaternion.Angle(transform.localRotation, Rotation.Value) > 0.2f ||
                transform.localScale != Scale.Value)
            {
                transformDataModel.Position.Value = transform.localPosition;
                transformDataModel.Rotation.Value = transform.localRotation;
                transformDataModel.Scale.Value = transform.localScale;

                // The object was moved locally, so reset the target positions to the current position
                Position.Reset(transform.localPosition);
                Rotation.Reset(transform.localRotation);
                Scale.Reset(transform.localScale);
            }
        }

        private void OnDestroy()
        {
            if (transformDataModel != null)
            {
                transformDataModel.PositionChanged -= OnPositionChanged;
                transformDataModel.RotationChanged -= OnRotationChanged;
                transformDataModel.ScaleChanged -= OnScaleChanged;
            }
        }

        private void OnPositionChanged()
        {
            Position.SetTarget(transformDataModel.Position.Value);
        }

        private void OnRotationChanged()
        {
            Rotation.SetTarget(transformDataModel.Rotation.Value);
        }

        private void OnScaleChanged()
        {
            Scale.SetTarget(transformDataModel.Scale.Value);
        }
    }
}
