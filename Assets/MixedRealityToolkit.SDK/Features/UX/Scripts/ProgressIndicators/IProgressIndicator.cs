// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public interface IProgressIndicator
    {
        /// <summary>
        /// The progress indicator's main transform.
        /// You can use this to attach follow scripts or solvers to the indicator.
        /// </summary>
        Transform MainTransform { get; }

        /// <summary>
        /// Used to determine whether it's appropriate to use this indicator.
        /// </summary>
        ProgressIndicatorState State { get; }

        /// <summary>
        /// The message to display during loading.
        /// </summary>
        string Message { set; }

        /// <summary>
        /// Loading progress value from 0 (just started) to 1 (complete)
        /// </summary>
        float Progress { set; }

        /// <summary>
        /// Opens the progress indicator before loading begins. Method is async to allow for animation to begin before loading.
        /// </summary>
        Task OpenAsync();

        /// <summary>
        /// Closes the progress indicator after loading is finished. Method is async to allow for animation to complete.
        /// </summary>
        Task CloseAsync();

    }
}
