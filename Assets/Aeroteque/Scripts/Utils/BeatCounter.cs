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
using System.Collections;

public class BeatCounter : MonoBehaviour {
	public static event GameActions.SimpleAction Tic;

	public static event GameActions.SimpleAction Toc;
	
	public static event GameActions.SimpleAction Tic1;
	public static event GameActions.SimpleAction Tic2;
	public static event GameActions.SimpleAction Tic3;
	
	public int numTicsPerToc = 4;
	public int sampleBpmMultiplier = 2;
	public float loopFrequency = 30f;
	public float offset = 0f;
	public AudioSource audioSource;
	
	[Space]
	public float bpm = 90f;
	public float startDelay = 1f;
	public delegate void AudioStartAction(double syncTime);
	public static event AudioStartAction OnAudioStart;
	
	private float nextBeatSample;
	private float samplePeriod;
	private float currentSample;

	private int ticCount = 0;
	
	void Awake () {
		// Calculate number of samples between each beat.
		samplePeriod = (60f / (bpm * sampleBpmMultiplier)) * audioSource.clip.frequency;
		
		if (offset < 0f) {
			offset = samplePeriod - offset;
		}

		nextBeatSample = 0f;
	}

	void Start () {
#if !UNITY_WEBGL
		double initTime = AudioSettings.dspTime;
		GetComponent<AudioSource>().PlayScheduled(initTime + startDelay);
		if (OnAudioStart != null) {
			OnAudioStart(initTime + startDelay);
		}
#else
		GetComponent<AudioSource>().Play();
		if (OnAudioStart != null) {
			OnAudioStart(AudioSettings.dspTime);
		}
#endif
	}
	
	void StartBeatCheck (double syncTime)
	{
		nextBeatSample = (float)syncTime * audioSource.clip.frequency;
		StartCoroutine(BeatCoroutine());
	}
	
	void OnEnable() {
		OnAudioStart += StartBeatCheck;
	}

	void OnDisable() {
		OnAudioStart -= StartBeatCheck;
	}

	IEnumerator BeatCoroutine()	{
		while (enabled) {
			currentSample = (float)AudioSettings.dspTime * audioSource.clip.frequency;
			
			if (currentSample >= (nextBeatSample + offset)) {
				if (audioSource.isPlaying && Time.timeScale > 0f) {
					int ticType = (ticCount++) % numTicsPerToc;
					if (Tic != null) {
						Tic();
					}
					switch (ticType) {
						case 0:
							if (Toc != null) {
								Toc();
							}
							break;
						case 1:
							if (Tic1 != null) Tic1();
							break;
						case 2:
							if (Tic2 != null) Tic2();
							break;
						case 3:
							if (Tic3 != null) Tic3();
							break;
					}
				}
				
				nextBeatSample += samplePeriod;
			}

			yield return new WaitForSecondsRealtime(loopFrequency / 1000f);
		}
	}

}
