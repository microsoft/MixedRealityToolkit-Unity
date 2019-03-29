// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    // [System.Serializable]
    // public class InteractionTestValueMap : IEnumerable, ISerializationCallbackReceiver
    // {
    //     [System.Serializable]
    //     internal class SerializableIntList : List<Tuple<string, int>>
    //     {}
    //     [System.Serializable]
    //     internal class SerializableBoolList : List<Tuple<string, bool>>
    //     {}

    //     private readonly SerializableIntList intValues = new SerializableIntList();
    //     private readonly SerializableBoolList boolValues = new SerializableBoolList();

    //     public void Add<T>(string name, T value)
    //     {
    //         Debug.LogWarning($"Interaction test does not support values of type {typeof(T)}");
    //     }
    //     public void Add(string name, int value)
    //     {
    //         intValues.Add(new Tuple<string, int>(name, value));
    //     }
    //     public void Add(string name, bool value)
    //     {
    //         boolValues.Add(new Tuple<string, bool>(name, value));
    //     }

    //     public IEnumerator GetEnumerator()
    //     {
    //         foreach (var v in intValues)
    //         {
    //             yield return v;
    //         }
    //         foreach (var v in boolValues)
    //         {
    //             yield return v;
    //         }
    //     }

    //     public void OnBeforeSerialize()
    //     {

    //     }

    //     public void OnAfterDeserialize()
    //     {

    //     }
    // }

    public interface IInteractionTestValueHandler
    {
        void AddValue<T>(string name, T value);
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
        public virtual void GetTestValues(object tested, IInteractionTestValueHandler values)
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
        public virtual void GetTestValues(T tested, IInteractionTestValueHandler values)
        {}

        /// <inheritdoc/>
        public sealed override void GetTestValues(object _tested, IInteractionTestValueHandler values)
        {
            var tested = _tested as T;
            GetTestValues(tested, values);
        }
    }

    public class InteractableTester : ComponentInteractionTester<Interactable>
    {
        /// <inheritdoc/>
        public override void GetTestValues(Interactable tested, IInteractionTestValueHandler values)
        {
            // basic button states
            values.AddValue("HasFocus", tested.HasFocus);
            values.AddValue("HasPress", tested.HasPress);
            values.AddValue("IsDisabled", tested.IsDisabled);

            // advanced button states from InteractableStates.InteractableStateEnum
            values.AddValue("IsTargeted", tested.IsTargeted);
            values.AddValue("IsInteractive", tested.IsInteractive);
            values.AddValue("HasObservationTargeted", tested.HasObservationTargeted);
            values.AddValue("HasObservation", tested.HasObservation);
            values.AddValue("IsVisited", tested.IsVisited);
            values.AddValue("IsToggled", tested.IsToggled);
            values.AddValue("HasGesture", tested.HasGesture);
            values.AddValue("HasGestureMax", tested.HasGestureMax);
            values.AddValue("HasCollision", tested.HasCollision);
            values.AddValue("HasVoiceCommand", tested.HasVoiceCommand);
            values.AddValue("HasPhysicalTouch", tested.HasPhysicalTouch);
            values.AddValue("HasCustom", tested.HasCustom);
        }
    }
}