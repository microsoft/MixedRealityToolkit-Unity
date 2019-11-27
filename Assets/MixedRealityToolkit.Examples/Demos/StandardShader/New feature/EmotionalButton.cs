using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionalButton : MonoBehaviour
{
    public GameObject ActiveOnOff;
    private bool act;




    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonAction()
    {
        GameObject g = GameObject.FindWithTag("object");
        act = ActiveOnOff.activeSelf;

        //Activate if target is deactive
        if (act == true)
        {
            ActiveOnOff.SetActive(false);
        }

        //Dectivate if target is active
        else
        {
            if (g != null)
            {
                g.SetActive(false);
            } 
            ActiveOnOff.SetActive(true);
        }
    }
}