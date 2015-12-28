using UnityEngine;
using System.Collections;

public class BirdEnemy : BaseEntity
{

    public GameObject skull;
    public Transform skullSpawner;
    public float skullSpeedInitial;
    public float skullSpeed;

    public float skullSpawningInterval;
    private float skullSpawningNeededTime;

    public float flapSpeed;
    public float flapHeight;

    private float originalFlapHeight;
    private float flapIndex;

    public override void StartMethod()
    {
        originalFlapHeight = transform.position.y;

        skullSpawningNeededTime = skullSpawningInterval + Time.time;

        targetEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseEntity>();
        if (targetEntity.transform.position.x < transform.position.x)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
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

        // Wait interval to spawn skull
        if (Time.time > skullSpawningNeededTime)
        {
            SpawnSkull();
            skullSpawningNeededTime = skullSpawningInterval + Time.time;
        }

        if (faceCheckHit && faceCheckRaycastHit)
        {
            // suicide on attack
            GetComponent<BaseHealth>().Die();
        }
    }

    public void SpawnSkull()
    {
        // Spawn skull
        GameObject spawnedSkullObj = SpawnItem(skull, skullSpawner.position);
        spawnedSkullObj.GetComponent<Rigidbody2D>().AddForce(new Vector2(((direction == 1) ? -skullSpeedInitial : skullSpeedInitial), -3), ForceMode2D.Impulse);

        BaseEntity spawnedSkullEntity = spawnedSkullObj.GetComponent<BaseEntity>();
        spawnedSkullEntity.direction = direction;
        spawnedSkullEntity.speed = skullSpeed;
        spawnedSkullEntity.regSpeed = skullSpeed;
    }
}
