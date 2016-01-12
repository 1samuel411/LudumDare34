using UnityEngine;
using System.Collections;

public class SkullEnemy : BaseEntity
{

    private bool changedDirection = false;
    public int damage;

    public float skullSpeed; //remember regSpeed
    public float skullSpeedInitial;

    public override void Awake()
    {
        base.Awake();
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(((direction == 1) ?
                -skullSpeedInitial : skullSpeedInitial), -3), ForceMode2D.Impulse);
        base.AwakeMethod();
    }

    public override void StartMethod() {
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
