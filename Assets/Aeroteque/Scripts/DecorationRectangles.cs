using UnityEngine;

public class DecorationRectangles : MonoBehaviour {
	public float rotationSpeed = 1f;
	public GameObject largeRing;
	public GameObject mediumRing;
	public GameObject smallRing;
	public Vector3 rotationAxis = Vector3.forward;
	
	void FixedUpdate () {
		largeRing.transform.Rotate(rotationAxis, rotationSpeed);
		mediumRing.transform.Rotate(rotationAxis, rotationSpeed / 2f);
		smallRing.transform.Rotate(rotationAxis, rotationSpeed / 4f);
	}
}
