// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A class that represents a Unity assembly definition (asmdef) file.
    /// </summary>
    [Serializable]
    public class AssemblyDefinition
    {
        /// <summary>
        /// Creates a new, empty assembly definition.
        /// </summary>
        public AssemblyDefinition() { }

        [SerializeField]
        private string name = null;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public string Name
        {
            get => name;
            set => name = value;
        }

        [SerializeField]
        private string[] references = null;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public string[] References
        {
            get => references;
            set => references = value;
        }

#if !UNITY_2019_3_OR_NEWER
        [SerializeField]
        private string[] optionalUnityReferences = null;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public string[] OptionalUnityReferences
        {
            get => optionalUnityReferences;
            set => optionalUnityReferences = value;
        }
#endif // !UNITY_2019_3_OR_NEWER

        [SerializeField]
        private string[] includePlatforms = null;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public string[] IncludePlatforms
        {
            get => includePlatforms;
            set => includePlatforms = value;
        }

        [SerializeField]
        private string[] excludePlatforms = null;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public string[] ExcludePlatforms
        {
            get => excludePlatforms;
            set => excludePlatforms = value;
        }

        [SerializeField]
        private bool allowUnsafeCode = false;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public bool AllowUnsafeCode
        {
            get => allowUnsafeCode;
            set => allowUnsafeCode = value;
        }

        [SerializeField]
        private bool overrideReferences = false;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public bool OverrideReferences
        {
            get => overrideReferences;
            set => overrideReferences = value;
        }

        [SerializeField]
        private string[] precompiledReferences = null;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public string[] PrecompiledReferences
        {
            get => precompiledReferences;
            set => precompiledReferences = value;
        }

        [SerializeField]
        private bool autoReferenced = true;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public bool AutoReferenced
        {
            get => autoReferenced;
            set => autoReferenced = value;
        }

        [SerializeField]
        private string[] defineConstraints = null;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public string[] DefineConstraints
        {
            get => defineConstraints;
            set => defineConstraints = value;
        }

#if UNITY_2019_3_OR_NEWER
        [SerializeField]
        private VersionDefine[] versionDefines = null;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public VersionDefine[] VersionDefines
        {
            get => versionDefines;
            set => versionDefines = value;
        }

        [SerializeField]
        private bool noEngineReferences = false;

        /// <summary>
        /// Please see <see href="https://docs.unity3d.com/Manual/class-AssemblyDefinitionImporter.html">Assembly Definition properties</see> on the Unity documentation site.
        /// </summary>
        public bool NoEngineReferences
        {
            get => noEngineReferences;
            set => noEngineReferences = value;
        }
#endif // UNITY_2019_3_OR_NEWER

        /// <summary>
        /// Loads an existing assembly definition file.
        /// </summary>
        /// <param name="fileName">The file to be loaded.</param>
        /// <returns>The assembly definition that has been loaded, or null.</returns>
        public static AssemblyDefinition Load(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Debug.LogError("An assembly definition file name must be specified.");
                return null;
            }

            FileInfo file = new FileInfo(fileName);
            if (!file.Exists)
            {
                Debug.LogError($"The {fileName} file could not be found.");
                return null;
            }

            return JsonUtility.FromJson<AssemblyDefinition>(File.ReadAllText(file.FullName));
        }

        /// <summary>
        /// Saves an assembly definition file.
        /// </summary>
        /// <param name="fileName">The name by which to save the assembly definition file.</param>
        /// <remarks>
        /// If the specified file exists, it will be overwritten.
        /// </remarks>
        public void Save(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                Debug.LogError("A name for the assembly definition file must be specified.");
                return;
            }

            FileInfo file = new FileInfo(fileName);

            bool readOnly = (file.Exists) ? file.IsReadOnly : false;
            if (readOnly)
            {
                file.IsReadOnly = false;
            }

            Debug.Log($"Saving {fileName}");
            using (StreamWriter writer = new StreamWriter(fileName, false))
            {
                writer.Write(JsonUtility.ToJson(this, true));
            }

            if (readOnly)
            {
                file.IsReadOnly = true;
            }
        }

        /// <summary>
        /// Adds a reference to an existing assembly definition.  Make sure to call Save() after using this method to save the changes.
        /// </summary>
        /// <param name="referenceName">The name of the reference to add to this assembly definition</param>
        public void AddReference(string referenceName)
        {
            if (string.IsNullOrWhiteSpace(referenceName))
            {
                Debug.LogError($"The reference name is empty and was not added to the {Name} assembly definition file.");
                return;
            }

            if (References == null)
            {
                References = new string[] { };
            }

            List<string> refs = References.ToList();

            if (!refs.Contains(referenceName))
            {
                refs.Add(referenceName);
            }
            else 
            {
                Debug.Log($"The {referenceName} reference name is already listed in the {Name} file.");
                return;
            }

            References = refs.ToArray();
        }
    }

#if UNITY_2019_3_OR_NEWER
    /// <summary>
    /// Represents a subclass of a Unity assembly definition (asmdef) file.
    /// </summary>
    [Serializable]
    public struct VersionDefine : IEquatable<VersionDefine>
    {
        public VersionDefine(string name, string expression, string define)
        {
            this.name = name;
            this.expression = expression;
            this.define = define;
        }

        [SerializeField]
        private string name;

        [SerializeField]
        private string expression;

        [SerializeField]
        private string define;

        bool IEquatable<VersionDefine>.Equals(VersionDefine other)
        {
            return name.Equals(other.name) &&
                expression.Equals(other.expression) &&
                define.Equals(other.define);
        }
    }
#endif // UNITY_2019_3_OR_NEWER
}
