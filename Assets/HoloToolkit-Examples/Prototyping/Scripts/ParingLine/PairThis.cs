using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Cursor = HoloToolkit.Unity.InputModule.Cursor;
public class PairThis : MonoBehaviour {

    public GameObject PairingLinePrefab;
    
	public float FadeOutTime = 0.5f;
    
    private Cursor mCursor;
    private List<PairingLine> PairingLineList;
    // Use this for initialization
    void Start()
    {
        PairingLineList = new List<PairingLine>();
    }

    public void PairToObject(GameObject obj)
    {
        if (mCursor == null || mCursor.enabled == false)
        {
            mCursor = FindObjectsOfType<Cursor>().Where(c => c.enabled).FirstOrDefault();
        }

        if (mCursor != null)
        {
            GameObject linesObj = GameObject.Instantiate(PairingLinePrefab);
            PairingLine line = linesObj.GetComponent<PairingLine>();
            line.StartPosition = mCursor.transform.position;
            line.LineType = PairingLine.PairingLineType.Trails;
            line.EndObject = obj;
            line.Width = 0.0025f; // for demo, 4/12
            line.FadeoutTime = FadeOutTime;
            line.FadeAndDestory = true;
            line.OnDestroyEvent.AddListener(HandleLineDestroy);
            PairingLineList.Add(line);
            //line.Color = color;
        }
    }

    public void KillLine(GameObject gameObject)
    {
        for (int i = 0; i < PairingLineList.Count; ++i)
        {
            if (PairingLineList[i].EndObject == gameObject)
            {
                PairingLineList[i].DestroyLine();
            }
        }
    }

    private void HandleLineDestroy(PairingLine line)
    {
        PairingLineList.Remove(line);
    }

}
