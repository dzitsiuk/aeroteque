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
	/// Controls all boids flocking around this object.
	/// </summary>
	public class BoidMaster : MonoBehaviour
	{
		/// <summary>
		/// What should we instantiate as a single boid.
		/// </summary>
		public GameObject boidPrefab;

		/// <summary>
		/// How many boids to create at start?
		/// </summary>
		public int spawnCount = 10;

		/// <summary>
		/// How far away should the new boids be spawned?
		/// </summary>
		public float spawnRadius = 100f;

		/// <summary>
		/// Minimum distance between all boids.
		/// </summary>
		public float neighborDistance = 10.0f;

		/// <summary>
		/// Speed of boids.
		/// </summary>
		public float speed = 10f;

		/// <summary>
		/// How much randomness there is in the speed.
		/// </summary>
		public float speedVariation = 1f;

		/// <summary>
		/// How easily boids rotate.
		/// </summary>
		public float rotationCoefficient = 5.0f;

		/// <summary>
		/// Unity layer where boids are.
		/// </summary>
		public LayerMask searchLayer;

		void Start()
		{
			for (var i = 0; i < spawnCount; i++)
			{
				Spawn();
			}
		}

		public GameObject Spawn()
		{
			return Spawn(transform.position + Random.insideUnitSphere * spawnRadius);
		}

		public GameObject Spawn(Vector3 position)
		{
			// Init a boid.
			var rotation = Quaternion.Slerp(transform.rotation, Random.rotation, 0.25f);
			var boid = Instantiate(boidPrefab, position, rotation) as GameObject;
			boid.GetComponent<BoidUnit>().master = this;

			// If possible, place the boid as a neighbour of the controller in the hierarchy.
			if (this.transform.parent != null)
			{
				boid.transform.parent = this.transform.parent;
			}

			return boid;
		}
	}
}