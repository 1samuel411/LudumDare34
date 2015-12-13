using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : BaseEntity
{

    // Moving
    public KeyCode leftKey;
    public KeyCode rightKey;
    public float holdTimeJump, holdTimeMove;
    private float _curHoldTimeJump, _curHoldTimeRight, _curHoldTimeLeft;
    private float _targetHoldTimeJump, _targetHoldTimeRight, _targetHoldTimeLeft;

    // Moving
    private bool _holdingLeftKey;
    private bool _holdingRightKey;
    private bool _holdingKeys;

    public PlayerWeaponHandler weaponHandler;
    public BaseWeapon weapon;

    public static PlayerController instance;

    public override void AwakeMethod()
    {
        instance = this;
    }

    public void Start()
    {
        weapon.ActivateGun(true);
        if(weaponHandler == null)
            weaponHandler = this.GetComponentInChildren<PlayerWeaponHandler>();
    }

    public override void UpdateMethod()
    {
        // Check left key
        if (Input.GetKey(leftKey) && !Input.GetKey(rightKey))
        {
            // If this is the first time holding it then set the hold time
            if (!_holdingLeftKey)
            {
                _curHoldTimeLeft = Time.time;
                _targetHoldTimeLeft = Time.time + holdTimeMove;
            }

            _holdingLeftKey = true;

            // Need to hold these keys for a certain time to allow left movement
            if (_curHoldTimeLeft > _targetHoldTimeLeft)
            {
                // Move left
                MoveLeft();
            }
            else
            {
                _curHoldTimeLeft += Time.deltaTime / Time.timeScale;
            }
        }
        else
        {
            _holdingLeftKey = false;
        }

        // Check right key
        if (Input.GetKey(rightKey) && !Input.GetKey(leftKey))
        {
            // If this is the first time holding it then set the hold time
            if (!_holdingRightKey)
            {
                _curHoldTimeRight = Time.time;
                _targetHoldTimeRight = Time.time + holdTimeMove;
            }

            _holdingRightKey = true;

            // Need to hold these keys for a certain time to allow right movement
            if (_curHoldTimeRight > _targetHoldTimeRight)
            {
                // Move right
                MoveRight();
            }
            else
            {
                _curHoldTimeRight += Time.deltaTime / Time.timeScale;
            }
        }
        else
        {
            _holdingRightKey = false;
        }


        // Jumping
        if (canJump && !isJumping)
        {
            // Check both keys
            if (Input.GetKey(leftKey) && Input.GetKey(rightKey))
            {
                // If this is the first time holding it then set the hold time
                if (!_holdingKeys)
                {
                    _curHoldTimeJump = Time.time;
                    _targetHoldTimeJump = Time.time + holdTimeJump;
                }

                _holdingKeys = true;

                // Need to hold these keys for a certain time to allow jumping
                if (_curHoldTimeJump > _targetHoldTimeJump)
                {
                    // Jump
                    Jump();
                    _holdingKeys = false;
                }
                else
                {
                    _curHoldTimeJump += Time.deltaTime / Time.timeScale;
                }

            }
            else
            {
                _holdingKeys = false;
            }
        }

        // Boost down
        if (isJumping && airTime > targetAirTimeBoost)
        {
            // Check both keys
            if (Input.GetKey(leftKey) && Input.GetKey(rightKey))
            {
                // If this is the first time holding it then set the hold time
                if (!_holdingKeys)
                {
                    _curHoldTimeJump = Time.time;
                    _targetHoldTimeJump = Time.time + holdTimeJump;
                }

                _holdingKeys = true;

                // Need to hold these keys for a certain time to allow jumping
                if (_curHoldTimeJump > _targetHoldTimeJump)
                {
                    // Jump
                    BoostDown(2);
                    _holdingKeys = false;
                }
                else
                {
                    _curHoldTimeJump += Time.deltaTime / Time.timeScale;
                }

            }
            else
            {
                _holdingKeys = false;
            }
        }

        //change this to delegate call!
        if (!weapon.weaponAttribute.CheckIfWeaponAvailable()) {
            EquipNextWeapon(weapon);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        Debug.Log("Collider + "  + collider.tag);
        if(string.CompareOrdinal(collider.tag, "Weapon") == 0) {
            Debug.Log("Found Weapon!");
            EquipNextWeapon(collider.GetComponent<BaseWeapon>());
        }
    }

    public void EquipNextWeapon(BaseWeapon newWeapon)
    {
        weaponHandler.PickedUpWeapon(newWeapon);
        weapon = weaponHandler.GetNextWeapon(weapon);
    }
}
