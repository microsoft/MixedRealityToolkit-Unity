using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightingCapture))]
public class LightingCaptureEditor : Editor {
	SerializedProperty _mapResolution;
	SerializedProperty _singleStampOnly;
	SerializedProperty _stampFovMultiplier;
	SerializedProperty _stampExpireDistance;
	SerializedProperty _useDirectionalLight;
	SerializedProperty _maxLightColorSaturation;
	SerializedProperty _lightAngleAdjustPerSecond;
	SerializedProperty _cameraOrientation;
	SerializedProperty _probe;
	SerializedProperty _directionalLight;

	private void OnEnable() {
		_mapResolution       = serializedObject.FindProperty("_mapResolution");
		_singleStampOnly     = serializedObject.FindProperty("_singleStampOnly");
		_stampFovMultiplier  = serializedObject.FindProperty("_stampFovMultiplier");
		_stampExpireDistance = serializedObject.FindProperty("_stampExpireDistance");
		_useDirectionalLight = serializedObject.FindProperty("_useDirectionalLight");
		_maxLightColorSaturation = serializedObject.FindProperty("_maxLightColorSaturation");
		_lightAngleAdjustPerSecond = serializedObject.FindProperty("_lightAngleAdjustPerSecond");
		_cameraOrientation   = serializedObject.FindProperty("_cameraOrientation");
		_probe               = serializedObject.FindProperty("_probe");
		_directionalLight    = serializedObject.FindProperty("_directionalLight");
	}
	public override void OnInspectorGUI() {
		EditorGUILayout.LabelField("Quality Presets", EditorStyles.boldLabel);
		int preset = EditorGUILayout.Popup("Apply preset:", 0, new string[] { "...", "Super Low", "Fast", "High", "Very High"});
		if (preset == 1) { // Low
			_mapResolution      .intValue = 128;
			_singleStampOnly    .boolValue = true;
			_stampFovMultiplier .floatValue = 2;
			_stampExpireDistance.floatValue = 0;
		} else if (preset == 2) { // Medium
			_mapResolution      .intValue = 128;
			_singleStampOnly    .boolValue = false;
			_stampFovMultiplier .floatValue = 2f;
			_stampExpireDistance.floatValue = 0;
		} else if (preset == 3) { // Normal
			_mapResolution      .intValue = 128;
			_singleStampOnly    .boolValue = false;
			_stampFovMultiplier .floatValue = 1.5f;
			_stampExpireDistance.floatValue = 0;
		} else if (preset == 4) { // High
			_mapResolution      .intValue = 256;
			_singleStampOnly    .boolValue = false;
			_stampFovMultiplier .floatValue = 1.5f;
			_stampExpireDistance.floatValue = 8;
		}
		
		EditorGUILayout.PropertyField(_mapResolution);
		EditorGUILayout.PropertyField(_singleStampOnly);
		EditorGUILayout.PropertyField(_stampFovMultiplier);
		EditorGUILayout.PropertyField(_stampExpireDistance);
		EditorGUILayout.PropertyField(_useDirectionalLight);
		EditorGUILayout.PropertyField(_maxLightColorSaturation);
		EditorGUILayout.PropertyField(_lightAngleAdjustPerSecond);
		EditorGUILayout.PropertyField(_cameraOrientation);
		EditorGUILayout.PropertyField(_probe);
		EditorGUILayout.PropertyField(_directionalLight);

		serializedObject.ApplyModifiedProperties();
	}
}
