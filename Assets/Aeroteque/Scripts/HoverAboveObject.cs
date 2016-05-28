using UnityEngine;

public class HoverAboveObject : MonoBehaviour {
	public Transform target;
	public Vector3 offset;
	
	void Update () {
		var rotatedOffset = new Vector3(offset.z * Mathf.Sin(target.rotation.eulerAngles.y * Mathf.Deg2Rad), offset.y,
										offset.z * Mathf.Cos(target.rotation.eulerAngles.y * Mathf.Deg2Rad));
		transform.position = target.position + rotatedOffset;
		transform.LookAt(target.position);
	}
}
