using UnityEngine;
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
