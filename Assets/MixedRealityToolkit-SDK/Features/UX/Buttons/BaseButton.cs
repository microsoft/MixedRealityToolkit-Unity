using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloToolkit.Unity.Buttons
{
    public class BaseButton// TEMP : ProfileButtonBase<ButtonMeshProfile>
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(BaseButton))]
    public class CustomEditor { }// TEMP: MRTKEditor { }
#endif
}
