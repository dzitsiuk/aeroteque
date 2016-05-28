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
using System;
using System.Collections;

public class PlatformController : MonoBehaviour {
	public static Vector3 HVec3 = new Vector3(1f, 1f, 0f);
	
	public int numberBeatsPerSwap = 4;
	public int numberBeatsPerPunch = 2;
	public int beatOffset = 0;
	public Vector3 transitionRotation = new Vector3(0f, 90f, 0f);
	public string platformConfigName;
	public string decorationConfigName;
	public float decorationChance = 0.7f;
	public bool looped = false;
	public int loopedMaxIndex = 7;
	public int loopedMinIndex = 4;
	
	private SpawnCollectionConfig platformConfig;
	private SpawnCollectionConfig decorationConfig;
	
	private GameObject decoration;
	private int beatCount = 0;
	private int currentPlatformType;
	private bool removed = true;

	void Awake() {
		var configs = GameObject.FindObjectsOfType<SpawnCollectionConfig>();
		foreach (var config in configs) {
			if (config.configName.Equals(platformConfigName, StringComparison.OrdinalIgnoreCase)) {
				platformConfig = config;
			}
			if (config.configName.Equals(decorationConfigName, StringComparison.OrdinalIgnoreCase)) {
				decorationConfig = config;
			}
		}
		if (platformConfig == null) {
			Debug.LogError("Can't find PlatformConfig");
			enabled = false;
			return;
		}
		if (decorationConfig == null) {
			Debug.LogError("Can't find DecorationConfig");
			enabled = false;
			return;
		}
	}
	
	void OnEnable() {
		if (!removed) {
			return;
		}
		removed = false;
		beatCount = 0;
		
		currentPlatformType = looped? loopedMaxIndex : 
				Mathf.Max(platformConfig.numberItemsToUse, platformConfig.spawnPrefabs.Length) - 1;
		var meshContainer = platformConfig.spawnPrefabs[currentPlatformType];
		this.GetComponent<MeshFilter>().mesh = meshContainer.GetComponent<MeshFilter>().sharedMesh;
		
        // Add decoration randomly.
        if (UnityEngine.Random.value < decorationChance && decorationConfig.spawnPrefabs.Length > 0) {
			int index = UnityEngine.Random.Range(0, decorationConfig.spawnPrefabs.Length);
            var decorationPrefab = decorationConfig.spawnPrefabs[index];
			
            decoration = SimplePool.Spawn(decorationPrefab, transform.position, transform.rotation);
			
            if (decoration == null) {
                Debug.LogError("Can't spawn decoration object from pool: " + decorationPrefab.name);
            } else {
                decoration.transform.parent = transform;
				
				// Make relative position and rotation work.
				decoration.transform.localPosition = decorationPrefab.transform.position;
				decoration.transform.localRotation = decorationPrefab.transform.rotation;
				decoration.transform.localScale = Vector3.one;
            }
        }

		BeatCounter.Tic += HandleTic;
	}
	
	void OnDisable() {
		if (!removed) {
			return;
		}
		BeatCounter.Tic -= HandleTic;
	}
	
	void DespawnPlatform() {
		StopAllCoroutines();
		GetComponent<Renderer>().enabled = true;
		
		transform.DetachChildren();
		
		// Despawn decoration.
		if (decoration != null) {
			decoration.transform.position = Vector3.zero;
			decoration.transform.localPosition = Vector3.zero;
			
			SimplePool.Despawn(decoration);
			
			decoration = null;
		}
		
		// Despawn itself.
		if (gameObject != null) {
			SimplePool.Despawn(gameObject);
			//Destroy(gameObject);
		}
	}
	
	private void HandleTic() {
		if (removed || Time.timeScale <= 0f) {
			return;
		}
		
		if (((beatCount++) + beatOffset) % numberBeatsPerSwap == 0) {
			ReplaceMesh();
		} else {
			if ((beatCount + beatOffset) % numberBeatsPerPunch == 0) {
				if (this == null) {
					return;
				}
				
				#if UNITY_WEBGL
				transform.DOComplete();
				#endif
				
				transform.DOPunchScale(HVec3 * 0.25f, 0.2f, 3, 1f);
			}
		}
	}
	
	private void ReplaceMesh() {
		currentPlatformType--;
		
		if (looped && currentPlatformType < loopedMinIndex) {
			currentPlatformType = loopedMaxIndex;
		}
		
		if (currentPlatformType < 0) {
			TweenOut();
		} else {
			// Unity is fun.
			if (this == null) {
				return;
			}
			
			if (currentPlatformType < 1 && !removed && gameObject.activeSelf) {
				StartCoroutine(BlinkCoroutine());
			}
			
			var meshContainer = platformConfig.spawnPrefabs[currentPlatformType];
			
			this.GetComponent<MeshFilter>().mesh = meshContainer.GetComponent<MeshFilter>().sharedMesh;
			
			#if UNITY_WEBGL
			transform.DOComplete();
			#endif
			
			transform.DOPunchScale(HVec3 * 0.1f, 0.2f, 3, 1f);
			
			float rotationDirection = UnityEngine.Random.value < 0.5f? 1f : -1f;
			transform.DORotate(transform.rotation.eulerAngles + transitionRotation * rotationDirection, 0.1f);
		}
	}
	
	private IEnumerator BlinkCoroutine() {
		var renderers = GetComponentsInChildren<Renderer>();
		bool isDimmed = false;
		while (!removed && enabled) {
			isDimmed = !isDimmed;
			
			foreach (var renderer in renderers) {
				if (renderer.material.HasProperty("_Alpha")) {
					renderer.material.SetFloat("_Alpha", isDimmed? 0.6f : 1f);
				}
			}
			
			yield return new WaitForSeconds(0.1f);
		}
		
		foreach (var renderer in renderers) {
			if (renderer.material.HasProperty("_Alpha")) {
				renderer.material.SetFloat("_Alpha", 1f);
			}
		}
	}
	
	private void TweenOut() {
		removed = true;
		
		// Restarting requires this. I know, right?
		if (this == null) {
			return;
		}
		
		transform.DORotate(transform.rotation.eulerAngles + transitionRotation, 0.15f);
		transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InExpo).OnComplete(()=>{
			DespawnPlatform();
		});
	}
}
