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
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class GameOverController : MonoBehaviour {
	public GameObject menu;
	public AudioSource music;
	public bool startLevelAfterRestart = true;
	public bool skipGameOverMenu = false;
	public Material skyboxMaterial;
	public bool hardReset = false;
	public Transform startPlatformInstance;
	public Transform startPlatformPrefab;
	
	private bool isHandled = false;
	private const string skyboxPropName = "_Border";
	private float skyboxBorder;
	
	void Start() {
		if (skyboxMaterial != null) {
			skyboxMaterial.SetFloat(skyboxPropName, 0.2f);
		}
	}
	
	public void HandleGameOver() {
		if (isHandled) {
			return;
		}
		isHandled = true;
		
		if (!skipGameOverMenu) {
			menu.SetActive(true);
			var menuCanvas = menu.GetComponentInChildren<CanvasGroup>();
			menuCanvas.DOFade(0f, 0.5f).From().SetUpdate(true);
			Cursor.visible = true;
			Time.timeScale = 0f;
		} else {
			DOVirtual.DelayedCall(3.5f, ()=>{
				Restart();
			});
			
			music.DOPitch(0.6f, 2.4f);
			
			if (skyboxMaterial != null) {
				skyboxBorder = skyboxMaterial.GetFloat(skyboxPropName);
				skyboxMaterial.DOFloat(1f, skyboxPropName, 2.5f).SetEase(Ease.InExpo);
			}
		}
	}
	
	public virtual void Restart() {
		Time.timeScale = 1f;
		
		if (hardReset) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		} else {
			var resets = GameObject.FindObjectsOfType<ObjectResetter>();
			foreach (var c in resets) {
				c.Reset();
			} 
			
			var spawners = GameObject.FindObjectsOfType<SpawnGameObjects>();
			foreach (var c in spawners) {
				c.Reset();
			}
			
			if (startPlatformPrefab != null && !startPlatformInstance) {
				startPlatformInstance = Instantiate(startPlatformPrefab, startPlatformPrefab.position, 
						startPlatformPrefab.rotation) as Transform;
			}
		}
		
		if (startLevelAfterRestart) {
			StartCoroutine(RequestStart());
		}
		
		if (skipGameOverMenu) {
			music.DOPitch(1f, 0.2f);
			if (skyboxMaterial != null) {
				skyboxMaterial.DOFloat(skyboxBorder, skyboxPropName, 2.5f).SetEase(Ease.OutExpo).OnComplete(() => {
					isHandled = false;
				});
			}
		} else {
			isHandled = false;
		}
		
		var score = GameObject.FindObjectOfType<ScoreCounter>();
		if (score != null) {
			score.Reset();
		}
	}
	
	private IEnumerator RequestStart() {
		yield return new WaitForEndOfFrame();
		LevelController.startRequested = true;
	}
}
