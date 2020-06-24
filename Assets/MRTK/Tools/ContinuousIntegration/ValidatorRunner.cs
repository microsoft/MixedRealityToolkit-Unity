// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tools.ContinuousIntegration
{
    /// <summary>
    /// The validator runner that is designed to be called from the command line as part
    /// of CI pipelines.
    /// </summary>
    public class ValidatorRunner
    {
        /// <summary>
        /// The list of validators that this runner will run.
        /// </summary>
        private static readonly List<IBaseValidator> validators = new List<IBaseValidator>
        {
            new ProfileSerializationValidator(),
            new ProfileAttributeValidator(),
        };

        /// <summary>
        /// Runs the CI-time code based validators.
        /// </summary>
        /// <remarks>
        /// While this is primarily being used from CI tools, it's also possible to run this by
        /// annotating this with: 
        /// [UnityEditor.MenuItem("Mixed Reality Toolkit/CI/Validate")]
        /// </remarks>
        public static void Run()
        {
            bool succeeded = true;
            foreach (var validator in validators)
            {
                // The order of operation below ensures that all validators get run even if a preceeding
                // one has failed. This relies on individual validators to report details of failures
                // as no logging is done here.
                succeeded = validator.Validate() && succeeded;
            }

            EditorApplication.Exit(succeeded ? 0 : 1);
        }
    }
}