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

    public int damage = 2;

    private new Rigidbody2D rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (listenForInfo)
        {
            if (collider.tag != "Player")
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
                }
            }
        }
    }
}
