using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{
    public AnimationCurve curve;
    public float timeOffset = 0.0f;
    // Start is called before the first frame update
    void Awake()
    {
        //transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.localScale = Vector3.one * curve.Evaluate(Time.time - timeOffset);
    }
}
