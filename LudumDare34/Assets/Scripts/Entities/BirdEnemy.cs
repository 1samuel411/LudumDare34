using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class BirdEnemy : BaseEntity
{

    public GameObject skull;
    public GameObject skullSpawner;
    public float skullSpawningInterval;

    public float flapSpeed;
    public float flapHeight;

    private float originalFlapHeight;
    private float flapIndex;
    private int _spawnHandlerKey;
    private bool _invokedSpawn = false;
    public SpawnHandlerDetails spawnHandlerDetails;
    public SpawnObject spawnObject;

    public override void Awake() {
        base.Awake();

        if (!_invokedSpawn) {
            _spawnHandlerKey = _poolManager.CreateNewSpawnHandler(skullSpawner, spawnHandlerDetails);
            Debug.Log("Spawned BirdEnemy with Keys: " + _spawnHandlerKey);
            spawnObject = _poolManager.AddToSpawnPool(skull, _spawnHandlerKey);
            _invokedSpawn = true;
        }

        StartCoroutine(Spawn());
    }

    public override void StartMethod() {
        originalFlapHeight = transform.position.y;

        targetEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseEntity>();
        direction = (targetEntity.transform.position.x < transform.position.x) ? 1 : -1;

        base.StartMethod();
    }

    public override void UpdateMethod ()
    {
        // flap
        Vector3 newPosition = transform.position;
        flapIndex += Time.deltaTime;
        newPosition.y = flapHeight * Mathf.Sin(flapSpeed * flapIndex) + originalFlapHeight;
        transform.position = newPosition;
        if (direction == 1)
            MoveLeft();
        else if (direction == -1)
            MoveRight();

        if (faceCheckHit && faceCheckRaycastHit) {
            // suicide on attack
            StopCoroutine(Spawn());
            GetComponent<BaseHealth>().Die();
        }
    }

    //public void SpawnSkull()
    //{
    //    // Spawn skull
    //    GameObject spawnedSkullObj = SpawnItem(skull, skullSpawner.transform.position);
    //    spawnedSkullObj.GetComponent<Rigidbody2D>().AddForce(new Vector2(((direction == 1) ? -skullSpeedInitial : skullSpeedInitial), -3), ForceMode2D.Impulse);

    //    BaseEntity spawnedSkullEntity = spawnedSkullObj.GetComponent<BaseEntity>();
    //    spawnedSkullEntity.direction = direction;
    //}

    public IEnumerator Spawn() {
        while (true) {
            _poolManager.Spawn(spawnObject);
            spawnObject.gameObject.GetComponent<BaseEntity>().direction = direction;
            yield return new WaitForSeconds(skullSpawningInterval);
        }
    }
}
