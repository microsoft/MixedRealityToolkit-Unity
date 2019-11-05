using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Extensions
{
    public interface IHandPhysicsService : IMixedRealityExtensionService
    {
        /// <summary>
        /// The Parent GameObject that contains all the PhysicsJoints
        /// </summary>
        GameObject HandPhysicsServiceRoot { get; }

        /// <summary>
        /// Whether make the Palm a PhysicsJoint
        /// </summary>
        bool UsePalmKinematicBody { get; set; }

        /// <summary>
        /// The prefab to represent each PhysicsJoint
        /// </summary>
        GameObject FingerTipKinematicBodyPrefab { get; set; }

        /// <summary>
        /// The prefab to represent the Palm PhysicsJoint
        /// </summary>
        GameObject PalmKinematicBodyPrefab { get; set; }
    }
}