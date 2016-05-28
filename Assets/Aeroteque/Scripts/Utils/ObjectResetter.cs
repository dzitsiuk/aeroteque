using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectResetter : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private List<Transform> originalStructure;

    private Rigidbody Rigidbody;

    private void Start()
    {
        originalStructure = new List<Transform>(GetComponentsInChildren<Transform>());
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        Rigidbody = GetComponent<Rigidbody>();
    }

    public void Reset() {
        // remove any gameobjects added (fire, skid trails, etc)
        foreach (var t in GetComponentsInChildren<Transform>())
        {
            if (!originalStructure.Contains(t))
            {
                t.parent = null;
            }
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
        if (Rigidbody)
        {
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void DelayedReset(float delay)
    {
        StartCoroutine(ResetCoroutine(delay));
    }


    public IEnumerator ResetCoroutine(float delay)
    {
        if (delay > 0f) {
            yield return new WaitForSeconds(delay);
        }

        Reset();
    }
}
