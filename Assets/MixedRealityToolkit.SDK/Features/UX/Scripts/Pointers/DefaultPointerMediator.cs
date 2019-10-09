// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

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

        private IPointerPreferences pointerPreferences;

        public DefaultPointerMediator()
            : this(null)
        {
        }

        public DefaultPointerMediator(IPointerPreferences pointerPrefs)
        {
            pointerPreferences = pointerPrefs;
        }

        public virtual void RegisterPointers(IMixedRealityPointer[] pointers)
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

        public virtual void UnregisterPointers(IMixedRealityPointer[] pointers)
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

        public virtual void UpdatePointers()
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

                    if (pointer.InputSourceParent != null)
                    {
                        foreach (IMixedRealityPointer otherPointer in pointerByInputSourceParent[pointer.InputSourceParent])
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

                    if (pointer.InputSourceParent != null)
                    {
                        foreach (IMixedRealityPointer otherPointer in pointerByInputSourceParent[pointer.InputSourceParent])
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

            // All other pointers that have not been assigned this frame
            // have no reason to be disabled, so make sure they are active
            foreach (IMixedRealityPointer unassignedPointer in unassignedPointers)
            {
                unassignedPointer.IsActive = true;
            }
        }

        private void ApplyCustomPointerBehaviors()
        {
            if (pointerPreferences != null)
            {
                Action<IMixedRealityPointer, PointerBehavior> setPointerState =
                    (ptr, behavior) =>
                    {
                        if (behavior == PointerBehavior.Default)
                        {
                            return;
                        }

                        bool isPointerOn = behavior == PointerBehavior.AlwaysOn;
                        ptr.IsActive = isPointerOn;
                        if (ptr is GenericPointer genericPtr)
                        {
                            genericPtr.IsInteractionEnabled = isPointerOn;
                        }
                        unassignedPointers.Remove(ptr);
                    };

                foreach (IMixedRealityPointer pointer in allPointers)
                {
                    setPointerState(pointer, pointerPreferences.GetPointerBehavior(pointer));
                }
            }
        }
    }
}