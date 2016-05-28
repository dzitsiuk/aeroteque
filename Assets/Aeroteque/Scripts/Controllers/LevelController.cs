using UnityEngine;
using DG.Tweening;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.ImageEffects;

public class LevelController : MonoBehaviour {
	public bool startWithKeys = true;
	public bool startWithClick = false;
	public bool startMovingAllSpawners = true;
	public float startMovingAllSpawnersDelay = 10f;
	public bool removeStartPlatformAfterStart = true;
	public PlatformController startPlatform;
	public bool enablePlayerOnStart = true;
	public bool startWhenPlayerLooksDown = false;
	public MonoBehaviour playerController;
	public CanvasGroup mainMenu;
	public GameObject pauseMenu;
	public GameObject[] fadeInOnStart;
	public AudioSource gameplayMusic;
	
	[HideInInspector]
	public bool levelStarted = false;
	
	[HideInInspector]
	public static bool startRequested = false;
	private bool startRequestedLocal = false;
	private bool paused = false;
	private bool playerLookedForward = false;
	
	void OnEnable() {
		BeatCounter.Toc += HandleToc;
	}
	
	void OnDisable() {
		BeatCounter.Toc -= HandleToc;
	}
	
	void Update() {
		if (!startRequested && !levelStarted && startWhenPlayerLooksDown) {
			var t = Camera.main.transform;
			float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(t.localRotation.x / t.localRotation.w);
			
			if (playerLookedForward && angleX > 60.0f) {
				// Looked down after looking forward.
				startRequested = true;
			}
			
			if (angleX < 5f) {
				playerLookedForward = true;
				
				// Disperse boids for effect.
				var boids = GameObject.FindObjectOfType<FlightKit.BoidMaster>();
				if (boids != null) {
					boids.neighborDistance = 80;
				}
			}
			
		}
		
		if (startRequested) {
			mainMenu.DOFade(0f, 0.3f).OnComplete(()=>{
				mainMenu.gameObject.SetActive(false);
			});
			StartLevel();
		}
		
		bool keyStart = startWithKeys && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space));
		bool clickStart = startWithClick && Input.GetMouseButtonDown(0);
		if (keyStart || clickStart) {
			StartLevel();
		}
		
		if (CrossPlatformInputManager.GetButtonDown("Cancel")) {
			if (levelStarted) {
				if (!paused) {
					Pause();
				} else {
					UnPause();
				}
			} else {
				// Quit game.
				Application.Quit();
			}
		}
	}
	
	private void HandleToc() {
		if (startRequestedLocal) {
			StartLevelCore();
		}
	}

	public virtual void StartLevel() {
		if (levelStarted) {
			return;
		}
		
		mainMenu.DOFade(0f, 0.3f).OnComplete(()=>{
			mainMenu.gameObject.SetActive(false);
		});
		
		var vca = GameObject.FindObjectOfType<VignetteAndChromaticAberration>();
		if (vca != null) {
			DOVirtual.Float(1f, 50f, 0.5f, (float value) => { vca.chromaticAberration = value; })
					.SetLoops(2, LoopType.Yoyo);
		}
		
		foreach (var go in fadeInOnStart) {
			var canvas = go.GetComponentInChildren<CanvasGroup>();
			if (canvas != null) {
				canvas.alpha = 0f;
				canvas.DOFade(1f, 1f);
			}
		}
		
		startRequestedLocal = true;
	}
	
	private void StartLevelCore() {
		if (levelStarted) {
			return;
		}
		
		if (removeStartPlatformAfterStart) {
			startPlatform.looped = false;
		}
		
		if (enablePlayerOnStart) {
			playerController.enabled = true;
			var playerRigidbody = playerController.GetComponent<Rigidbody>();
			if (playerRigidbody != null) {
				playerRigidbody.useGravity = true;
			}
		}
		
		// Change music.
		var beatCounter = GameObject.FindObjectOfType<BeatCounter>();
		var currentMusic = beatCounter.audioSource;
		beatCounter.audioSource = gameplayMusic;
		gameplayMusic.Play();
		gameplayMusic.timeSamples = currentMusic.timeSamples;
		currentMusic.Stop();
		
		// If restarting.
		if (startRequested) {
			gameplayMusic.DOFade(0f, 1f).From();
		}
		
		if (startMovingAllSpawners) {
			DOVirtual.DelayedCall(startMovingAllSpawnersDelay, () => {
				var spawners = GameObject.FindObjectsOfType<SpawnGameObjects>();
				foreach (var spawner in spawners) {
					spawner.isMoving = true;
				}
			});
		}
		
		Cursor.visible = false;
		startRequested = false;
		startRequestedLocal = false;
		levelStarted = true;
	}
	
	public virtual void Pause() {
		paused = true;
		pauseMenu.SetActive(true);
		Cursor.visible = true;
		Time.timeScale = 0f;
		gameplayMusic.Pause();
	}
	
	public virtual void UnPause() {
		paused = false;
		pauseMenu.SetActive(false);
		if (levelStarted) {
			Cursor.visible = false;
		}
		Time.timeScale = 1f;
		gameplayMusic.UnPause();
	}
	
	public virtual void SetStartOnLookDown(bool start) {
		startWhenPlayerLooksDown = start;
	}
	
}
