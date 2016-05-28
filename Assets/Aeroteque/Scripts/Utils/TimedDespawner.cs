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
