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
﻿using UnityEngine;
using DG.Tweening;

public class GlobalInit : MonoBehaviour {
    public AudioSource musicFadeIn;
    public bool clearPools = false;
    
    [System.Serializable]
    public struct PoolPreloadItem {
        public GameObject prefab;
        public int quantity;
    }
    
    public PoolPreloadItem[] poolPreload;
    
    void Awake() {
        // Preload pool items.
        foreach (var item in poolPreload) {
            SimplePool.Preload(item.prefab, item.quantity);
        }
    }
    
    void Start() {
        DOTween.Init(recycleAllByDefault: true, useSafeMode: true, logBehaviour: LogBehaviour.Default);
        DOTween.SetTweensCapacity(500, 10);
        
        Cursor.visible = true;
        
        musicFadeIn.DOFade(0f, 2f).From().SetDelay(1f);
    }
    
    void OnDeactivate() {
        DOTween.Clear();
    }
}
