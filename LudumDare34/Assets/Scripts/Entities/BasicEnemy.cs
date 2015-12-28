using UnityEngine;
using System.Collections;

public class BasicEnemy : BaseEntity
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

    public override void UpdateMethod ()
    {
        if (direction == 1)
            MoveLeft();
        else if (direction == -1)
            MoveRight();

        if(faceCheckHit && faceCheckRaycastHit)
        {
            BaseHealth healthFace = null;
            healthFace = faceCheckRaycastHit.transform.GetComponent<BaseHealth>();
            if (healthFace)
                healthFace.DealDamage(damage);
            // suicide on attack
            GetComponent<BaseHealth>().Die();
        }
    }
}
