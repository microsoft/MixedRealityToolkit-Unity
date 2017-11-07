//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Collections;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Mesh button is a mesh renderer interactable with state data for button state
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    public class MeshButton : Button
    {
        /// <summary>
        /// If true then use the 
        /// </summary>
        public bool UseAnimator = false;

        /// <summary>
        /// Array of button states for Mesh Button Datum
        /// </summary>
        [Header("Mesh Button")]
        [Tooltip("Button State information")]
        public MeshButtonDatum[] ButtonStates = new MeshButtonDatum[]{ new MeshButtonDatum((ButtonStateEnum)0), new MeshButtonDatum((ButtonStateEnum)1),
            new MeshButtonDatum((ButtonStateEnum)2), new MeshButtonDatum((ButtonStateEnum)3),
            new MeshButtonDatum((ButtonStateEnum)4), new MeshButtonDatum((ButtonStateEnum)5) };

        /// <summary>
        /// Mesh renderer button for mesh button.
        /// </summary>
        private MeshRenderer _renderer;
        
        /// <summary>
        /// Mesh filter object for mesh button.
        /// </summary>
        private MeshFilter _meshFilter;

        /// <summary>
        /// Mesh filter object for mesh button.
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// On state change swap out the active mesh based on the state
        /// </summary>
        public override void OnStateChange(ButtonStateEnum newState)
        {
            MeshButtonDatum stateDatum = ButtonStates[(int)newState];

            // if filter or renderer are null set them
            if (_meshFilter == null)
                _meshFilter = this.GetComponent<MeshFilter>();

            if (_renderer == null)
                _renderer = this.GetComponent<MeshRenderer>();


            if (_animator == null)
                _animator = this.GetComponent<Animator>();

            // Play animator state
            if (UseAnimator)
                _animator.Play(stateDatum.Name);

            // Set the color from the datum 
            if (_renderer != null)
                    _renderer.material.color = stateDatum.StateColor;

            base.OnStateChange(newState);
        }
    }
}