using UnityEngine;
using UnityEditor;
using System.Collections;

using Photon.Pun;

public class PhotonGUI
{
    #region Styles
    static GUIStyle m_DefaultTitleStyle;
    public static GUIStyle DefaultTitleStyle
    {
        get
        {
            if( m_DefaultTitleStyle == null )
            {
                m_DefaultTitleStyle = new GUIStyle();
                m_DefaultTitleStyle.border = new RectOffset( 2, 2, 2, 1 );
                m_DefaultTitleStyle.margin = new RectOffset( 5, 5, 5, 0 );
                m_DefaultTitleStyle.padding = new RectOffset( 5, 5, 0, 0 );
                m_DefaultTitleStyle.alignment = TextAnchor.MiddleLeft;
                m_DefaultTitleStyle.normal.background = ReorderableListResources.texTitleBackground;
                m_DefaultTitleStyle.normal.textColor = EditorGUIUtility.isProSkin
                    ? new Color( 0.8f, 0.8f, 0.8f )
                    : new Color( 0.2f, 0.2f, 0.2f );
            }

            return m_DefaultTitleStyle;
        }
    }

    static GUIStyle m_DefaultContainerStyle;
    public static GUIStyle DefaultContainerStyle
    {
        get
        {
            if( m_DefaultContainerStyle == null )
            {
                m_DefaultContainerStyle = new GUIStyle();
                m_DefaultContainerStyle.border = new RectOffset( 2, 2, 1, 2 );
                m_DefaultContainerStyle.margin = new RectOffset( 5, 5, 5, 5 );
                m_DefaultContainerStyle.padding = new RectOffset( 1, 1, 2, 2 );
                m_DefaultContainerStyle.normal.background = ReorderableListResources.texContainerBackground;
            }

            return m_DefaultContainerStyle;
        }
    }

    static GUIStyle m_DefaultAddButtonStyle;
    public static GUIStyle DefaultAddButtonStyle
    {
        get
        {
            if( m_DefaultAddButtonStyle == null )
            {
                m_DefaultAddButtonStyle = new GUIStyle();
                m_DefaultAddButtonStyle.fixedWidth = 30;
                m_DefaultAddButtonStyle.fixedHeight = 16;
                m_DefaultAddButtonStyle.normal.background = ReorderableListResources.texAddButton;
                m_DefaultAddButtonStyle.active.background = ReorderableListResources.texAddButtonActive;
            }

            return m_DefaultAddButtonStyle;
        }
    }

    static GUIStyle m_DefaultRemoveButtonStyle;
    public static GUIStyle DefaultRemoveButtonStyle
    {
        get
        {
            if( m_DefaultRemoveButtonStyle == null )
            {
                m_DefaultRemoveButtonStyle = new GUIStyle();
                m_DefaultRemoveButtonStyle.fixedWidth = 30;
                m_DefaultRemoveButtonStyle.fixedHeight = 20;
                m_DefaultRemoveButtonStyle.active.background = ReorderableListResources.CreatePixelTexture( "Dark Pixel (List GUI)", new Color32( 18, 18, 18, 255 ) );
                m_DefaultRemoveButtonStyle.imagePosition = ImagePosition.ImageOnly;
                m_DefaultRemoveButtonStyle.alignment = TextAnchor.MiddleCenter;
            }

            return m_DefaultRemoveButtonStyle;
        }
    }

    static GUIStyle m_DefaultContainerRowStyle;
    public static GUIStyle DefaultContainerRowStyle
    {
        get
        {
            if( m_DefaultContainerRowStyle == null )
            {
                m_DefaultContainerRowStyle = new GUIStyle();
                m_DefaultContainerRowStyle.border = new RectOffset( 2, 2, 2, 2 );

                m_DefaultContainerRowStyle.margin = new RectOffset( 5, 5, 5, 5 );
                m_DefaultContainerRowStyle.padding = new RectOffset( 1, 1, 2, 2 );
                m_DefaultContainerRowStyle.normal.background = ReorderableListResources.texContainerBackground;
            }

            return m_DefaultContainerRowStyle;
        }
    }

    static GUIStyle m_FoldoutBold;
    public static GUIStyle FoldoutBold
    {
        get
        {
            if( m_FoldoutBold == null )
            {
                m_FoldoutBold = new GUIStyle( EditorStyles.foldout );
                m_FoldoutBold.fontStyle = FontStyle.Bold;
            }

            return m_FoldoutBold;
        }
    }

    static GUIStyle m_RichLabel;
    public static GUIStyle RichLabel
    {
        get
        {
            if( m_RichLabel == null )
            {
                m_RichLabel = new GUIStyle( GUI.skin.label );
                m_RichLabel.richText = true;
                m_RichLabel.wordWrap = true;
            }

            return m_RichLabel;
        }
    }
    #endregion

    static Texture2D m_HelpIcon;
    public static Texture2D HelpIcon
    {
        get
        {
            if( m_HelpIcon == null )
            {
                m_HelpIcon = AssetDatabase.LoadAssetAtPath( "Assets/Photon Unity Networking/Editor/PhotonNetwork/help.png", typeof( Texture2D ) ) as Texture2D;
            }

            return m_HelpIcon;
        }
    }

    #region Interface
    public static void ContainerHeader( string headline )
    {
        DoContainerHeader( headline, 27, 0 );
    }

    public static bool ContainerHeaderToggle( string headline, bool toggle )
    {
        return DoContainerHeaderToggle( headline, toggle );
    }

    public static bool ContainerHeaderFoldout( string headline, bool foldout )
    {
        return DoContainerHeaderFoldout( headline, foldout );
    }

    public static Rect ContainerBody( float height )
    {
        return DoContainerBody( height );
    }

    public static bool AddButton()
    {
        Rect controlRect = EditorGUILayout.GetControlRect( false, DefaultAddButtonStyle.fixedHeight - 5 );
        controlRect.yMin -= 5;
        controlRect.yMax -= 5;

        Rect addButtonRect = new Rect( controlRect.xMax - DefaultAddButtonStyle.fixedWidth,
                                       controlRect.yMin,
                                       DefaultAddButtonStyle.fixedWidth,
                                       DefaultAddButtonStyle.fixedHeight );

        return GUI.Button( addButtonRect, "", DefaultAddButtonStyle );
    }

    public static void DrawSplitter( Rect position )
    {
        ReorderableListResources.DrawTexture( position, ReorderableListResources.texItemSplitter );
    }

    public static void DrawGizmoOptions(
        Rect position,
        string label,
        SerializedProperty gizmoEnabledProperty,
        SerializedProperty gizmoColorProperty,
        SerializedProperty gizmoTypeProperty,
        SerializedProperty gizmoSizeProperty )
    {
        float height = EditorGUIUtility.singleLineHeight;
        float flexibleWidth = Mathf.Max( 40, position.width - EditorGUIUtility.labelWidth - 20 - 75 - 5 - 40 - 5 );

        Rect labelRect = new Rect( position.xMin, position.yMin, EditorGUIUtility.labelWidth, height );
        GUI.Label( labelRect, label );

        Rect enabledRect = new Rect( labelRect.xMax, labelRect.yMin, 20, height );
        EditorGUI.PropertyField( enabledRect, gizmoEnabledProperty, GUIContent.none );

        bool oldGUIEnabled = GUI.enabled;
        GUI.enabled = gizmoEnabledProperty.boolValue;

        Rect colorRect = new Rect( enabledRect.xMax + 5, labelRect.yMin, 70, height );
        EditorGUI.PropertyField( colorRect, gizmoColorProperty, GUIContent.none );

        Rect typeRect = new Rect( colorRect.xMax + 5, labelRect.yMin, flexibleWidth * 0.7f, height );
        EditorGUI.PropertyField( typeRect, gizmoTypeProperty, GUIContent.none );

        Rect sizeLabelRect = new Rect( typeRect.xMax + 10, labelRect.yMin, 30, height );
        GUI.Label( sizeLabelRect, "Size" );

        Rect sizeRect = new Rect( sizeLabelRect.xMax + 5, labelRect.yMin, flexibleWidth * 0.3f, height );
        EditorGUI.PropertyField( sizeRect, gizmoSizeProperty, GUIContent.none );

        GUI.enabled = oldGUIEnabled;
    }

    #endregion

    #region Implementation
    static Rect DoContainerBody( float height )
    {
        Rect controlRect = EditorGUILayout.GetControlRect( false, height );
        controlRect.yMin -= 3;
        controlRect.yMax -= 2;

        int controlID = GUIUtility.GetControlID( FocusType.Passive, controlRect );

        if( Event.current.type == EventType.Repaint )
        {
            PhotonGUI.DefaultContainerStyle.Draw( controlRect, GUIContent.none, controlID );
        }

        return controlRect;
    }

    static bool DoContainerHeaderToggle( string headline, bool toggle )
    {
        Rect rect = DoContainerHeader( headline, 27, 15 );
        Rect toggleRect = new Rect( rect.xMin + 5, rect.yMin + 5, EditorGUIUtility.labelWidth, rect.height );

        return EditorGUI.Toggle( toggleRect, toggle );
    }


    static bool DoContainerHeaderFoldout( string headline, bool foldout )
    {
        Rect rect = DoContainerHeader( "", 27, 0f );
        Rect foldoutRect = new Rect( rect.xMin + 15, rect.yMin + 5, rect.width, rect.height );

        return EditorGUI.Foldout( foldoutRect, foldout, headline, FoldoutBold );
    }

    static Rect DoContainerHeader( string headline, float height, float contentOffset )
    {
        GUILayout.Space( 5 );
        Rect controlRect = EditorGUILayout.GetControlRect( false, height );

        int controlID = GUIUtility.GetControlID( FocusType.Passive, controlRect );

        if( Event.current.type == EventType.Repaint )
        {
            PhotonGUI.DefaultTitleStyle.Draw( controlRect, GUIContent.none, controlID );

            Rect labelRect = new Rect( controlRect.xMin + 5 + contentOffset, controlRect.yMin + 5, controlRect.width, controlRect.height );
            GUI.Label( labelRect, headline, EditorStyles.boldLabel );
        }

        return controlRect;
    }
    #endregion
}
