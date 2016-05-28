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
using DG.Tweening;

public class PunchToBeat : MonoBehaviour {
	public int beatsPerPunch = 2;
	public float punchScale = 0.1f;
	public float punchPosition = 0.1f;
	public float punchRotation = 0.1f;
	private int beatCount = 0;
	private int beatOffset = 0;

	void OnEnable() {
		BeatCounter.Tic += HandleTic;
		
		beatOffset = Random.Range(1, 4);
	}
	
	void OnDisable() {
		BeatCounter.Tic -= HandleTic;
	}
	
	private void HandleTic() {
		if ((((++beatCount) + beatOffset) % beatsPerPunch) == 0) {
			if (punchScale > 0f) {
				transform.DOPunchScale(Vector3.one * punchScale, 0.15f, 2);
			}
			
			if (punchPosition > 0f) {
				transform.DOPunchPosition(Vector3.one * punchPosition, 0.15f, 2);
			}
			
			if (punchRotation > 0f) {
				transform.DOPunchRotation(Random.onUnitSphere * punchRotation, 0.25f, 2);
			}
		}
	}
}
