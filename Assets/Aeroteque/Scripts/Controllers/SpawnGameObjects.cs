using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class SpawnGameObjects : MonoBehaviour
{
    public int beatsBetweenSpawns = 4;
    public int beatOffset = 4;
    public int numberObjectsPerSpawn = 1;
    public Vector3 minRange = new Vector3(-100, -100, 0);
    public Vector3 maxRange = new Vector3(100, 100, 20);
    public float minDistanceToOthers = 10f;
    public string othersLayerName;

    [Space]
    public Transform spawnStartPoint;
    public Vector3 spawnPositionSpeed = new Vector3(0f, 1f, 5f);
    public bool isMoving = false;

    [Space]
    public Vector2 spawnPositionWavelength;
    public Vector2 spawnPositionAmplitude;

    [Space]
    public bool randomScale = false;
    public float scaleMin = 0.8f;
    public float scaleMax = 1.2f;
    
    [Space]
    public bool drawGizmos = true;
    public bool usePooling = false;
    public GameObject[] spawnObjects;
    
    private int beatCount = 0;
    private int layerMask;
    private Vector3 currentSpawnCenter;
    private float oscArg = 0f;

    void Start() {
        if (spawnObjects.Length == 0) {
            Debug.LogError("SpawnObjects is empty!");
            enabled = false;
            return;
        }
        
        layerMask = 1 << LayerMask.NameToLayer(othersLayerName);
        currentSpawnCenter = spawnStartPoint.position;
    }
    
    void OnEnable() {
        BeatCounter.Tic += HandleTic;
    }
    
    void OnDisable() {
        BeatCounter.Tic -= HandleTic;
    }
    
    public void Reset() {
        currentSpawnCenter = spawnStartPoint.position;
    }
    
    private void HandleTic() {
        if ((beatCount + beatOffset + 1) % beatsBetweenSpawns == 0) {
            for (int i = 0; i < numberObjectsPerSpawn; ++i) {
                SpawnObject();
            }
        }
        beatCount++;
    }
    
    void FixedUpdate() {
        if (isMoving) {
            // Wavy movement around.
            currentSpawnCenter.x += Mathf.Sin(oscArg * spawnPositionWavelength.x) * spawnPositionAmplitude.x;
            currentSpawnCenter.y += Mathf.Cos(oscArg * spawnPositionWavelength.y) * spawnPositionAmplitude.y;
            oscArg += 0.01f;
            // Move forward.
            currentSpawnCenter += spawnPositionSpeed * 0.01f;
            // Update transform.
            spawnStartPoint.position = currentSpawnCenter;
        }
    }

    void OnDrawGizmos() {
        if (!drawGizmos) {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(currentSpawnCenter, 2.5f);
        Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(currentSpawnCenter + (maxRange + minRange) / 2f, maxRange - minRange);
    }

    void SpawnObject() {
        Vector3 spawnPosition = currentSpawnCenter;

        bool positionValid = false;
        int tryCount = 0;
        while (!positionValid) {
            spawnPosition = currentSpawnCenter;
            spawnPosition.x += Random.Range(minRange.x, maxRange.x);
            spawnPosition.y += Random.Range(minRange.y, maxRange.y);
            spawnPosition.z += Random.Range(minRange.z, maxRange.z);

            positionValid = true;
            Collider[] platforms = Physics.OverlapSphere(spawnPosition, minDistanceToOthers,
                    layerMask);
            // If there is another platform except the current one.
            if (platforms.Length > 0) {
                positionValid = false;
                if (tryCount++ > 1000) {
                    Debug.LogError("Can't find a place to spawn an object");
                    break;
                }
            }
        }

        // Actually spawn the game object.
        var spawnObjectPrefab = spawnObjects[Random.Range(0, spawnObjects.Length)];
        
        GameObject spawnedObject = null;
        if (usePooling) {
            spawnedObject = SimplePool.Spawn(spawnObjectPrefab, spawnPosition,
                    spawnObjectPrefab.transform.rotation);
        } else {
            spawnedObject = Instantiate(spawnObjectPrefab, spawnPosition, 
                    spawnObjectPrefab.transform.rotation) as GameObject;
        }
        if (spawnedObject == null) {
            Debug.LogError("Can't spawn object: " + spawnObjectPrefab.name);
            return;
        }
        
        // Make the parent the spawner so hierarchy doesn't get very messy.
        spawnedObject.transform.parent = transform;

        // Tween the object's scale.
        spawnedObject.transform.localScale = Vector3.zero;
        float scale = !randomScale? 1f : Random.Range(scaleMin, scaleMax);
        spawnedObject.transform.DOScale(scale, 0.6f).SetEase(Ease.OutElastic);
    }
}