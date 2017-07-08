using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationViewManagerEditButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Text field;

    public void StartEdit()
    {
        StartCoroutine(OpenViewEdit());
    }

    public IEnumerator OpenViewEdit()
    {
        string result = null;

        var avm = this.GetComponent<HoloToolkit.Unity.ApplicationViewManager>();
        yield return avm.OnLaunchXamlView<string>("TestPage", s => result = s); 
        yield return new WaitUntil(() => result != null);
        if (field!=null)
        {
            field.text = result;
        }
    }

}
