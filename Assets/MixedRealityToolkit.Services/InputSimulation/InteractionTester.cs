// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class InteractionTestValues : Dictionary<string, object>
    {
    }

    public class InteractionTester
    {
        private static Dictionary<Type, InteractionTester> testers = CreateTesters();

        public static bool TryGetTester(Type type, out InteractionTester tester)
        {
            return testers.TryGetValue(type, out tester);
        }

        public static bool TryGetTester<T>(T obj, out InteractionTester tester)
        {
            return TryGetTester(obj.GetType(), out tester);
        }

        private static Dictionary<Type, InteractionTester> CreateTesters()
        {
            var map = new Dictionary<Type, InteractionTester>();

            var assembly = Assembly.GetExecutingAssembly();
            var baseType = typeof(InteractionTester);
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsClass || type.IsAbstract ||
                    !type.IsSubclassOf(baseType))
                {
                    continue;
                }

                var instance = System.Activator.CreateInstance(type) as InteractionTester;
                map.Add(instance.TestedType, instance);
            }

            return map;
        }

        public virtual Type TestedType { get; }

        /// <summary>
        /// Copy relevant values of an object into a dictionary for testing.
        /// </summary>
        public virtual void GetTestValues(object tested, InteractionTestValues values)
        {}

        /// <summary>
        /// Ascertain that values match the state of the tested object.
        /// </summary>
        public virtual void CheckTestValues(object tested, InteractionTestValues values)
        {}
    }

    /// <summary>
    /// Specialized tester class for a specific component type.
    /// </summary>
    public abstract class ComponentInteractionTester<T> : InteractionTester where T : Component
    {
        public sealed override Type TestedType => typeof(T);

        /// <summary>
        /// Copy relevant values of an object into a dictionary for testing.
        /// </summary>
        public virtual void GetTestValues(T tested, InteractionTestValues values)
        {}

        /// <summary>
        /// Ascertain that values match the state of the tested object.
        /// </summary>
        public virtual void CheckTestValues(T tested, InteractionTestValues values)
        {}

        /// <inheritdoc/>
        public sealed override void GetTestValues(object _tested, InteractionTestValues values)
        {
            var tested = _tested as T;
            GetTestValues(tested, values);
        }

        /// <inheritdoc/>
        public sealed override void CheckTestValues(object _tested, InteractionTestValues values)
        {
            var tested = _tested as T;
            CheckTestValues(tested, values);
        }
    }

    // public class InteractableTester : ComponentInteractionTester<Interactable>
    // {
    //     /// <inheritdoc/>
    //     public override void GetTestValues(Interactable tested, InteractionTestValues values)
    //     {
    //         // basic button states
    //         values.Add("HasFocus", tested.HasFocus);
    //         values.Add("HasPress", tested.HasPress);
    //         values.Add("IsDisabled", tested.IsDisabled);

    //         // advanced button states from InteractableStates.InteractableStateEnum
    //         values.Add("IsTargeted", tested.IsTargeted);
    //         values.Add("IsInteractive", tested.IsInteractive);
    //         values.Add("HasObservationTargeted", tested.HasObservationTargeted);
    //         values.Add("HasObservation", tested.HasObservation);
    //         values.Add("IsVisited", tested.IsVisited);
    //         values.Add("IsToggled", tested.IsToggled);
    //         values.Add("HasGesture", tested.HasGesture);
    //         values.Add("HasGestureMax", tested.HasGestureMax);
    //         values.Add("HasCollision", tested.HasCollision);
    //         values.Add("HasVoiceCommand", tested.HasVoiceCommand);
    //         values.Add("HasPhysicalTouch", tested.HasPhysicalTouch);
    //         values.Add("HasCustom", tested.HasCustom);
    //     }

    //     /// <inheritdoc/>
    //     public override void CheckTestValues(Interactable tested, InteractionTestValues values)
    //     {
    //         // basic button states
    //         values["HasFocus"].Equals(tested.HasFocus);
    //         values["HasPress"].Equals(tested.HasPress);
    //         values["IsDisabled"].Equals(tested.IsDisabled);

    //         // advanced button states from InteractableStates.InteractableStateEnum
    //         values["IsTargeted"].Equals(tested.IsTargeted);
    //         values["IsInteractive"].Equals(tested.IsInteractive);
    //         values["HasObservationTargeted"].Equals(tested.HasObservationTargeted);
    //         values["HasObservation"].Equals(tested.HasObservation);
    //         values["IsVisited"].Equals(tested.IsVisited);
    //         values["IsToggled"].Equals(tested.IsToggled);
    //         values["HasGesture"].Equals(tested.HasGesture);
    //         values["HasGestureMax"].Equals(tested.HasGestureMax);
    //         values["HasCollision"].Equals(tested.HasCollision);
    //         values["HasVoiceCommand"].Equals(tested.HasVoiceCommand);
    //         values["HasPhysicalTouch"].Equals(tested.HasPhysicalTouch);
    //         values["HasCustom"].Equals(tested.HasCustom);
    //     }
    // }
}