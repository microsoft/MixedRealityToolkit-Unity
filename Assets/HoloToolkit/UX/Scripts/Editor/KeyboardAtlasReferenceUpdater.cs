using System.Collections.Generic;
using HoloToolkit.UI.Keyboard;
using UnityEditor;
using UnityEditor.Build;

public class KeyboardAtlasReferenceUpdater : AtlasReferenceUpdater, IPreprocessBuild
{
    protected override IList<string> PrefabPaths
    {
        get
        {
            return new List<string> { "Assets/HoloToolkit/UX/Prefabs/Keyboard.prefab" };
        }
    }

    protected override string AtlasPath
    {
        get
        {
            return "Assets/HoloToolkit/UX/Textures/KeyboardAtlas.spriteatlas";
        }
    }

    public int callbackOrder { get { return int.MaxValue; } }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        UpdateAtlasReferences();
    }
}
