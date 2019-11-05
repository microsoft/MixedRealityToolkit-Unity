// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;

namespace Microsoft.MixedReality.Toolkit.Tests.Utilities.Editor
{
    public class DummyProjectPreferences : ProjectPreferences
    {
        protected new const string FILE_NAME = "DummyTestProjectPreferences.asset";

        public static string PreferencesFilePath = FilePath;
    }
}