using UnityEngine;

public class OutOfBoundsController : MonoBehaviour {
	public float maxDistance = 100f;
	public float globalMinY = 0f;
	public Transform targetObject;
	
	void Update () {
		if (transform.position.y < globalMinY || 
				Vector3.Distance(transform.position, targetObject.position) > maxDistance) {
			var gameOver = GameObject.FindObjectOfType<GameOverController>();
			gameOver.HandleGameOver();
		}
	}
}
