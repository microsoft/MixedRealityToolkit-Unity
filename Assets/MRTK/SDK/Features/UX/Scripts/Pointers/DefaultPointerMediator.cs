﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The default implementation for pointer mediation in MRTK which is responsible for
    /// determining which pointers are active based on the state of all pointers.
    /// For example, one of the key things this class does is disable far pointers when a near pointer is close to an object.
    /// </summary>
    public class DefaultPointerMediator : IMixedRealityPointerMediator
    {
        protected readonly HashSet<IMixedRealityPointer> allPointers = new HashSet<IMixedRealityPointer>();
        protected readonly HashSet<IMixedRealityPointer> farInteractPointers = new HashSet<IMixedRealityPointer>();
        protected readonly HashSet<IMixedRealityNearPointer> nearInteractPointers = new HashSet<IMixedRealityNearPointer>();
        protected readonly HashSet<IMixedRealityTeleportPointer> teleportPointers = new HashSet<IMixedRealityTeleportPointer>();
        protected readonly HashSet<IMixedRealityPointer> unassignedPointers = new HashSet<IMixedRealityPointer>();
        protected readonly Dictionary<IMixedRealityInputSource, HashSet<IMixedRealityPointer>> pointerByInputSourceParent = new Dictionary<IMixedRealityInputSource, HashSet<IMixedRealityPointer>>();
        protected IPointerPreferences pointerPreferences;

        public DefaultPointerMediator()
        {
            pointerPreferences = null;
        }

        [Obsolete("Use DefaultPointerMediator() instead, followed by a call to SetPointerPreferences()")]
        public DefaultPointerMediator(IPointerPreferences pointerPrefs)
        {
            pointerPreferences = pointerPrefs;
        }

        private static readonly ProfilerMarker RegisterPointersPerfMarker = new ProfilerMarker("[MRTK] DefaultPointerMediator.RegisterPointers");

        public virtual void RegisterPointers(IMixedRealityPointer[] pointers)
        {
            using (RegisterPointersPerfMarker.Auto())
            {
                for (int i = 0; i < pointers.Length; i++)
                {
                    IMixedRealityPointer pointer = pointers[i];

                    allPointers.Add(pointer);

                    pointer.IsActive = true;

                    if (pointer is IMixedRealityTeleportPointer)
                    {
                        teleportPointers.Add(pointer as IMixedRealityTeleportPointer);
                    }
                    else if (pointer is IMixedRealityNearPointer)
                    {
                        nearInteractPointers.Add(pointer as IMixedRealityNearPointer);
                    }
                    else
                    {
                        farInteractPointers.Add(pointer);
                    }

                    if (pointer.InputSourceParent != null)
                    {
                        HashSet<IMixedRealityPointer> children;
                        if (!pointerByInputSourceParent.TryGetValue(pointer.InputSourceParent, out children))
                        {
                            children = new HashSet<IMixedRealityPointer>();
                            pointerByInputSourceParent.Add(pointer.InputSourceParent, children);
                        }
                        children.Add(pointer);
                    }
                }
            }
        }

        private static readonly ProfilerMarker UnregisterPointersPerfMarker = new ProfilerMarker("[MRTK] DefaultPointerMediator.UnregisterPointers");

        public virtual void UnregisterPointers(IMixedRealityPointer[] pointers)
        {
            using (UnregisterPointersPerfMarker.Auto())
            {
                for (int i = 0; i < pointers.Length; i++)
                {
                    IMixedRealityPointer pointer = pointers[i];

                    allPointers.Remove(pointer);
                    farInteractPointers.Remove(pointer);
                    nearInteractPointers.Remove(pointer as IMixedRealityNearPointer);
                    teleportPointers.Remove(pointer as IMixedRealityTeleportPointer);

                    foreach (HashSet<IMixedRealityPointer> siblingPointers in pointerByInputSourceParent.Values)
                    {
                        siblingPointers.Remove(pointer);
                    }
                }
            }
        }

        private static readonly ProfilerMarker UpdatePointersPerfMarker = new ProfilerMarker("[MRTK] DefaultPointerMediator.UpdatePointers");

        public virtual void UpdatePointers()
        {
            using (UpdatePointersPerfMarker.Auto())
            {
                // If there's any teleportation going on, disable all pointers except the teleporter
                foreach (IMixedRealityTeleportPointer pointer in teleportPointers)
                {
                    if (pointer.TeleportRequestRaised)
                    {
                        pointer.IsActive = true;

                        foreach (IMixedRealityPointer otherPointer in allPointers)
                        {
                            if (otherPointer.PointerId == pointer.PointerId)
                            {
                                continue;
                            }

                            otherPointer.IsActive = false;
                        }
                        // Don't do any further checks
                        return;
                    }
                }

                // pointers whose active state has not yet been set this frame
                unassignedPointers.Clear();
                foreach (IMixedRealityPointer unassignedPointer in allPointers)
                {
                    unassignedPointers.Add(unassignedPointer);
                }

                ApplyCustomPointerBehaviors();

                // If any pointers are locked, they have priority. 
                // Deactivate all other pointers that are on that input source
                foreach (IMixedRealityPointer pointer in allPointers)
                {
                    if (pointer.IsFocusLocked)
                    {
                        pointer.IsActive = true;
                        unassignedPointers.Remove(pointer);

                        HashSet<IMixedRealityPointer> children;

                        if (pointer.InputSourceParent != null && pointerByInputSourceParent.TryGetValue(pointer.InputSourceParent, out children))
                        {
                            foreach (IMixedRealityPointer otherPointer in children)
                            {
                                if (!unassignedPointers.Contains(otherPointer))
                                {
                                    continue;
                                }

                                otherPointer.IsActive = false;
                                unassignedPointers.Remove(otherPointer);
                            }
                        }
                    }
                }

                // Check for near and far interactions
                // Any far interact pointers become disabled when a near pointer is near an object
                foreach (IMixedRealityNearPointer pointer in nearInteractPointers)
                {
                    if (!unassignedPointers.Contains(pointer))
                    {
                        continue;
                    }

                    if (pointer.IsNearObject)
                    {
                        pointer.IsActive = true;
                        unassignedPointers.Remove(pointer);

                        HashSet<IMixedRealityPointer> children;

                        if (pointer.InputSourceParent != null && pointerByInputSourceParent.TryGetValue(pointer.InputSourceParent, out children))
                        {
                            foreach (IMixedRealityPointer otherPointer in children)
                            {
                                if (!unassignedPointers.Contains(otherPointer))
                                {
                                    continue;
                                }

                                if (otherPointer is IMixedRealityNearPointer)
                                {
                                    // Only disable far interaction pointers
                                    // It is okay for example to have two near pointers active on a single controller
                                    // like a poke pointer and a grab pointer
                                    continue;
                                }

                                otherPointer.IsActive = false;
                                unassignedPointers.Remove(otherPointer);
                            }
                        }
                    }
                }

                // Check for far interactions
                // Any far pointer other than GGV has priority over GGV
                foreach (IMixedRealityPointer pointer in farInteractPointers)
                {
                    if (!unassignedPointers.Contains(pointer))
                    {
                        continue;
                    }

                    if (!(pointer is GGVPointer))
                    {
                        pointer.IsActive = true;
                        unassignedPointers.Remove(pointer);

                        HashSet<IMixedRealityPointer> children;

                        if (pointer.InputSourceParent != null && pointerByInputSourceParent.TryGetValue(pointer.InputSourceParent, out children))
                        {
                            foreach (IMixedRealityPointer otherPointer in children)
                            {
                                if (!unassignedPointers.Contains(otherPointer) || !farInteractPointers.Contains(otherPointer))
                                {
                                    continue;
                                }

                                if (otherPointer is GGVPointer)
                                {
                                    // Disable the GGV pointer of an input source
                                    // when there is another far pointer belonging to the same source
                                    otherPointer.IsActive = false;
                                    unassignedPointers.Remove(otherPointer);
                                }
                            }
                        }
                    }
                }

                // All other pointers that have not been assigned this frame
                // have no reason to be disabled, so make sure they are active
                foreach (IMixedRealityPointer unassignedPointer in unassignedPointers)
                {
                    unassignedPointer.IsActive = true;
                }
            }
        }

        /// <inheritdoc />
        public void SetPointerPreferences(IPointerPreferences pointerPreferences)
        {
            this.pointerPreferences = pointerPreferences;
        }

        private static readonly ProfilerMarker ApplyCustomPointerBehaviorsPerfMarker = new ProfilerMarker("[MRTK] DefaultPointerMediator.ApplyCustomPointerBehaviors");

        private void ApplyCustomPointerBehaviors()
        {
            using (ApplyCustomPointerBehaviorsPerfMarker.Auto())
            {
                if (pointerPreferences != null)
                {
                    foreach (IMixedRealityPointer pointer in allPointers)
                    {
                        ApplyPointerBehavior(pointer, pointerPreferences.GetPointerBehavior(pointer));
                    }
                }
            }
        }

        private static readonly ProfilerMarker ApplyPointerBehaviorPerfMarker = new ProfilerMarker("[MRTK] DefaultPointerMediator.ApplyPointerBehavior");

        private void ApplyPointerBehavior(IMixedRealityPointer pointer, PointerBehavior behavior)
        {
            using (ApplyPointerBehaviorPerfMarker.Auto())
            {
                if (behavior == PointerBehavior.Default)
                {
                    return;
                }

                bool isPointerOn = behavior == PointerBehavior.AlwaysOn;
                pointer.IsActive = isPointerOn;
                if (pointer is GenericPointer genericPtr)
                {
                    genericPtr.IsInteractionEnabled = isPointerOn;
                }

                unassignedPointers.Remove(pointer);
            }
        }
    }
}
