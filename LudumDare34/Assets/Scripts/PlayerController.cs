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

    // Hardlanding
    public GameObject hardLandingExplosion;
    public float hardLandingSize = 3;
    public float hardLandingForce = 3;

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

        if(weaponHandler == null)
            weaponHandler = this.GetComponentInChildren<PlayerWeaponHandler>();

        weaponHandler.AddFirstWeapon(ref weapon);
    }

    public override void UpdateMethod()
    {
        // Hard landing
        if(isBoosting && grounded)
        {
            isBoosting = false;
            Instantiate(hardLandingExplosion, transform.position, Quaternion.identity);

            // Hurt enemies
            RaycastHit2D[] leftBox = Physics2D.BoxCastAll(new Vector2(transform.position.x - hardLandingSize, transform.position.y), new Vector2(hardLandingSize, 5), 0, Vector2.left);
            RaycastHit2D[] rightBox = Physics2D.BoxCastAll(new Vector2(transform.position.x + hardLandingSize, transform.position.y), new Vector2(hardLandingSize, 5), 0, Vector2.right);
            for (int i = 0; i < leftBox.Length; i++)
            {
                Vector3 distance = leftBox[i].transform.position - transform.position;
                if(distance.magnitude < (hardLandingSize * hardLandingSize))
                {
                    if (leftBox[i].collider.tag == "Enemy")
                    {
                        BaseHealth enemyHealth = leftBox[i].collider.GetComponent<BaseHealth>();
                        if (enemyHealth._died == false)
                        {
                            BaseEntity enemyEntity = leftBox[i].collider.GetComponent<BaseEntity>();
                            enemyEntity.Knockback(hardLandingForce, 0.2f, 1);
                            enemyHealth.DealDamage(1);
                        }
                    }
                }
            }

            for (int i = 0; i < rightBox.Length; i++)
            {
                Vector3 distance = rightBox[i].transform.position - transform.position;
                if (distance.magnitude < (hardLandingSize * hardLandingSize))
                {
                    if (rightBox[i].collider.tag == "Enemy")
                    {
                        BaseHealth enemyHealth = rightBox[i].collider.GetComponent<BaseHealth>();
                        if (enemyHealth._died == false)
                        {
                            BaseEntity enemyEntity = rightBox[i].collider.GetComponent<BaseEntity>();
                            enemyEntity.Knockback(hardLandingForce, 0.2f, -1);
                            enemyHealth.DealDamage(1);
                        }
                    }
                }
            }
        }

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
