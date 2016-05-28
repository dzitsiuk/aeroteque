using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public class LevelReset : MonoBehaviour {
	public bool startLevelAfterRestart = true;
	
	void Update () {
		if (CrossPlatformInputManager.GetButton("Reset") && Time.timeScale > 0) {
			DOTween.Clear();
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			if (startLevelAfterRestart) {
				StartCoroutine(RequestStart());
			}
		}
	}
	
	private IEnumerator RequestStart() {
		yield return new WaitForEndOfFrame();
		LevelController.startRequested = true;
	}
}
