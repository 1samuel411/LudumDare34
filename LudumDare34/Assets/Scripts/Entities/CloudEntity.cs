using UnityEngine;
using System.Collections;

public class CloudEntity : BaseEntity
{

    public int damage;

    public override void StartMethod()
    {
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

    public override void UpdateMethod()
    {
        if (direction == 1)
        {
            MoveLeft();
            rigidbody.velocity = new Vector2(-maxVelocity, 0);
        }
        else if (direction == -1)
        {
            rigidbody.velocity = new Vector2(maxVelocity, 0);
        }

        if (faceCheckHit && faceCheckRaycastHit)
        {
            // suicide on attack
            baseHealth.zoomable = false;
            baseHealth.Die();
        }
    }
}
