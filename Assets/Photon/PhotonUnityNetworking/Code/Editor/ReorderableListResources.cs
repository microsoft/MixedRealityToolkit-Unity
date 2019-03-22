// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;

using System;

namespace Photon.Pun
{

    /// <summary>
    /// Resources to assist with reorderable list control.
    /// </summary>
    internal static class ReorderableListResources
    {

        static ReorderableListResources()
        {
            GenerateSpecialTextures();
            LoadResourceAssets();
        }

        #region Texture Resources

        private enum ResourceName
        {
            add_button = 0,
            add_button_active,
            container_background,
            grab_handle,
            remove_button,
            remove_button_active,
            title_background,
        }

        /// <summary>
        /// Resource assets for light skin.
        /// </summary>
        /// <remarks>
        /// <para>Resource assets are PNG images which have been encoded using a base-64
        /// string so that actual asset files are not necessary.</para>
        /// </remarks>
        private static string[] s_LightSkin = {
            "iVBORw0KGgoAAAANSUhEUgAAAB4AAAAQCAYAAAABOs/SAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAZdEVYdFNvZnR3YXJlAEFkb2JlIEltYWdlUmVhZHlxyWU8AAAAW0lEQVRIS+3NywnAQAhF0anI4mzVCmzBBl7QEBgGE5JFhBAXd+OHM5gZZgYRKcktNxu+HRFF2e6qhtOjtQM7K/tZ+xY89wSbazg9eqOfw6oag4rcChjY8coAjA2l1RxFDY8IFAAAAABJRU5ErkJggg==",
            "iVBORw0KGgoAAAANSUhEUgAAAB4AAAAQCAYAAAABOs/SAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAGlJREFUeNpiFBER+f/jxw8GNjY2BnqAX79+MXBwcDAwMQwQGHoWnzp1CoxHjo8pBSykBi8+MTMzs2HmY2QfwXxKii9HExdZgNwgHuFB/efPH7pZCLOL8f///wyioqL/6enbL1++MAIEGABvGSLA+9GPZwAAAABJRU5ErkJggg==",
            "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAZdEVYdFNvZnR3YXJlAEFkb2JlIEltYWdlUmVhZHlxyWU8AAAAMElEQVQYV2P4//8/Q1FR0X8YBvHBAp8+ffp/+fJlMA3igwUfPnwIFgDRYEFM7f8ZAG1EOYL9INrfAAAAAElFTkSuQmCC",
            "iVBORw0KGgoAAAANSUhEUgAAAAkAAAAFCAYAAACXU8ZrAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAZdEVYdFNvZnR3YXJlAEFkb2JlIEltYWdlUmVhZHlxyWU8AAAAIElEQVQYV2P49OnTf0KYobCw8D8hzPD/P2FMLesK/wMAs5yJpK+6aN4AAAAASUVORK5CYII=",
            "iVBORw0KGgoAAAANSUhEUgAAAAgAAAACCAIAAADq9gq6AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAABVJREFUeNpiVFZWZsAGmBhwAIAAAwAURgBt4C03ZwAAAABJRU5ErkJggg==",
            "iVBORw0KGgoAAAANSUhEUgAAAAgAAAACCAIAAADq9gq6AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAABVJREFUeNpivHPnDgM2wMSAAwAEGAB8VgKYlvqkBwAAAABJRU5ErkJggg==",
            "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAEFJREFUeNpi/P//P0NxcfF/BgRgZP78+fN/VVVVhpCQEAZjY2OGs2fPNrCApBwdHRkePHgAVwoWnDVrFgMyAAgwAAt4E1dCq1obAAAAAElFTkSuQmCC"
        };
        /// <summary>
        /// Resource assets for dark skin.
        /// </summary>
        /// <remarks>
        /// <para>Resource assets are PNG images which have been encoded using a base-64
        /// string so that actual asset files are not necessary.</para>
        /// </remarks>
        private static string[] s_DarkSkin = {
            "iVBORw0KGgoAAAANSUhEUgAAAB4AAAAQCAYAAAABOs/SAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAIBJREFUeNpiVFZW/u/i4sLw4sULBnoACQkJhj179jAwMQwQGHoWl5aWgvHI8TGlgIXU4MUn1t3dPcx8HB8fD2cvXLgQQ0xHR4c2FmMzmBTLhl5QYwt2cn1MtsXkWjg4gvrt27fgWoMeAGQXCDD+//+fQUVF5T89fXvnzh1GgAADAFmSI1Ed3FqgAAAAAElFTkSuQmCC",
            "iVBORw0KGgoAAAANSUhEUgAAAB4AAAAQCAYAAAABOs/SAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAHlJREFUeNpiFBER+f/jxw8GNjY2BnqAX79+MXBwcDAwMQwQGHoWv3nzBoxHjo8pBSykBi8+MWAOGWY+5uLigrO/ffuGIYbMppnF5Fg2tFM1yKfk+pbkoKZGEA+OVP3nzx+6WQizi/H///8MoqKi/+np2y9fvjACBBgAoTYjgvihfz0AAAAASUVORK5CYII=",
            "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAD1JREFUeNpi/P//P4OKisp/Bii4c+cOIwtIwMXFheHFixcMEhISYAVMINm3b9+CBUA0CDCiazc0NGQECDAAdH0YelA27kgAAAAASUVORK5CYII=",
            "iVBORw0KGgoAAAANSUhEUgAAAAkAAAAFCAYAAACXU8ZrAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACRJREFUeNpizM3N/c9AADAqKysTVMTi5eXFSFAREFPHOoAAAwBCfwcAO8g48QAAAABJRU5ErkJggg==",
            "iVBORw0KGgoAAAANSUhEUgAAAAgAAAAECAYAAACzzX7wAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACJJREFUeNpi/P//PwM+wHL06FG8KpgYCABGZWVlvCYABBgA7/sHvGw+cz8AAAAASUVORK5CYII=",
            "iVBORw0KGgoAAAANSUhEUgAAAAgAAAAECAYAAACzzX7wAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACBJREFUeNpi/P//PwM+wPKfgAomBgKAhYuLC68CgAADAAxjByOjCHIRAAAAAElFTkSuQmCC",
            "iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAADtJREFUeNpi/P//P4OKisp/Bii4c+cOIwtIQE9Pj+HLly9gQRCfBcQACbx69QqmmAEseO/ePQZkABBgAD04FXsmmijSAAAAAElFTkSuQmCC"
        };

        /// <summary>
        /// Gets light or dark texture "add_button.png".
        /// </summary>
        public static Texture2D texAddButton
        {
            get { return s_Cached[ (int)ResourceName.add_button ]; }
        }
        /// <summary>
        /// Gets light or dark texture "add_button_active.png".
        /// </summary>
        public static Texture2D texAddButtonActive
        {
            get { return s_Cached[ (int)ResourceName.add_button_active ]; }
        }
        /// <summary>
        /// Gets light or dark texture "container_background.png".
        /// </summary>
        public static Texture2D texContainerBackground
        {
            get { return s_Cached[ (int)ResourceName.container_background ]; }
        }
        /// <summary>
        /// Gets light or dark texture "grab_handle.png".
        /// </summary>
        public static Texture2D texGrabHandle
        {
            get { return s_Cached[ (int)ResourceName.grab_handle ]; }
        }
        /// <summary>
        /// Gets light or dark texture "remove_button.png".
        /// </summary>
        public static Texture2D texRemoveButton
        {
            get { return s_Cached[ (int)ResourceName.remove_button ]; }
        }
        /// <summary>
        /// Gets light or dark texture "remove_button_active.png".
        /// </summary>
        public static Texture2D texRemoveButtonActive
        {
            get { return s_Cached[ (int)ResourceName.remove_button_active ]; }
        }
        /// <summary>
        /// Gets light or dark texture "title_background.png".
        /// </summary>
        public static Texture2D texTitleBackground
        {
            get { return s_Cached[ (int)ResourceName.title_background ]; }
        }

        #endregion

        #region Generated Resources

        public static Texture2D texItemSplitter { get; private set; }

        /// <summary>
        /// Generate special textures.
        /// </summary>
        private static void GenerateSpecialTextures()
        {
            var splitterColor = EditorGUIUtility.isProSkin
                ? new Color( 1f, 1f, 1f, 0.14f )
                : new Color( 0.59f, 0.59f, 0.59f, 0.55f )
                ;
            texItemSplitter = CreatePixelTexture( "(Generated) Item Splitter", splitterColor );
        }

        /// <summary>
        /// Create 1x1 pixel texture of specified color.
        /// </summary>
        /// <param name="name">Name for texture object.</param>
        /// <param name="color">Pixel color.</param>
        /// <returns>
        /// The new <c>Texture2D</c> instance.
        /// </returns>
        public static Texture2D CreatePixelTexture( string name, Color color )
        {
            var tex = new Texture2D( 1, 1, TextureFormat.ARGB32, false, true );
            tex.name = name;
            tex.hideFlags = HideFlags.HideAndDontSave;
            tex.filterMode = FilterMode.Point;
            tex.SetPixel( 0, 0, color );
            tex.Apply();
            return tex;
        }

        #endregion

        #region Load PNG from Base-64 Encoded String

        private static Texture2D[] s_Cached;

        /// <summary>
        /// Read textures from base-64 encoded strings. Automatically selects assets based
        /// upon whether the light or dark (pro) skin is active.
        /// </summary>
        private static void LoadResourceAssets()
        {
            var skin = EditorGUIUtility.isProSkin ? s_DarkSkin : s_LightSkin;
            s_Cached = new Texture2D[ skin.Length ];

            for( int i = 0; i < s_Cached.Length; ++i )
            {
                // Get image data (PNG) from base64 encoded strings.
                byte[] imageData = Convert.FromBase64String( skin[ i ] );

                // Gather image size from image data.
                int texWidth, texHeight;
                GetImageSize( imageData, out texWidth, out texHeight );

                // Generate texture asset.
                var tex = new Texture2D( texWidth, texHeight, TextureFormat.ARGB32, false, true );
                tex.hideFlags = HideFlags.HideAndDontSave;
                tex.name = "(Generated) ReorderableList:" + i;
                tex.filterMode = FilterMode.Point;
                tex.LoadImage( imageData );

                s_Cached[ i ] = tex;
            }

            s_LightSkin = null;
            s_DarkSkin = null;
        }

        /// <summary>
        /// Read width and height if PNG file in pixels.
        /// </summary>
        /// <param name="imageData">PNG image data.</param>
        /// <param name="width">Width of image in pixels.</param>
        /// <param name="height">Height of image in pixels.</param>
        private static void GetImageSize( byte[] imageData, out int width, out int height )
        {
            width = ReadInt( imageData, 3 + 15 );
            height = ReadInt( imageData, 3 + 15 + 2 + 2 );
        }

        private static int ReadInt( byte[] imageData, int offset )
        {
            return ( imageData[ offset ] << 8 ) | imageData[ offset + 1 ];
        }

        #endregion

        #region GUI Helper
        private static GUIStyle s_TempStyle = new GUIStyle();

        /// <summary>
        /// Draw texture using <see cref="GUIStyle"/> to workaround bug in Unity where
        /// <see cref="GUI.DrawTexture"/> flickers when embedded inside a property drawer.
        /// </summary>
        /// <param name="position">Position of which to draw texture in space of GUI.</param>
        /// <param name="texture">Texture.</param>
        public static void DrawTexture( Rect position, Texture2D texture )
        {
            if( Event.current.type != EventType.Repaint )
                return;

            s_TempStyle.normal.background = texture;

            s_TempStyle.Draw( position, GUIContent.none, false, false, false, false );
        }
        #endregion

    }

}