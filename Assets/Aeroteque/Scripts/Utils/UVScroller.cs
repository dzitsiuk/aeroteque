using UnityEngine;

public class UVScroller : MonoBehaviour {
	public Material targetMaterial;
	public float speedX = 0f;
	public float speedY = 0f;
	
	private Vector2 offset;
	private Vector2 initOffset;
	
	void Start() {
		offset = targetMaterial.mainTextureOffset;
		initOffset = targetMaterial.mainTextureOffset;
	}
	
	void OnDisable() {
		targetMaterial.mainTextureOffset = initOffset;
	}

	void Update () {
		offset.x += speedX * Time.deltaTime;
		offset.y += speedY * Time.deltaTime;
		targetMaterial.mainTextureOffset = offset;
	}
}
