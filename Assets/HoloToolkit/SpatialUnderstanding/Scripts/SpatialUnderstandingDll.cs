// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Encapsulates the primary dll functions, including marshalling helper functions.
    /// The dll functions are organized into four parts - in this behavior, 
    /// SpatialUnderstandingDllTopology, SpatialUnderstandingDllShapes, and
    /// SpatialUnderstandingDllObjectPlacement. The scan flow, raycast, and alignment
    /// functions are included in this class.
    /// </summary>
    public class SpatialUnderstandingDll
    {
        /// <summary>
        /// Representation of the mesh data to be passed to the understanding dll.
        /// Used by SpatialUnderstandingSourceMesh to store local copies of the mesh data.
        /// </summary>
        public struct MeshData
        {
            public int MeshID;
            public int LastUpdateID;
            public Matrix4x4 Transform;
            public Vector3[] Verts;
            public Vector3[] Normals;
            public Int32[] Indices;

            public MeshData(MeshFilter meshFilter)
            {
                MeshID = 0;
                LastUpdateID = 0;

                Transform = meshFilter.transform.localToWorldMatrix;
                Verts = meshFilter.sharedMesh.vertices;
                Normals = meshFilter.sharedMesh.normals;
                Indices = meshFilter.sharedMesh.triangles;
            }
            public void CopyFrom(MeshFilter meshFilter, int meshID = 0, int lastUpdateID = 0)
            {
                MeshID = meshID;
                LastUpdateID = lastUpdateID;

                if (meshFilter != null)
                {
                    Transform = meshFilter.transform.localToWorldMatrix;

                    // Note that we are assuming that Unity's getters for vertices/normals/triangles make
                    // copies of the array.  As of unity 5.4 this assumption is correct.
                    Verts = meshFilter.sharedMesh.vertices;
                    Normals = meshFilter.sharedMesh.normals;
                    Indices = meshFilter.sharedMesh.triangles;
                }
            }
        }

        // Privates
        private Imports.MeshData[] reusedMeshesForMarshalling = null;
        private List<GCHandle> reusedPinnedMemoryHandles = new List<GCHandle>();

        private Imports.RaycastResult reusedRaycastResult = new Imports.RaycastResult();
        private IntPtr reusedRaycastResultPtr;

        private Imports.PlayspaceStats reusedPlayspaceStats = new Imports.PlayspaceStats();
        private IntPtr reusedPlayspaceStatsPtr;

        private Imports.PlayspaceAlignment reusedPlayspaceAlignment = new Imports.PlayspaceAlignment();
        private IntPtr reusedPlayspaceAlignmentPtr;

        private SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult reusedObjectPlacementResult = new SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult();
        private IntPtr reusedObjectPlacementResultPtr;

        /// <summary>
        /// Pins the specified object so that the backing memory can not be relocated, adds the pinned
        /// memory handle to the tracking list, and then returns that address of the pinned memory so
        /// that it can be passed into the DLL to be access directly from native code.
        /// </summary>
        public IntPtr PinObject(System.Object obj)
        {
            GCHandle h = GCHandle.Alloc(obj, GCHandleType.Pinned);
            reusedPinnedMemoryHandles.Add(h);
            return h.AddrOfPinnedObject();
        }

        /// <summary>
        /// Pins the string, converting to the format expected by the dll. See PinObject for
        /// additional details.
        /// </summary>
        public IntPtr PinString(string str)
        {
            byte[] obj = System.Text.Encoding.ASCII.GetBytes(str);
            GCHandle h = GCHandle.Alloc(obj, GCHandleType.Pinned);
            reusedPinnedMemoryHandles.Add(h);
            return h.AddrOfPinnedObject();
        }

        /// <summary>
        /// Unpins all of the memory previously pinned by calls to PinObject().
        /// </summary>
        public void UnpinAllObjects()
        {
            for (int i = 0; i < reusedPinnedMemoryHandles.Count; ++i)
            {
                reusedPinnedMemoryHandles[i].Free();
            }
            reusedPinnedMemoryHandles.Clear();
        }

        /// <summary>
        /// Copies the supplied mesh data into the reusedMeshesForMarhsalling array. All managed arrays
        /// are pinned so that the marshalling only needs to pass a pointer and the native code can
        /// reference the memory in place without needing the marshaller to create a complete copy of
        /// the data.
        /// </summary>
        public IntPtr PinMeshDataForMarshalling(List<MeshData> meshes)
        {
            // if we have a big enough array reuse it, otherwise create new
            if (reusedMeshesForMarshalling == null || reusedMeshesForMarshalling.Length < meshes.Count)
            {
                reusedMeshesForMarshalling = new Imports.MeshData[meshes.Count];
            }

            for (int i = 0; i < meshes.Count; ++i)
            {
                IntPtr pinnedVerts = (meshes[i].Verts != null) && (meshes[i].Verts.Length > 0) ? PinObject(meshes[i].Verts) : IntPtr.Zero;
                IntPtr pinnedNormals = (meshes[i].Verts != null) && (meshes[i].Verts.Length > 0) ? PinObject(meshes[i].Normals) : IntPtr.Zero;
                IntPtr pinnedIndices = (meshes[i].Indices != null) && (meshes[i].Indices.Length > 0) ? PinObject(meshes[i].Indices) : IntPtr.Zero;
                reusedMeshesForMarshalling[i] = new Imports.MeshData()
                {
                    meshID = meshes[i].MeshID,
                    lastUpdateID = meshes[i].LastUpdateID,
                    transform = meshes[i].Transform,
                    vertCount = (meshes[i].Verts != null) ? meshes[i].Verts.Length : 0,
                    indexCount = (meshes[i].Indices != null) ? meshes[i].Indices.Length : 0,
                    verts = pinnedVerts,
                    normals = pinnedNormals,
                    indices = pinnedIndices,
                };
            }

            return PinObject(reusedMeshesForMarshalling);
        }

        /// <summary>
        /// Reusable raycast result object pointer. Can be used for inline raycast calls.
        /// </summary>
        /// <returns>Raycast result pointer</returns>
        public IntPtr GetStaticRaycastResultPtr()
        {
            if (reusedRaycastResultPtr == IntPtr.Zero)
            {
                GCHandle h = GCHandle.Alloc(reusedRaycastResult, GCHandleType.Pinned);
                reusedRaycastResultPtr = h.AddrOfPinnedObject();
            }
            return reusedRaycastResultPtr;
        }
        /// <summary>
        /// Resuable raycast result object. Can be used for inline raycast calls.
        /// </summary>
        /// <returns>Raycast result structure</returns>
        public Imports.RaycastResult GetStaticRaycastResult()
        {
            return reusedRaycastResult;
        }

        /// <summary>
        /// Resuable playspace statistics pointer. Can be used for inline playspace statistics calls.
        /// </summary>
        /// <returns>playspace statistics pointer</returns>
        public IntPtr GetStaticPlayspaceStatsPtr()
        {
            if (reusedPlayspaceStatsPtr == IntPtr.Zero)
            {
                GCHandle h = GCHandle.Alloc(reusedPlayspaceStats, GCHandleType.Pinned);
                reusedPlayspaceStatsPtr = h.AddrOfPinnedObject();
            }
            return reusedPlayspaceStatsPtr;
        }
        /// <summary>
        /// Resuable playspace statistics. Can be used for inline playspace statistics calls.
        /// </summary>
        /// <returns>playspace statistics structure</returns>
        public Imports.PlayspaceStats GetStaticPlayspaceStats()
        {
            return reusedPlayspaceStats;
        }

        /// <summary>
        /// Resuable playspace alignment pointer. Can be used for inline playspace alignment query calls.
        /// </summary>
        /// <returns>playspace alignment pointer</returns>
        public IntPtr GetStaticPlayspaceAlignmentPtr()
        {
            if (reusedPlayspaceAlignmentPtr == IntPtr.Zero)
            {
                GCHandle h = GCHandle.Alloc(reusedPlayspaceAlignment, GCHandleType.Pinned);
                reusedPlayspaceAlignmentPtr = h.AddrOfPinnedObject();
            }
            return reusedPlayspaceAlignmentPtr;
        }
        /// <summary>
        /// Resuable playspace alignment. Can be used for inline playspace alignment query calls.
        /// </summary>
        /// <returns>playspace alignment structure</returns>
        public Imports.PlayspaceAlignment GetStaticPlayspaceAlignment()
        {
            return reusedPlayspaceAlignment;
        }

        /// <summary>
        /// Resuable object placement results pointer. Can be used for inline object placement queries.
        /// </summary>
        /// <returns>Object placement result pointer</returns>
        public IntPtr GetStaticObjectPlacementResultPtr()
        {
            if (reusedObjectPlacementResultPtr == IntPtr.Zero)
            {
                GCHandle h = GCHandle.Alloc(reusedObjectPlacementResult, GCHandleType.Pinned);
                reusedObjectPlacementResultPtr = h.AddrOfPinnedObject();
            }
            return reusedObjectPlacementResultPtr;
        }
        /// <summary>
        /// Resuable object placement results. Can be used for inline object placement queries.
        /// </summary>
        /// <returns>Object placement result structure</returns>
        public SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult GetStaticObjectPlacementResult()
        {
            return reusedObjectPlacementResult;
        }
        
        /// <summary>
        /// Marshals BoundedPlane data returned from a DLL API call into a managed BoundedPlane array
        /// and then frees the memory that was allocated within the DLL.
        /// </summary>
        /// <remarks>Disabling warning 618 when calling Marshal.SizeOf(), because
        /// Unity does not support .Net 4.5.1+ for using the preferred Marshal.SizeOf(T) method."/>, </remarks>
        public T[] MarshalArrayFromIntPtr<T>(IntPtr outArray, int count)
        {
            T[] resultArray = new T[count];
#pragma warning disable 618
            int structSize = Marshal.SizeOf(typeof(T));
#pragma warning restore 618
            IntPtr current = outArray;

            try
            {
                for (int i = 0; i < count; i++)
                {
#pragma warning disable 618
                    resultArray[i] = (T)Marshal.PtrToStructure(current, typeof(T));
#pragma warning restore 618
                    current = (IntPtr)((long)current + structSize);
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(outArray);
            }

            return resultArray;
        }

        public class Imports
        {
            /// <summary>
            /// Mesh input data passed to the dll
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MeshData
            {
                public int meshID;
                public int lastUpdateID;
                public Matrix4x4 transform;
                public Int32 vertCount;
                public Int32 indexCount;
                public IntPtr verts;
                public IntPtr normals;
                public IntPtr indices;
            };
            /// <summary>
            /// Playspace statistics for querying scanning progress
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public class PlayspaceStats
            {
                public int IsWorkingOnStats;				// 0 if still working on creating the stats

                public float HorizSurfaceArea;              // In m2 : All horizontal faces UP between Ground – 0.15 and Ground + 1.f (include Ground and convenient horiz surface)
                public float TotalSurfaceArea;              // In m2 : All !
                public float UpSurfaceArea;                 // In m2 : All horizontal faces UP (no constraint => including ground)
                public float DownSurfaceArea;               // In m2 : All horizontal faces DOWN (no constraint => including ceiling)
                public float WallSurfaceArea;               // In m2 : All Vertical faces (not only walls)
                public float VirtualCeilingSurfaceArea;     // In m2 : estimation of surface of virtual Ceiling.
                public float VirtualWallSurfaceArea;        // In m2 : estimation of surface of virtual Walls.

                public int NumFloor;                        // List of Area of each Floor surface (contains count)
                public int NumCeiling;                      // List of Area of each Ceiling surface (contains count)
                public int NumWall_XNeg;                    // List of Area of each Wall XNeg surface (contains count)
                public int NumWall_XPos;                    // List of Area of each Wall XPos surface (contains count)
                public int NumWall_ZNeg;                    // List of Area of each Wall ZNeg surface (contains count)
                public int NumWall_ZPos;                    // List of Area of each Wall ZPos surface (contains count)
                public int NumPlatform;                     // List of Area of each Horizontal not Floor surface (contains count)

                public int CellCount_IsPaintMode;           // Number paint cells (could deduce surface of painted area) => 8cm x 8cm cell
                public int CellCount_IsSeenQualtiy_None;    // Number of not seen cells => 8cm x 8cm cell
                public int CellCount_IsSeenQualtiy_Seen;    // Number of seen cells => 8cm x 8cm cell
                public int CellCount_IsSeenQualtiy_Good;    // Number of seen cells good quality => 8cm x 8cm cell
            };
            /// <summary>
            /// Playspace alignment results. Reports internal alignment of room in Unity space and basic alignment of the room.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public class PlayspaceAlignment
            {
                public Vector3 Center;
                public Vector3 HalfDims;
                public Vector3 BasisX;
                public Vector3 BasisY;
                public Vector3 BasisZ;
                public float FloorYValue;
                public float CeilingYValue;
            };
            /// <summary>
            /// Result structure returns from a raycast call.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public class RaycastResult
            {
                public enum SurfaceTypes
                {
                    Invalid,            // If no intersection
                    Other,
                    Floor,
                    FloorLike,          // Not part of the floor topology, but close to the floor and looks like the floor
                    Platform,			// Horizontal platform between the ground and the ceiling
                    Ceiling,
                    WallExternal,
                    WallLike,           // Not part of the external wall surface
                };
                public SurfaceTypes SurfaceType;
                float SurfaceArea;		// Zero if unknown (not part of the topology analysis)
                public Vector3 IntersectPoint;
                public Vector3 IntersectNormal;
            };

            // Functions
            /// <summary>
            /// Initialize the spatial understanding dll. Function must be called
            /// before any other dll function.
            /// </summary>
            /// <returns>Zero if fails, one if success</returns>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern int SpatialUnderstanding_Init();
#else
            public static int SpatialUnderstanding_Init()
            {
                return 0;
            }
#endif
            /// <summary>
            /// Terminate the spatial understanding dll. 
            /// </summary>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern void SpatialUnderstanding_Term();
#else
            public static void SpatialUnderstanding_Term()
            {
            }
#endif

            /// <summary>
            /// Initialize the scanning process.
            /// </summary>
            /// <param name="camPos_X">The user's camera/view position, x value</param>
            /// <param name="camPos_Y">The user's camera/view position, y value</param>
            /// <param name="camPos_Z">The user's camera/view position, z value</param>
            /// <param name="camFwd_X">The user's camera/view unit forward vector, x value</param>
            /// <param name="camFwd_Y">The user's camera/view unit forward vector, y value</param>
            /// <param name="camFwd_Z">The user's camera/view unit forward vector, z value</param>
            /// <param name="camUp_X">The user's camera/view unit up vector, x value</param>
            /// <param name="camUp_Y">The user's camera/view unit up vector, y value</param>
            /// <param name="camUp_Z">The user's camera/view unit up vector, z value</param>
            /// <param name="searchDst">Suggested search distance for playspace center</param>
            /// <param name="optimalSize">Optimal room size. Used to determind the playspace size</param>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern void GeneratePlayspace_InitScan(
                [In] float camPos_X, [In] float camPos_Y, [In] float camPos_Z,
                [In] float camFwd_X, [In] float camFwd_Y, [In] float camFwd_Z,
                [In] float camUp_X, [In] float camUp_Y, [In] float camUp_Z,
                [In] float searchDst, [In] float optimalSize);
#else
            public static void GeneratePlayspace_InitScan(
                [In] float camPos_X, [In] float camPos_Y, [In] float camPos_Z,
                [In] float camFwd_X, [In] float camFwd_Y, [In] float camFwd_Z,
                [In] float camUp_X, [In] float camUp_Y, [In] float camUp_Z,
                [In] float searchDst, [In] float optimalSize)
            {
            }
#endif

            /// <summary>
            /// Update the playspace scanning. Should be called once per frame during scanning.
            /// </summary>
            /// <param name="meshCount">Number of meshes passed in the meshes parameter</param>
            /// <param name="meshes">Array of meshes</param>
            /// <param name="camPos_X">The user's camera/view position, x value</param>
            /// <param name="camPos_Y">The user's camera/view position, y value</param>
            /// <param name="camPos_Z">The user's camera/view position, z value</param>
            /// <param name="camFwd_X">The user's camera/view unit forward vector, x value</param>
            /// <param name="camFwd_Y">The user's camera/view unit forward vector, y value</param>
            /// <param name="camFwd_Z">The user's camera/view unit forward vector, z value</param>
            /// <param name="camUp_X">The user's camera/view unit up vector, x value</param>
            /// <param name="camUp_Y">The user's camera/view unit up vector, y value</param>
            /// <param name="camUp_Z">The user's camera/view unit up vector, z value</param>
            /// <param name="deltaTime">Time since last update</param>
            /// <returns>One if scanning has been finalized, zero if more updates are required.</returns>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern int GeneratePlayspace_UpdateScan(
                [In] int meshCount, [In] IntPtr meshes,
                [In] float camPos_X, [In] float camPos_Y, [In] float camPos_Z,
                [In] float camFwd_X, [In] float camFwd_Y, [In] float camFwd_Z,
                [In] float camUp_X, [In] float camUp_Y, [In] float camUp_Z,
                [In] float deltaTime);
#else
            public static int GeneratePlayspace_UpdateScan(
                [In] int meshCount, [In] IntPtr meshes,
                [In] float camPos_X, [In] float camPos_Y, [In] float camPos_Z,
                [In] float camFwd_X, [In] float camFwd_Y, [In] float camFwd_Z,
                [In] float camUp_X, [In] float camUp_Y, [In] float camUp_Z,
                [In] float deltaTime)
            {
                return 0;
            }
#endif

            /// <summary>
            /// Request scanning that the scanning phase be ended and the playspace
            /// finalized. This should be called once the user is happy with the currently
            /// scanned in playspace.
            /// </summary>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern void GeneratePlayspace_RequestFinish();
#else
            public static void GeneratePlayspace_RequestFinish()
            {
            }
#endif

            /// <summary>
            /// Extracting the mesh is a two step process, the first generates the mesh for extraction & saves it off.
            ///	The caller is able to see vertex counts, etc. so they can allocate the proper amount of memory.
            /// The second call, the caller provides buffers of the appropriate size (or larger), passing in the 
            /// buffer sizes for validation.
            /// </summary>
            /// <param name="vertexCount">Filled in with the number of vertices to be returned in the subsequent extract call</param>
            /// <param name="indexCount">Filled in with the number of indices to be returned in the subsequent extract call</param>
            /// <returns>Zero if fails, one if success</returns>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern int GeneratePlayspace_ExtractMesh_Setup(
                [Out] out int vertexCount,
                [Out] out int indexCount);
#else
            public static int GeneratePlayspace_ExtractMesh_Setup(
                [Out] out int vertexCount,
                [Out] out int indexCount)
            {
                vertexCount = 0;
                indexCount = 0;
                return 0;
            }
#endif

            /// <summary>
            /// Call to receive the dll's custom generated mesh data. Use GeneratePlayspace_ExtractMesh_Setup to
            /// query the minimum size of the vertex positions, normals, and indices.
            /// </summary>
            /// <param name="bufferVertexCount">Size of vericesPos & verticesNormal, in number Vector3 elements in each array</param>
            /// <param name="verticesPos">Array to receive the vertex positions</param>
            /// <param name="verticesNormal">Array to receive vertex normals</param>
            /// <param name="bufferIndexCount">Size of indices, in number of elements</param>
            /// <param name="indices">Array to receive the mesh indices</param>
            /// <returns>Zero if fails, one if success</returns>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern int GeneratePlayspace_ExtractMesh_Extract(
                [In] int bufferVertexCount,
                [In] IntPtr verticesPos,        // (vertexCount) DirectX::XMFLOAT3*
                [In] IntPtr verticesNormal,     // (vertexCount) DirectX::XMFLOAT3*
                [In] int bufferIndexCount,
                [In] IntPtr indices);           // (indexCount) INT32*
#else
            public static int GeneratePlayspace_ExtractMesh_Extract(
                [In] int bufferVertexCount,
                [In] IntPtr verticesPos,        // (vertexCount) DirectX::XMFLOAT3*
                [In] IntPtr verticesNormal,     // (vertexCount) DirectX::XMFLOAT3*
                [In] int bufferIndexCount,
                [In] IntPtr indices)            // (indexCount) INT32*
            {
                return 0;
            }
#endif

            /// <summary>
            /// Query the playspace scan statistics. 
            /// </summary>
            /// <param name="playspaceStats">playspace stats structure to receive the statistics data</param>
            /// <returns>Zero if fails, one if success</returns>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern int QueryPlayspaceStats(
                [In] IntPtr playspaceStats);    // PlayspaceStats
#else
            public static int QueryPlayspaceStats(
                [In] IntPtr playspaceStats)
            {
                return 0;
            }
#endif

            /// <summary>
            /// Query the playspace alignment data. This will not be valid until after scanning is finalized.
            /// </summary>
            /// <param name="playspaceAlignment">playspace alignment structure to receive the alignment data</param>
            /// <returns>Zero if fails, one if success</returns>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern int QueryPlayspaceAlignment(
                [In] IntPtr playspaceAlignment); // PlayspaceAlignment
#else
            public static int QueryPlayspaceAlignment(
                [In] IntPtr playspaceAlignment)
            {
                return 0;
            }
#endif

            /// <summary>
            /// Perform a raycast against the internal world representation of the understanding dll. 
            /// This will not be valid until after scanning is finalized.
            /// </summary>
            /// <param name="rayPos_X">Ray origin, x component</param>
            /// <param name="rayPos_Y">Ray origin, y component</param>
            /// <param name="rayPos_Z">Ray origin, z component</param>
            /// <param name="rayVec_X">Ray direction vector, x component. Length of ray indicates the length of the ray cast query.</param>
            /// <param name="rayVec_Y">Ray direction vector, y component. Length of ray indicates the length of the ray cast query.</param>
            /// <param name="rayVec_Z">Ray direction vector, z component. Length of ray indicates the length of the ray cast query.</param>
            /// <param name="result">Structure to receive the results of the raycast</param>
            /// <returns>Zero if fails or no intersection, one if an intersection is detected</returns>
#if UNITY_METRO && !UNITY_EDITOR
            [DllImport("SpatialUnderstanding")]
            public static extern int PlayspaceRaycast(
                [In] float rayPos_X, [In] float rayPos_Y, [In] float rayPos_Z,
                [In] float rayVec_X, [In] float rayVec_Y, [In] float rayVec_Z,
                [In] IntPtr result);            // RaycastResult
#else
            public static int PlayspaceRaycast(
                [In] float rayPos_X, [In] float rayPos_Y, [In] float rayPos_Z,
                [In] float rayVec_X, [In] float rayVec_Y, [In] float rayVec_Z,
                [In] IntPtr result)
            {
                return 0;
            }
#endif
        }
    }
}