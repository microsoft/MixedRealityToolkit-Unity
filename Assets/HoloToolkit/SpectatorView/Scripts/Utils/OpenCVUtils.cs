using System.Runtime.InteropServices;
using UnityEngine;

namespace HoloToolkit.Unity.SpectatorView
{
	/// <summary>
	/// Utility function to ensure the OpenCVWrapper has been successfully loaded
	/// </summary>
	public class OpenCVUtils : MonoBehaviour
	{
		/// <summary>
		/// Utility function to ensure the OpenCVWrapper has been successfully loaded
		/// </summary>
		[DllImport("SpectatorViewPlugin", EntryPoint = "CheckLibraryHasLoaded")]
		public static extern void CheckOpenCVWrapperHasLoaded();
	}
}
