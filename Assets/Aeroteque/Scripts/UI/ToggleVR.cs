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
using UnityStandardAssets.ImageEffects;

public class ToggleVR : MonoBehaviour {
	public bool isVR = false;
	public bool toggleVRWithClick = false;
	
	public GameObject[] monoViewObjects;
	public GameObject[] stereoViewObjects;
	
	public virtual void EnableVR() {
		isVR = true;
		InvalidateState();
	}
	
	public virtual void DisableVR() {
		isVR = false;
		InvalidateState();
	}
	
	public virtual void Toggle() {
		isVR = !isVR;
		InvalidateState();
	}
	
	private void InvalidateState() {
		var cardboard = GameObject.FindObjectOfType<Cardboard>();
		if (cardboard != null) {
			cardboard.VRModeEnabled = isVR;
		}
		
		foreach (var go in monoViewObjects) {
			go.SetActive(!isVR);
		}
		foreach (var go in stereoViewObjects) {
			go.SetActive(isVR);
		}
		
		if (isVR) {
			var vca = GameObject.FindObjectOfType<VignetteAndChromaticAberration>();
			if (vca != null) {
				vca.enabled = false;
			}
		}
	}
	
	void Update () {
		// Toggle VR.
		if (toggleVRWithClick && Input.GetMouseButtonDown(0) && Time.timeScale > 0f) {
			Toggle();
		}
	}
}
