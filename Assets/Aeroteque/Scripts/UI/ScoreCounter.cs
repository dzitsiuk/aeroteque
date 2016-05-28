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
using Characters.FirstPerson;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour {
	private static string BEST_SCORE_KEY = "BestScore";
	
	[HideInInspector]
	public int score = 0;
	
	public Text[] scroreTexts;
	public Text[] bestScroreTexts;
	
	private int bestScore = 0;
	
	void Awake() {
		if (PlayerPrefs.HasKey(BEST_SCORE_KEY)) {
			bestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY);
			
			foreach (var text in bestScroreTexts) {
				text.text = bestScore.ToString();
			}
		}
	}
	
	public void Reset() {
		score = 0;
		
		foreach (var text in scroreTexts) {
			text.text = score.ToString();
		}
	}
	
	void OnEnable() {
		PlatformerFirstPersonController.Landing += HandleLanding;
		TangoCharacterController.Landing += HandleLanding;
	}
	
	void OnDisable() {
		PlatformerFirstPersonController.Landing -= HandleLanding;
		TangoCharacterController.Landing -= HandleLanding;
	}
	
	void HandleLanding () {
		score++;
		
		foreach (var text in scroreTexts) {
			text.text = score.ToString();
		}
		
		if (score > bestScore) {
			bestScore = score;
			PlayerPrefs.SetInt(BEST_SCORE_KEY, bestScore);
			
			foreach (var text in bestScroreTexts) {
				text.text = bestScore.ToString();
			}
		}
	}
}
