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
using UnityEngine;
using DG.Tweening;

public class TimedDespawner : MonoBehaviour
{
    public float timeOut = 1.0f;
    public bool detachChildren = false;
    public bool tweenOutScale = false;
    public float tweenOutDuration = 0.5f;
    
    private void OnEnable() {
        Invoke("DespawnNow", timeOut);
        if (tweenOutScale && timeOut > tweenOutDuration) {
            Invoke("TweenOut", timeOut - tweenOutDuration);
        }
    }
    
    private void TweenOut() {
        transform.DOScale(0f, tweenOutDuration - 0.05f).SetEase(Ease.InBack);
    }

    private void DespawnNow() {
        if (detachChildren) {
            transform.DetachChildren();
        }
        SimplePool.Despawn(gameObject);
    }
}
