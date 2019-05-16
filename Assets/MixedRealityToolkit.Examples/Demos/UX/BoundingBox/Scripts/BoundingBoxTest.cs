using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoundingBoxTest : InputSystemGlobalListener, IMixedRealitySpeechHandler
{
    BoundingBox bbox;
    private bool signal;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        // verify bbox can be created at scale of 1
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<MeshRenderer>().material.color = Color.black;
        cube.transform.position = Vector3.forward * 5;
        bbox = cube.AddComponent<BoundingBox>();
        bbox.HideElementsInInspector = false;
        bbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateOnStart;
        var mh = cube.AddComponent<ManipulationHandler>();

        Debug.Log("FlattenX");
        yield return WaitForSpeechCommand();
        bbox.FlattenAxis = BoundingBox.FlattenModeType.FlattenX;
        Debug.Log("FlattenY");
        yield return WaitForSpeechCommand();
        bbox.FlattenAxis = BoundingBox.FlattenModeType.FlattenY;
        Debug.Log("FlattenNone");
        yield return WaitForSpeechCommand();
        bbox.FlattenAxis = BoundingBox.FlattenModeType.DoNotFlatten;

        Debug.Log("BoxPadding .2");
        yield return WaitForSpeechCommand();
        bbox.BoxPadding = new Vector3(0.2f, 0.2f, 0.2f);
        Debug.Log("BoxPadding 0");
        yield return WaitForSpeechCommand();
        bbox.BoxPadding = Vector3.zero;

        Debug.Log("scalehandle widget");
        yield return WaitForSpeechCommand();
        bbox.ScaleHandleSize = 0.03f;
        bbox.ScaleHandlePrefab = LoadAsset<GameObject>("MRTK_BoundingBox_ScaleWidget");

        Debug.Log("handles red");
        yield return WaitForSpeechCommand();
        bbox.HandleMaterial = LoadAsset<Material>("MRTK_Standard_Red");

        Debug.Log("BBox material cyan");
        yield return WaitForSpeechCommand();
        bbox.BoxMaterial = LoadAsset<Material>("MRTK_Standard_Cyan");
        Debug.Log("BBox material none");
        yield return WaitForSpeechCommand();
        bbox.BoxMaterial = null;

        Debug.Log("BBox grabbed green");
        yield return WaitForSpeechCommand();
        bbox.BoxGrabbedMaterial = LoadAsset<Material>("MRTK_Standard_Emerald");

        Debug.Log("Wireframe radius 0.1");
        yield return WaitForSpeechCommand();
        bbox.WireframeEdgeRadius = 0.1f;

        Debug.Log("Wireframe shape cylinder");
        yield return WaitForSpeechCommand();
        bbox.WireframeShape = BoundingBox.WireframeType.Cylindrical;
    }

    private T LoadAsset<T>(string s) where T : UnityEngine.Object
    {
        string[] paths = AssetDatabase.FindAssets(s);
        var result = (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[0]), typeof(T));
        return result;
    }

    private IEnumerator WaitForSpeechCommand()
    {
        while (!signal)
        {
            yield return null;
        }
        signal = false;
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData.Command.Keyword.Equals("Select", System.StringComparison.CurrentCultureIgnoreCase))
        {
            signal = true;
        }
    }
}
