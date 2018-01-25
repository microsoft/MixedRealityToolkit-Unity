// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class BaseInputMapping<T> : IInputSourceMapping<T>
    {
        [SerializeField]
        private InputSourceMappingValue<T>[] mappingValues;

        public InputSourceMappingValue<T>[] MappingValues
        {
            get { return mappingValues; }
            set { mappingValues = value; }
        }

        public string GetMapping(T type)
        {
            for (var i = 0; i < MappingValues.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(mappingValues[i].InputType, type))
                {
                    return MappingValues[i].Value;
                }
            }

            return string.Empty;
        }

        public void SetMapping(T type, string value)
        {
            for (int i = 0; i < mappingValues.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(mappingValues[i].InputType, type))
                {
                    mappingValues[i].Value = value;
                }
            }
        }

        public float GetAxis(T type)
        {
            for (int i = 0; i < mappingValues.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(mappingValues[i].InputType, type))
                {
                    return Input.GetAxis(mappingValues[i].Value);
                }
            }

            return 0f;
        }

        public bool GetButtonUp(T type)
        {
            for (int i = 0; i < mappingValues.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(mappingValues[i].InputType, type))
                {
                    return Input.GetButtonUp(mappingValues[i].Value);
                }
            }

            return false;
        }

        public bool GetButtonDown(T type)
        {
            for (int i = 0; i < mappingValues.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(mappingValues[i].InputType, type))
                {
                    return Input.GetButtonDown(mappingValues[i].Value);
                }
            }

            return false;
        }

        public bool GetButtonPressed(T type)
        {
            for (int i = 0; i < mappingValues.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(mappingValues[i].InputType, type))
                {
                    return Input.GetButtonDown(mappingValues[i].Value);
                }
            }

            return false;
        }
    }
}
