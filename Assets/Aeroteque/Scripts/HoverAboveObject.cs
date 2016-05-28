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

public class HoverAboveObject : MonoBehaviour {
	public Transform target;
	public Vector3 offset;
	
	void Update () {
		var rotatedOffset = new Vector3(offset.z * Mathf.Sin(target.rotation.eulerAngles.y * Mathf.Deg2Rad), offset.y,
										offset.z * Mathf.Cos(target.rotation.eulerAngles.y * Mathf.Deg2Rad));
		transform.position = target.position + rotatedOffset;
		transform.LookAt(target.position);
	}
}
