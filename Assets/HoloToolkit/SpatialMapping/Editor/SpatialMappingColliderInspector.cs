// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SpatialMappingCollider))]
public class SpatialMappingColliderInspector : Editor
{
    /// <summary>
    /// The SpatialMappingCollider we are adjusting the settings for
    /// </summary>
    private SpatialMappingCollider _collider;

    void OnEnable()
    {
        _collider = target as SpatialMappingCollider;
    }

    public override void OnInspectorGUI()
    {
        _collider.EnableCollisions = EditorGUILayout.Toggle(new GUIContent("Enable Collisions", "Toggles whether colliders are enabled on mesh"), _collider.EnableCollisions);
        _collider.PhysicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(new GUIContent("Physic Material", "Material describing the physical properties of the mesh"), _collider.PhysicMaterial, typeof(PhysicMaterial), false);
        _collider.MeshLayer = EditorGUILayout.LayerField(new GUIContent("Physics Layer", "The layer to be used for raycasts into the mesh"), _collider.MeshLayer);

        _collider.FreezeMeshUpdates = EditorGUILayout.Toggle(new GUIContent("Freeze Mesh Updates", ""), _collider.FreezeMeshUpdates);
        GUIContent[] boundsChoices = { new GUIContent("Bounding Box"), new GUIContent("Sphere") };
        int currentChoice = _collider.UseSphereBounds ? 1 : 0;
        int choice = EditorGUILayout.Popup(new GUIContent("Bounding Volume", "The shape of the bounds for the observed region"), currentChoice, boundsChoices);
        switch (choice)
        {
            case 0:
                _collider.UseSphereBounds = false;
                _collider.Extents = EditorGUILayout.Vector3Field(new GUIContent("Extents", "The extents of the observation volume"), _collider.Extents);
                break;
            case 1:
                _collider.UseSphereBounds = true;
                _collider.SphereRadius = EditorGUILayout.FloatField(new GUIContent("Radius", "The radius of the observation sphere volume"), _collider.SphereRadius);
                break;
            default:
                throw new System.Exception("Unexpected option!");
        }

        GUIContent[] lodChoices = { new GUIContent("High"), new GUIContent("Medium"), new GUIContent("Low") };
        _collider.LevelOfDetail = (SMBaseAbstract.MeshLevelOfDetail)EditorGUILayout.Popup(new GUIContent("Level of Detail", "The quality of the resulting mesh. Lower is better for performance and physics while higher will look more accurate and is better for rendering"), (int)_collider.LevelOfDetail, lodChoices);
        _collider.TimeBetweenUpdates = EditorGUILayout.FloatField(new GUIContent("Time Between Updates", "How long to wait (in seconds) between Spatial Mapping updates"), _collider.TimeBetweenUpdates);

    }
}
#endif
