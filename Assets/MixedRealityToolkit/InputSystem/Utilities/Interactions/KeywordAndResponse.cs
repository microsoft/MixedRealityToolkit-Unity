using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Utilities.Interactions
{
    [Serializable]
    public struct KeywordAndResponse
    {
        [Tooltip("The keyword to handle.")]
        public string Keyword;

        [Tooltip("The handler to be invoked.")]
        public UnityEvent Response;
    }
}