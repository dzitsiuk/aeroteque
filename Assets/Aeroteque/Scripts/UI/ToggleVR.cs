using UnityEngine;
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
