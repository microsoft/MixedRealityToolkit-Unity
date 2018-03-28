// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.ARCapture
{
	public class PlatformSwitcher : MonoBehaviour
	{
        [Serializable]
		public enum Platform
		{
			Hololens = 0,
			IPhone
		}

        [SerializeField]
		public Platform TargetPlatform;

		public void SwitchPlatform(Platform platform)
		{
#if UNITY_EDITOR
			TargetPlatform = platform;

			string platformGameObjectName = "";

			switch(platform)
			{
				case Platform.Hololens:
					platformGameObjectName = "Hololens";
					break;

				case Platform.IPhone:
					platformGameObjectName = "IPhone";
					break;
			}

			for(int i=0; i<transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);

				if(child.gameObject.name == "Shared")
				{
					continue;
				}

			    if (child.gameObject.name == "WorldSync")
				{
			        continue;
				}

                child.gameObject.SetActive(child.name == platformGameObjectName);
			}
#endif
		}
	}
}
