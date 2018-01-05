//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity
{
	/// <summary>
	///   Momentumizer solver applies accel/velocity/friction to simulate momentum for an object being moved by other solvers/components
	/// </summary>
	public class SolverMomentumizer : Solver
	{
		[Tooltip("Friction to slow down the current velocity")]
		public float resistance = 0.99f;
		[Tooltip("Apply more resistance when going faster- applied resistance is resistance * (velocity ^ reisistanceVelPower)")]
		public float resistanceVelPower = 1.5f;
		[Tooltip("Accelerate to goal position at this rate")]
		public float accelRate = 10f;
		[Tooltip("Apply more acceleration if farther from target- applied accel is accelRate + springiness * distance")]
		public float springiness = 0;

		[Tooltip("Instantly maintain a constant depth from the view point instead of simulating Z-velocity")]
		public bool SnapZ = true;

		private Vector3 velocity;

		public override void SolverUpdate()
		{
			CalculateMomentum();
		}

		public override void SnapTo(Vector3 position, Quaternion rotation)
		{
			base.SnapTo(position, rotation);
			velocity = Vector3.zero;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			velocity = Vector3.zero;
		}

		private void CalculateMomentum()
		{
			// Start with SnapZ
			if (SnapZ)
			{
				// Snap the current depth to the goal depth
				var refPos = getRefPos();
				float goalDepth = (solverHandler.GoalPosition - refPos).magnitude;
				Vector3 currentDelta = transform.position - refPos;
				float currentDeltaLen = currentDelta.magnitude;
				if (!Mathf.Approximately(currentDeltaLen, 0))
				{
					Vector3 currentDeltaNorm = currentDelta / currentDeltaLen;
					transform.position += currentDeltaNorm * (goalDepth - currentDeltaLen);
				}
			}

			// Determine and apply accel
			Vector3 delta = solverHandler.GoalPosition - transform.position;
			float deltaLen = delta.magnitude;
			if (deltaLen > 0.01f)
			{
				Vector3 deltaNorm = delta / deltaLen;

				velocity += deltaNorm * (solverHandler.DeltaTime * (accelRate + springiness * deltaLen));
			}

			// Resistance
			float velMag = velocity.magnitude;
			if (!Mathf.Approximately(velMag, 0))
			{
				Vector3 velNormal = velocity / velMag;
				float powFactor = velMag > 1f ? Mathf.Pow(velMag, resistanceVelPower) : velMag;
				velocity -= velNormal * (powFactor * resistance * solverHandler.DeltaTime);
			}

			if (velocity.sqrMagnitude < 0.001f)
			{
				velocity = Vector3.zero;
			}

			// Apply vel to the solver... no wait, the actual transform
			transform.position += velocity * solverHandler.DeltaTime;
		}

		private Vector3 getRefPos()
		{
			if (solverHandler.TransformTarget == null)
			{
				return Vector3.zero;
			}
			return solverHandler.TransformTarget.position;
		}
	}
}
