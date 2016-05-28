//-----------------------------------------------------------------------
// Aeroteque - desktop, VR and Google Tango game made with Unity.
// Copyright (C) 2016  Dustyroom
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//-----------------------------------------------------------------------
﻿using UnityEngine;

namespace FlightKit
{
	/// <summary>
	/// Controller of a single boid instance.
	/// </summary>
	public class BoidUnit : MonoBehaviour
	{
		/// <summary>
		/// Reference to the boid controller.
		/// </summary>
		public BoidMaster master;

		void Update()
		{
			var currentPosition = transform.position;
			var currentRotation = transform.rotation;

			// Initialize steering vectors.
			var separation = Vector3.zero;
			var alignment = master.transform.forward;
			var cohesion = master.transform.position;

			// Looks up nearby boids.
			var nearbyBoids = Physics.OverlapSphere(currentPosition, master.neighborDistance, master.searchLayer);
            
			// Accumulates the vectors.
			foreach (var boid in nearbyBoids)
			{
				// Skip myself.
				if (boid.gameObject == gameObject)
				{
					continue;
				}
				var t = boid.transform;
				separation += GetSeparationVector(t);
				alignment += t.forward;
				cohesion += t.position;
			}

			// Normalize steering vectors.
			var avg = 1.0f / nearbyBoids.Length;
			alignment *= avg;
			cohesion *= avg;
			cohesion = (cohesion - currentPosition).normalized;

			// Calculates a rotation from the vectors.
			var direction = separation + alignment + cohesion;
			var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);

			// Apply the rotation with interpolation.
			if (rotation != currentRotation)
			{
				var ip = Mathf.Exp(-master.rotationCoefficient * Time.deltaTime);
				transform.rotation = Quaternion.Slerp(rotation, currentRotation, ip);
			}

			// Velocity factors randomized with noise.
			var noise = Mathf.PerlinNoise(Time.time, Random.value * 10.0f) * 2.0f - 1.0f;
			var speed = master.speed * (1.0f + noise * master.speedVariation);

			// Moves forawrd.
			transform.position = currentPosition + transform.forward * speed * Time.deltaTime;
		}

		// Caluculates the separation vector with a target.
		private Vector3 GetSeparationVector(Transform target)
		{
			var vectorFromTarget = transform.position - target.transform.position;
			var distance = vectorFromTarget.magnitude;
			var scaler = Mathf.Clamp01(1.0f - distance / master.neighborDistance);
			return vectorFromTarget * (scaler / distance);
		}

	}
}