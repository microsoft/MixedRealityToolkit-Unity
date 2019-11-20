// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This script provides cursor context for the manipulation handler
    /// </summary>
    public class CursorContextManipulationHandler : MonoBehaviour
    {
        private CursorContextInfo contextInfo;
        private int hoverCount = 0;

        private void Start()
        {
            ManipulationHandler mh = GetComponent<ManipulationHandler>();

            contextInfo = gameObject.EnsureComponent<CursorContextInfo>();
            contextInfo.ObjectCenter = transform;

            mh.OnHoverEntered.AddListener(ManipulatorHoverEntered);
            mh.OnHoverExited.AddListener(ManipulatorHoverExited);
            mh.OnManipulationStarted.AddListener(ManipulationStarted);
            mh.OnManipulationEnded.AddListener(ManipulationEnded);
        }

        private void ManipulatorHoverEntered(ManipulationEventData manipEvent)
        {
            hoverCount++;
            if (hoverCount >= 2 && 
                manipEvent.ManipulationSource.ManipulationType != ManipulationHandler.HandMovementType.OneHandedOnly)
            {
                contextInfo.CurrentCursorAction = CursorContextInfo.CursorAction.Move;
            }
        }

        private void ManipulatorHoverExited(ManipulationEventData manipEvent)
        {
            hoverCount--;
            if (hoverCount < 2)
            {
                contextInfo.CurrentCursorAction = CursorContextInfo.CursorAction.None;
            }
        }

        private void ManipulationStarted(ManipulationEventData manipEvent)
        {
            contextInfo.CurrentCursorAction = CursorContextInfo.CursorAction.Move;
        }

        private void ManipulationEnded(ManipulationEventData manipEvent)
        {
            contextInfo.CurrentCursorAction = hoverCount < 2 ?
                CursorContextInfo.CursorAction.None :
                CursorContextInfo.CursorAction.Move;
        }
    }
}
