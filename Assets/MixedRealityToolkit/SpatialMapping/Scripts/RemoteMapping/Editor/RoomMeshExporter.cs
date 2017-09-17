using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using HoloToolkit.Unity.SpatialMapping;

namespace HoloToolkit.Unity
{
    public static class RoomMeshExporter
    {
        private const string ExportDirectoryKey = "_ExportDirectory";
        private const string ExportDirectoryDefault = "MeshExport";
        private const string ExportDialogErrorTitle = "Export Error";
        private const string WavefrontFileExtension = ".obj";

        public static string ExportDirectory
        {
            get
            {
                return EditorPrefsUtility.GetEditorPref(ExportDirectoryKey, ExportDirectoryDefault);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = ExportDirectoryDefault;
                }

                EditorPrefsUtility.SetEditorPref(ExportDirectoryKey, value);
            }
        }

        private static bool MakeExportDirectory()
        {
            try
            {
                Directory.CreateDirectory(ExportDirectory);
            }
            catch
            {
                return false;
            }

            return true;
        }

        [MenuItem("HoloToolkit/Export/Export Room (.room) To Wavefront (.obj)...")]
        public static void ExportRoomToWavefront()
        {
            string selectedFile = EditorUtility.OpenFilePanelWithFilters("Select Room File", MeshSaver.MeshFolderName, new string[] { "Room", "room" });
            if (string.IsNullOrEmpty(selectedFile))
            {
                return;
            }

            string fileName = Path.GetFileNameWithoutExtension(selectedFile);
            IEnumerable<Mesh> meshes = null;
            try
            {
                meshes = MeshSaver.Load(fileName);
            }
            catch
            {
                // Handling exceptions, and null returned by MeshSaver.Load, by checking if meshes
                // is still null below.
            }

            if (meshes == null)
            {
                EditorUtility.DisplayDialog(ExportDialogErrorTitle, "Unable to parse selected file.", "Ok");
                return;
            }

            SaveMeshesToWavefront(fileName, meshes);

            // Open the location on where the mesh was saved.
            System.Diagnostics.Process.Start(ExportDirectory);
        }

        [MenuItem("HoloToolkit/Export/Export Selection To Wavefront (.obj)")]
        public static void ExportSelectionToWavefront()
        {
            Transform[] selectedTransforms = Selection.transforms;
            if (selectedTransforms.Length <= 0)
            {
                EditorUtility.DisplayDialog(ExportDialogErrorTitle, "Please select GameObject(s) within the scene that you want saved.", "OK");
                return;
            }

            List<MeshFilter> meshFilters = new List<MeshFilter>(selectedTransforms.Length);
            for (int i = 0, iLength = selectedTransforms.Length; i < iLength; ++i)
            {
                meshFilters.AddRange(selectedTransforms[i].GetComponentsInChildren<MeshFilter>());
            }

            if (meshFilters.Count == 0)
            {
                EditorUtility.DisplayDialog(ExportDialogErrorTitle, "Nothing selected contains a MeshFilter.", "Ok");
                return;
            }

            SaveMeshFiltersToWavefront("Selection", meshFilters);

            // Open the location on where the mesh was saved.
            System.Diagnostics.Process.Start(ExportDirectory);
        }

        /// <summary>
        /// Saves meshes without any modifications during serialization.
        /// </summary>
        /// <param name="fileName">Name of the file, without path and extension.</param>
        public static void SaveMeshesToWavefront(string fileName, IEnumerable<Mesh> meshes)
        {
            if (!MakeExportDirectory())
            {
                EditorUtility.DisplayDialog(ExportDialogErrorTitle, "Failed to create export directory.", "Ok");
                return;
            }

            string filePath = Path.Combine(ExportDirectory, fileName + WavefrontFileExtension);
            using (StreamWriter stream = new StreamWriter(filePath))
            {
                stream.Write(SerializeMeshes(meshes));
            }
        }

        /// <summary>
        /// Transform all vertices and normals of the meshes into world space during serialization.
        /// </summary>
        /// <param name="fileName">Name of the file, without path and extension.</param>
        public static void SaveMeshFiltersToWavefront(string fileName, IEnumerable<MeshFilter> meshes)
        {
            if (!MakeExportDirectory())
            {
                EditorUtility.DisplayDialog(ExportDialogErrorTitle, "Failed to create export directory.", "Ok");
                return;
            }

            string filePath = Path.Combine(ExportDirectory, fileName + WavefrontFileExtension);
            using (StreamWriter stream = new StreamWriter(filePath))
            {
                stream.Write(SerializeMeshFilters(meshes));
            }
        }

        private static string SerializeMeshes(IEnumerable<Mesh> meshes)
        {
            StringWriter stream = new StringWriter();
            int offset = 0;
            foreach (var mesh in meshes)
            {
                SerializeMesh(mesh, stream, ref offset);
            }
            return stream.ToString();
        }

        private static string SerializeMeshFilters(IEnumerable<MeshFilter> meshes)
        {
            StringWriter stream = new StringWriter();
            int offset = 0;
            foreach (var mesh in meshes)
            {
                SerializeMeshFilter(mesh, stream, ref offset);
            }
            return stream.ToString();
        }

        /// <summary>
        /// Write single mesh to the stream passed in.
        /// </summary>
        /// <param name="meshFilter">Mesh to be serialized.</param>
        /// <param name="stream">Stream to write the mesh into.</param>
        /// <param name="offset">Index offset for handling multiple meshes in a single stream.</param>
        private static void SerializeMesh(Mesh mesh, TextWriter stream, ref int offset)
        {
            // Write vertices to .obj file. Need to make sure the points are transformed so everything is at a single origin.
            foreach (Vector3 vertex in mesh.vertices)
            {
                stream.WriteLine(string.Format("v {0} {1} {2}", -vertex.x, vertex.y, vertex.z));
            }

            // Write normals. Need to transform the direction.
            foreach (Vector3 normal in mesh.normals)
            {
                stream.WriteLine(string.Format("vn {0} {1} {2}", normal.x, normal.y, normal.z));
            }

            // Write indices.
            for (int s = 0, sLength = mesh.subMeshCount; s < sLength; ++s)
            {
                int[] indices = mesh.GetTriangles(s);
                for (int i = 0, iLength = indices.Length - indices.Length % 3; i < iLength; i += 3)
                {
                    // Format is "vertex index / material index / normal index"
                    stream.WriteLine(string.Format("f {0}//{0} {1}//{1} {2}//{2}",
                        indices[i + 2] + 1 + offset,
                        indices[i + 1] + 1 + offset,
                        indices[i + 0] + 1 + offset));
                }
            }

            offset += mesh.vertices.Length;
        }

        /// <summary>
        /// Write single, transformed, mesh to the stream passed in.
        /// </summary>
        /// <param name="meshFilter">Contains the mesh to be transformed and serialized.</param>
        /// <param name="stream">Stream to write the transformed mesh into.</param>
        /// <param name="offset">Index offset for handling multiple meshes in a single stream.</param>
        private static void SerializeMeshFilter(MeshFilter meshFilter, TextWriter stream, ref int offset)
        {
            Mesh mesh = meshFilter.sharedMesh;

            // Write vertices to .obj file. Need to make sure the points are transformed so everything is at a single origin.
            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 pos = meshFilter.transform.TransformPoint(vertex);
                stream.WriteLine(string.Format("v {0} {1} {2}", -pos.x, pos.y, pos.z));
            }

            // Write normals. Need to transform the direction.
            foreach (Vector3 meshNormal in mesh.normals)
            {
                Vector3 normal = meshFilter.transform.TransformDirection(meshNormal);
                stream.WriteLine(string.Format("vn {0} {1} {2}", normal.x, normal.y, normal.z));
            }

            // Write indices.
            for (int s = 0, sLength = mesh.subMeshCount; s < sLength; ++s)
            {
                int[] indices = mesh.GetTriangles(s);
                for (int i = 0, iLength = indices.Length - indices.Length % 3; i < iLength; i += 3)
                {
                    // Format is "vertex index / material index / normal index"
                    stream.WriteLine(string.Format("f {0}//{0} {1}//{1} {2}//{2}",
                        indices[i + 0] + 1 + offset,
                        indices[i + 1] + 1 + offset,
                        indices[i + 2] + 1 + offset));
                }
            }

            offset += mesh.vertices.Length;
        }
    }
}