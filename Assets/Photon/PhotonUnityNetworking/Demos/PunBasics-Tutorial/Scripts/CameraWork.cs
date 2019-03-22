// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraWork.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the Camera work to follow the player
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

namespace Photon.Pun.Demo.PunBasics
{
	/// <summary>
	/// Camera work. Follow a target
	/// </summary>
	public class CameraWork : MonoBehaviour
	{
        #region Private Fields

	    [Tooltip("The distance in the local x-z plane to the target")]
	    [SerializeField]
	    private float distance = 7.0f;

	    [Tooltip("The height we want the camera to be above the target")]
	    [SerializeField]
	    private float height = 3.0f;

	    [Tooltip("The Smooth time lag for the height of the camera.")]
	    [SerializeField]
	    private float heightSmoothLag = 0.3f;

	    [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
	    [SerializeField]
	    private Vector3 centerOffset = Vector3.zero;

	    [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
	    [SerializeField]
	    private bool followOnStart = false;

        // cached transform of the target
        Transform cameraTransform;

		// maintain a flag internally to reconnect if target is lost or camera is switched
		bool isFollowing;

		// Represents the current velocity, this value is modified by SmoothDamp() every time you call it.
		private float heightVelocity;

		// Represents the position we are trying to reach using SmoothDamp()
		private float targetHeight = 100000.0f;

        #endregion

        #region MonoBehaviour Callbacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase
        /// </summary>
        void Start()
		{
			// Start following the target if wanted.
			if (followOnStart)
			{
				OnStartFollowing();
			}

		}

		/// <summary>
		/// MonoBehaviour method called after all Update functions have been called. This is useful to order script execution. For example a follow camera should always be implemented in LateUpdate because it tracks objects that might have moved inside Update.
		/// </summary>
		void LateUpdate()
		{
			// The transform target may not destroy on level load, 
			// so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
			if (cameraTransform == null && isFollowing)
			{
				OnStartFollowing();
			}

			// only follow is explicitly declared
			if (isFollowing) {
				Apply ();
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Raises the start following event. 
		/// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
		/// </summary>
		public void OnStartFollowing()
		{	      
			cameraTransform = Camera.main.transform;
			isFollowing = true;
			// we don't smooth anything, we go straight to the right camera shot
			Cut();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Follow the target smoothly
		/// </summary>
		void Apply()
	    {
			Vector3 targetCenter = transform.position + centerOffset;

	        // Calculate the current & target rotation angles
			float originalTargetAngle = transform.eulerAngles.y;
	        float currentAngle = cameraTransform.eulerAngles.y;

	        // Adjust real target angle when camera is locked
	        float targetAngle = originalTargetAngle;

			currentAngle = targetAngle;

	        targetHeight = targetCenter.y + height;
	        
	        // Damp the height
	        float currentHeight = cameraTransform.position.y;
	        currentHeight = Mathf.SmoothDamp( currentHeight, targetHeight, ref heightVelocity, heightSmoothLag );

	        // Convert the angle into a rotation, by which we then reposition the camera
	        Quaternion currentRotation = Quaternion.Euler( 0, currentAngle, 0 );

	        // Set the position of the camera on the x-z plane to:
	        // distance meters behind the target
	        cameraTransform.position = targetCenter;
	        cameraTransform.position += currentRotation * Vector3.back * distance;

	        // Set the height of the camera
	        cameraTransform.position = new Vector3( cameraTransform.position.x, currentHeight, cameraTransform.position.z );

	        // Always look at the target	
	        SetUpRotation(targetCenter);
	    }

	   
		/// <summary>
		/// Directly position the camera to a the specified Target and center.
		/// </summary>
		void Cut( )
	    {
	        float oldHeightSmooth = heightSmoothLag;
	        heightSmoothLag = 0.001f;

	        Apply();

	        heightSmoothLag = oldHeightSmooth;
	    }

		/// <summary>
		/// Sets up the rotation of the camera to always be behind the target
		/// </summary>
		/// <param name="centerPos">Center position.</param>
	    void SetUpRotation( Vector3 centerPos )
	    {
	        Vector3 cameraPos = cameraTransform.position;
	        Vector3 offsetToCenter = centerPos - cameraPos;

	        // Generate base rotation only around y-axis
	        Quaternion yRotation = Quaternion.LookRotation( new Vector3( offsetToCenter.x, 0, offsetToCenter.z ) );

	        Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
	        cameraTransform.rotation = yRotation * Quaternion.LookRotation( relativeOffset );

	    }

		#endregion
	}
}