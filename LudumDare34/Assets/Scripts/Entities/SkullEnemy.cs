﻿using UnityEngine;
using System.Collections;

public class SkullEnemy : BaseEntity
{

    private bool changedDirection = false;
    public int damage;

    public override void StartMethod()
    {
        targetEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseEntity>();
        
        Jump();
    }

    public override void UpdateMethod()
    {
        if (grounded)
        {
            if (direction == 1)
                MoveLeft();
            else if (direction == -1)
                MoveRight();
        }

        if(!changedDirection && grounded)
        {
            changedDirection = true;
            if (targetEntity.transform.position.x < transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        }

        if(grounded && canJump)
        {
            Jump();
        }

        if (faceCheckHit && faceCheckRaycastHit)
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
