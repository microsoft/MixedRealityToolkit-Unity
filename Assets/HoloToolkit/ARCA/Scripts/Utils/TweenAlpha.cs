using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAlpha : MonoBehaviour
{

    public float TargetAlpha;
    public float Duration = 0.5f;
    Material mat;

    void Start()
    {
        if (mat == null)
            mat = GetComponent<Renderer>().material;
    }
   
	public void StartEffect()
    {
        StartCoroutine(LerpAlpha());
    }

    IEnumerator LerpAlpha()
    {
        var elapsedTime = 0.0f;
        var currentA = mat.color.a;
        while (elapsedTime < Duration)
        {
            elapsedTime += Time.deltaTime;
            var a = Mathf.Lerp(currentA,TargetAlpha, (elapsedTime / Duration));
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b,a);
            yield return null;
        }
    }
}
