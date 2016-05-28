using UnityEngine;
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
