// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Allows for UnityEvents invoking based off bound variable updates.
    /// Works in two situations:
    /// 1) Looking to identify simply when a target variable (of any type) has changed. This is useful for lists populating, etc.
    /// 2) Looking to bind true/false events to a BOOLEAN bound variable.
    /// </summary>
    [Serializable]
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Boolean Events")]
    public class DataConsumerBooleanEvents : DataConsumerGOBase
    {
        [Serializable]
        protected class BooleanEvent
        {
            [Tooltip("The local key path associated with this event.")]
            [SerializeField]
            private string localKeyPath = null;

            /// <summary>
            /// The local keypath that will trigger the events in this object when states change
            /// </summary>
            /// <remarks>
            /// This is generally intended to be bound to a bool value. However, it will attempt to
            /// take a meaningful action for other data types as well:
            ///
            ///     string - null or empty string is false, otherwise true
            ///     numeric - 0 is false, any other value is true
            ///     collection - empty is false, otherwise true
            ///
            ///     Note that it is possible for an event to be triggered more than
            ///     once for the same value in some circumstances.
            /// </remarks>
            public string LocalKeyPath => localKeyPath;

            [Tooltip("An event triggered after a boolean value at the specified keypath has changed state.")]
            [SerializeField]
            private UnityEvent onValueChanged = new UnityEvent();

            /// <summary>
            /// The event that will be triggered after the value changes
            /// </summary>
            public UnityEvent OnValueChanged => onValueChanged;

            [Tooltip("An event triggered when a boolean value at the specified keypath becomes true.")]
            [SerializeField]
            private UnityEvent onValueTrue = new UnityEvent();

            public UnityEvent OnValueTrue => onValueTrue;

            [Tooltip("An event triggered when a boolean value at the specified keypath becomes false.")]
            [SerializeField]
            private UnityEvent onValueFalse = new UnityEvent();

            public UnityEvent OnValueFalse => onValueFalse;
        }

        [Tooltip("An array of keypaths and the events they trigger based on state changes.")]
        [SerializeField]
        private BooleanEvent[] booleanEvents;

        protected Dictionary<string, BooleanEvent> keyPathToEventLookup = new Dictionary<string, BooleanEvent>();

        protected override void AttachDataConsumer()
        {
            foreach (BooleanEvent boolEvent in booleanEvents)
            {
                AddKeyPathListener(boolEvent.LocalKeyPath);
                keyPathToEventLookup[boolEvent.LocalKeyPath] = boolEvent;
            }
        }

        protected override void DetachDataConsumer()
        {
            keyPathToEventLookup.Clear();
        }

        protected override bool ManageChildren()
        {
            return false;
        }

        protected override void ProcessDataChanged(IDataSource dataSource, string resolvedKeyPath, string localKeyPath, object newValue, DataChangeType dataChangeType)
        {
            if (keyPathToEventLookup.ContainsKey(localKeyPath))
            {
                BooleanEvent boolEvent = keyPathToEventLookup[localKeyPath];
                boolEvent.OnValueChanged?.Invoke();

                // use finalBoolValue to trigger appropriate events
                if (ConvertToBool(newValue))
                {
                    boolEvent.OnValueTrue?.Invoke();
                }
                else
                {
                    boolEvent.OnValueFalse?.Invoke();
                }
            }
        }

        protected bool ConvertToBool(object newValue)
        {
            if (newValue != null)
            {
                if (newValue is bool newBoolValue)
                {
                    return newBoolValue;
                }
                else if (newValue is string newStringValue)
                {
                    return newStringValue.Length > 0;
                }
                else if (IsNumber(newValue))
                {
                    return System.Convert.ToDouble(newValue) != 0;
                }
                else if (newValue is ICollection newCollectionValue)
                {
                    return newCollectionValue.Count != 0;
                }
            }

            return false;
        }

        protected bool IsNumber(object value)
        {
            return value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal;
        }
    }
}
