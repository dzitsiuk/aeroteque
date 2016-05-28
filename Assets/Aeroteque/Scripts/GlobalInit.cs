using UnityEngine;
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
