using UnityEngine;
using System.Collections;

public class AxeLogic : MonoBehaviour
{

    private bool _beingThrown;
    public bool beingThrown
    {
        get
        {
            return _beingThrown;
        }
        set
        {
            if (value)
                listenForInfo = true;
            else
                listenForInfo = false;
            _beingThrown = value;
        }
    }
    public bool listenForInfo
    {
        get
        {
            return _listenForInfo;
        }
        set
        {
            if (value)
                hitPlayer = false;
            _listenForInfo = value;
        }
    }
    private bool _listenForInfo;

    private bool hitPlayer;

    private new Rigidbody2D rigidbody;

    public BossEnemy boss;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (listenForInfo)
        {
            if (collider.tag != "Player" && beingThrown)
            {
                listenForInfo = false;
                rigidbody.isKinematic = true;
            }
            else
            {
                if(!hitPlayer)
                {
                    hitPlayer = true;
                    // Attack
                    boss.targetEntity.Knockback(10, 0.2f, boss.direction);
                    boss.targetEntity.baseHealth.DealDamage(boss.damage);
                }
            }
        }
    }
}
