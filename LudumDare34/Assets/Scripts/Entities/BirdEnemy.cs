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
    private bool _invokedSpawn = false;
    public SpawnHandlerDetails spawnHandlerDetails;
    public SpawnObject spawnObject;

    public override void Awake() {
        base.Awake();

        if (!_invokedSpawn) {
            spawnObject = _poolManager.AddToSpawnPool(skull);
            _invokedSpawn = true;
        }
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
            baseHealth.zoomable = false;
            GetComponent<BaseHealth>().Die();
        }
    }

    public override void OnEnable() {
        base.OnEnable();

        if(_invokedSpawn)
            StartCoroutine(Spawn());
    }

    public void OnDisable() {
        StopCoroutine(Spawn());
    }

    public IEnumerator Spawn() {
        while (true) {
            _poolManager.SpawnAt(spawnObject, skullSpawner.transform);
            spawnObject.gameObject.GetComponent<BaseEntity>().direction = direction;
            yield return new WaitForSeconds(skullSpawningInterval);
        }
    }
}
