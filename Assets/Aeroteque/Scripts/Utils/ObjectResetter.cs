//-----------------------------------------------------------------------
// Aeroteque - desktop, VR and Google Tango game made with Unity.
// Copyright (C) 2016  Dustyroom
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//-----------------------------------------------------------------------
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
