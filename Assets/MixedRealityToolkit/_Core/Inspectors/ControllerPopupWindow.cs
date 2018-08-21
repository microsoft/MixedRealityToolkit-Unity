// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    public class ControllerPopupWindow : EditorWindow
    {
        [SerializeField]
        private Texture2D xboxControllerWhite;
        [SerializeField]
        private Texture2D xboxControllerBlack;

        [SerializeField]
        private Texture2D wmrControllerLeftWhite;
        [SerializeField]
        private Texture2D wmrControllerLeftBlack;
        [SerializeField]
        private Texture2D wmrControllerRightWhite;
        [SerializeField]
        private Texture2D wmrControllerRightBlack;

        [SerializeField]
        private Texture2D touchControllerLeftWhite;
        [SerializeField]
        private Texture2D touchControllerLeftBlack;
        [SerializeField]
        private Texture2D touchControllerRightWhite;
        [SerializeField]
        private Texture2D touchControllerRightBlack;

        [SerializeField]
        private Texture2D viveWandControllerLeftWhite;
        [SerializeField]
        private Texture2D viveWandControllerLeftBlack;
        [SerializeField]
        private Texture2D viveWandControllerRightWhite;
        [SerializeField]
        private Texture2D viveWandControllerRightBlack;

        private SupportedControllerType currentControllerType;
        private Handedness currentHandedness;

        private Texture2D currentControllerTexture;

        private void OnFocus()
        {
            #region Xbox Controller

            if (xboxControllerWhite == null)
            {
                xboxControllerWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/XboxController_white.png", typeof(Texture2D));
            }

            if (xboxControllerBlack == null)
            {
                xboxControllerBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/XboxController_black.png", typeof(Texture2D));
            }

            #endregion Xbox Controller

            #region Windows Mixed Reality Controller

            if (wmrControllerLeftWhite == null)
            {
                wmrControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_left_white.png", typeof(Texture2D));
            }

            if (wmrControllerLeftBlack == null)
            {
                wmrControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_left_black.png", typeof(Texture2D));
            }

            if (wmrControllerRightWhite == null)
            {
                wmrControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_right_white.png", typeof(Texture2D));
            }

            if (wmrControllerRightBlack == null)
            {
                wmrControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_right_black.png", typeof(Texture2D));
            }

            #endregion Windows Mixed Reality Controller

            #region Touch Controller

            if (touchControllerLeftWhite == null)
            {
                touchControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_left_white.png", typeof(Texture2D));
            }

            if (touchControllerLeftBlack == null)
            {
                touchControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_left_black.png", typeof(Texture2D));
            }

            if (touchControllerRightWhite == null)
            {
                touchControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_right_white.png", typeof(Texture2D));
            }

            if (touchControllerRightBlack == null)
            {
                touchControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_right_black.png", typeof(Texture2D));
            }

            #endregion Touch Controller

            if (viveWandControllerLeftWhite == null)
            {
                viveWandControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_left_white.png", typeof(Texture2D));
            }

            if (viveWandControllerLeftBlack == null)
            {
                viveWandControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_left_black.png", typeof(Texture2D));
            }

            if (viveWandControllerRightWhite == null)
            {
                viveWandControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_right_white.png", typeof(Texture2D));
            }

            if (viveWandControllerRightBlack == null)
            {
                viveWandControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_right_black.png", typeof(Texture2D));
            }

            switch (currentControllerType)
            {
                case SupportedControllerType.None:
                    break;
                case SupportedControllerType.GenericOpenVR:
                    break;
                case SupportedControllerType.ViveWand:
                    if (currentHandedness == Handedness.Left)
                    {
                        currentControllerTexture = EditorGUIUtility.isProSkin ? viveWandControllerLeftWhite : viveWandControllerLeftBlack;
                    }
                    else if (currentHandedness == Handedness.Right)
                    {
                        currentControllerTexture = EditorGUIUtility.isProSkin ? viveWandControllerRightWhite : viveWandControllerRightBlack;
                    }
                    break;
                case SupportedControllerType.ViveKnuckles:
                    break;
                case SupportedControllerType.OculusTouch:
                    if (currentHandedness == Handedness.Left)
                    {
                        currentControllerTexture = EditorGUIUtility.isProSkin ? touchControllerLeftWhite : touchControllerLeftBlack;
                    }
                    else if (currentHandedness == Handedness.Right)
                    {
                        currentControllerTexture = EditorGUIUtility.isProSkin ? touchControllerRightWhite : touchControllerRightBlack;
                    }
                    break;
                case SupportedControllerType.OculusRemote:
                    break;
                case SupportedControllerType.WindowsMixedReality:
                    if (currentHandedness == Handedness.Left)
                    {
                        currentControllerTexture = EditorGUIUtility.isProSkin ? wmrControllerLeftWhite : wmrControllerLeftBlack;
                    }
                    else if (currentHandedness == Handedness.Right)
                    {
                        currentControllerTexture = EditorGUIUtility.isProSkin ? wmrControllerRightWhite : wmrControllerRightBlack;
                    }
                    break;
                case SupportedControllerType.GenericUnityDevice:
                    break;
                case SupportedControllerType.XboxController:
                    currentControllerTexture = EditorGUIUtility.isProSkin ? xboxControllerWhite : xboxControllerBlack;
                    break;
            }
        }

        public static void Show(SupportedControllerType controllerType, Handedness handedness = Handedness.None)
        {
            var window = (ControllerPopupWindow)GetWindow(typeof(ControllerPopupWindow));
            window.Close();
            window = (ControllerPopupWindow)CreateInstance(typeof(ControllerPopupWindow));
            window.titleContent = new GUIContent($"{controllerType} Input Action Assignment");
            window.currentControllerType = controllerType;
            window.currentHandedness = handedness;
            window.ShowUtility();
            var windowSize = new Vector2(512f, 512f);
            window.maxSize = windowSize;
            window.minSize = windowSize;
            window.CenterOnMainWin();
        }

        private void OnGUI()
        {
            GUILayout.Label(currentControllerTexture, GUILayout.MaxHeight(512f));
        }
    }
}
