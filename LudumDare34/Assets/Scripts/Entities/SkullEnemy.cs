using UnityEngine;
using System.Collections;

public class SkullEnemy : BaseEntity
{

    private bool changedDirection = false;
    private bool hitFloorOnce = false;
    public int damage;

    public float skullSpeed; //remember regSpeed
    public float skullSpeedInitial;

    public override void OnEnable() {
        transform.localScale = new Vector3(direction, 1, 1);
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(((direction == 1) ?
                -skullSpeedInitial : skullSpeedInitial), -3), ForceMode2D.Impulse);
        
        changedDirection = false;
        hitFloorOnce = false;
        if (!baseHealth)
            baseHealth = GetComponent<BaseHealth>();
        
        base.OnEnable();
    }

    public override void UpdateMethod()
    {
        if (grounded) {
            hitFloorOnce = true;
            if (direction == 1)
                MoveLeft();
            else if (direction == -1)
                MoveRight();
        }

        if(!changedDirection && grounded && hitFloorOnce) {
            changedDirection = true;
            if (targetEntity.transform.position.x < transform.position.x) {
                direction = 1;
            } else {
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
            baseHealth.zoomable = false;
            baseHealth.addScore = false;
            baseHealth.Die();
        }
    }
}
