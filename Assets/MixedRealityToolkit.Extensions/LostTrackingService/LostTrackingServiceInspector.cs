#if UNITY_EDITOR
using System;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking.Editor
{	
	[MixedRealityServiceInspector(typeof(ILostTrackingService))]
	public class LostTrackingServiceInspector : BaseMixedRealityServiceInspector
	{
		public override void DrawInspectorGUI(object target)
		{
			LostTrackingService service = (LostTrackingService)target;
			
			// Draw inspector here
		}
	}
}

#endif