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

public class UVScroller : MonoBehaviour {
	public Material targetMaterial;
	public float speedX = 0f;
	public float speedY = 0f;
	
	private Vector2 offset;
	private Vector2 initOffset;
	
	void Start() {
		offset = targetMaterial.mainTextureOffset;
		initOffset = targetMaterial.mainTextureOffset;
	}
	
	void OnDisable() {
		targetMaterial.mainTextureOffset = initOffset;
	}

	void Update () {
		offset.x += speedX * Time.deltaTime;
		offset.y += speedY * Time.deltaTime;
		targetMaterial.mainTextureOffset = offset;
	}
}
