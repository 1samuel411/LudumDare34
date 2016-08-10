using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : BaseEntity {
    // Moving
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode toggleWeaponKey;
    public float holdTimeJump, holdTimeMove, holdTimeStrafe;
    private float _curHoldTimeJump, _curHoldTimeRight, _curHoldTimeLeft;
    private float _targetHoldTimeJump, _targetHoldTimeRight, _targetHoldTimeLeft, _targetHoldTimeStrafe;

    // Hardlanding
    public GameObject hardLandingExplosion;
    public float hardLandingSize = 3;
    public float hardLandingForce = 3;

    // Landing
    public bool landed;

    // Toggling
    public float toggleTimer = 0.5f;
    public float toggleTime;
    public bool canToggleWeapon = true;
    public bool cinematic = false;

    // Moving
    private bool _holdingLeftKey;
    private bool _holdingRightKey;
    private bool _holdingStrafeLeftKey;
    private bool _holdingStrafeRightKey;
    private bool _holdingKeys;
    private bool _strafing;
    private bool _strafingAnim;

    public int boostDamage = 1;

    public PlayerWeaponHandler weaponHandler;
    public BaseWeapon weapon;
    public Animator animator;

    public static PlayerController instance;

    public override void AwakeMethod()
    {
        animator = GetComponentInChildren<Animator>();
        instance = this;

        if (weaponHandler == null)
            weaponHandler = this.GetComponentInChildren<PlayerWeaponHandler>();

        if (!TouchController.initialized)
            TouchController.Initialize();
    }

    public override void StartMethod()
    {
        weaponHandler.AddFirstWeapon(ref weapon);
        if(VoiceManager.instance)
            VoiceManager.instance.PlayStartSound();
    }

    public override void UpdateMethod()
    {
        if (!gotItems && LevelManager.instance)
        {
            GetItems();
        }
        // Animator
        animator.SetBool("grounded", grounded);
        animator.SetFloat("yvelocity", rigidbody.velocity.y);
        if (_holdingLeftKey || _holdingRightKey || _strafing)
            animator.SetFloat("xvelocity", rigidbody.velocity.x);
        else
            animator.SetFloat("xvelocity", 0);
        if (weapon.holdType == BaseWeapon.HoldType.Onehanded)
        {
            animator.SetBool("oneHanded", true);
        }
        else
        {
            animator.SetBool("oneHanded", false);
        }
        animator.SetBool("strafing", _strafingAnim);
        if (baseHealth._died || cinematic)
            return;

        // Hard landing
        if (isBoosting && grounded)
        {
            isBoosting = false;
            Instantiate(hardLandingExplosion, transform.position, Quaternion.identity);

            // Hurt enemies
            RaycastHit2D[] leftBox = Physics2D.BoxCastAll(new Vector2(transform.position.x - hardLandingSize, transform.position.y), new Vector2(hardLandingSize, 5), 0, Vector2.left);
            RaycastHit2D[] rightBox = Physics2D.BoxCastAll(new Vector2(transform.position.x + hardLandingSize, transform.position.y), new Vector2(hardLandingSize, 5), 0, Vector2.right);
            for (int i = 0; i < leftBox.Length; i++)
            {
                Vector3 distance = leftBox[i].transform.position - transform.position;
                if (distance.magnitude < (hardLandingSize * hardLandingSize))
                {
                    if (leftBox[i].collider.tag == "Enemy")
                    {
                        BaseHealth enemyHealth = leftBox[i].collider.GetComponent<BaseHealth>();
                        if (enemyHealth._died == false)
                        {
                            BaseEntity enemyEntity = leftBox[i].collider.GetComponent<BaseEntity>();
                            enemyEntity.Knockback(hardLandingForce, 0.2f, 1);
                            enemyHealth.DealDamage(boostDamage);
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
                            enemyHealth.DealDamage(boostDamage);
                        }
                    }
                }
            }
        }

        // Check left key
        if (Input.GetKey(leftKey) || TouchController.controller.GetTouch(TouchLocations.Left, 250))
        {
            if (_holdingRightKey)
            {
                // If this is the first time holding it then set the hold time
                if (!_holdingStrafeLeftKey)
                {
                    _targetHoldTimeStrafe = Time.time + holdTimeStrafe;
                }

                _holdingStrafeLeftKey = true;

                // Need to hold these keys for a certain time to allow left movement
                if (Time.time > _targetHoldTimeStrafe)
                {
                    // Move left
                    _holdingRightKey = false;
                    _strafing = true;
                    _strafingAnim = true;
                    maxVelocity = originalMaxVelocity * 0.6f;
                    MoveRight(true, 1);
                    if (Tutorial.instance)
                        Tutorial.instance.CompleteStage("StrafeRight");
                }
            }
            else
            {
                _holdingStrafeLeftKey = false;
                // If this is the first time holding it then set the hold time
                if (!_holdingLeftKey || _holdingStrafeRightKey)
                {
                    _strafing = false;
                    if (!_holdingLeftKey)
                    {
                        _curHoldTimeLeft = Time.time;
                        _targetHoldTimeLeft = Time.time + holdTimeMove;
                    }
                }

                _holdingLeftKey = true;

                // Need to hold these keys for a certain time to allow left movement
                if (_curHoldTimeLeft > _targetHoldTimeLeft)
                {
                    // Move left
                    if (!_strafing)
                    {
                        maxVelocity = originalMaxVelocity;
                        _strafingAnim = false;
                        MoveLeft();
                        if (Tutorial.instance)
                            Tutorial.instance.CompleteStage("MoveLeft");
                    }
                }
                else
                {
                    _curHoldTimeLeft += Time.deltaTime;
                }
            }
        }
        else
        {
            _holdingLeftKey = false;
        }

        // Check right key
        if (Input.GetKey(rightKey) || TouchController.controller.GetTouch(TouchLocations.Right, 250))
        {
            if (_holdingLeftKey)
            {
                // If this is the first time holding it then set the hold time
                if (!_holdingStrafeRightKey)
                {
                    _targetHoldTimeStrafe = Time.time + holdTimeStrafe;
                }

                _holdingStrafeRightKey = true;

                // Need to hold these keys for a certain time to allow left movement
                if (Time.time >= _targetHoldTimeStrafe)
                {
                    // Move left
                    _holdingLeftKey = false;
                    _strafing = true;
                    _strafingAnim = true;
                    maxVelocity = originalMaxVelocity * 0.6f;
                    MoveLeft(true, -1);
                    if (Tutorial.instance)
                        Tutorial.instance.CompleteStage("StrafeLeft");
                    return;
                }
            }
            else
            {
                _holdingStrafeRightKey = false;
                // If this is the first time holding it then set the hold time
                if (!_holdingRightKey || _holdingStrafeLeftKey)
                {
                    _strafing = false;
                    if (!_holdingRightKey)
                    {
                        _curHoldTimeRight = Time.time;
                        _targetHoldTimeRight = Time.time + holdTimeMove;
                    }
                }

                _holdingRightKey = true;

                // Need to hold these keys for a certain time to allow right movement
                if (_curHoldTimeRight >= _targetHoldTimeRight)
                {
                    // Move right
                    if (!_strafing)
                    {
                        maxVelocity = originalMaxVelocity;
                        _strafingAnim = false;
                        MoveRight();
                        if (Tutorial.instance)
                            Tutorial.instance.CompleteStage("MoveRight");
                    }
                }
                else
                {
                    _curHoldTimeRight += Time.deltaTime;
                }
            }
        }
        else
        {
            _holdingRightKey = false;
        }

        if (Tutorial.instance)
            if (Tutorial.instance.stages[Tutorial.instance.currentStage].name == "StrafeLeft" && !_holdingLeftKey)
            {
                Tutorial.instance.GoToStage("MoveLeft");
            }

        if (Tutorial.instance)
            if (Tutorial.instance.stages[Tutorial.instance.currentStage].name == "StrafeRight" && !_holdingRightKey)
            {
                Tutorial.instance.GoToStage("MoveRight");
            }

        // Jumping
        if (canJump && !isJumping)
        {
            // Check both keys
            if (Input.GetKey(jumpKey))
            {
                // If this is the first time holding it then set the hold time
                if (!_holdingKeys)
                {
                    _curHoldTimeJump = Time.time;
                    _targetHoldTimeJump = Time.time + holdTimeJump;
                }

                _holdingKeys = true;

                MoveLeft(false);
                MoveRight(false);
                // Need to hold these keys for a certain time to allow jumping
                if (_curHoldTimeJump > _targetHoldTimeJump)
                {
                    // Jump
                    animator.SetTrigger("jump");
                    rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                    Jump();
                    
                    landed = false;
                    _holdingKeys = false;

                    if (Tutorial.instance)
                        Tutorial.instance.CompleteStage("Jump");
                }
                else
                {
                    _curHoldTimeJump = Time.time;
                }
            }
            else
            {
                _holdingKeys = false;
            }

            if (TouchController.controller.GetSwipe(SwipeLocations.Up))
            {
                // Jump
                animator.SetTrigger("jump");
                rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                Jump();
                
                landed = false;
                _holdingKeys = false;

                if (Tutorial.instance)
                    Tutorial.instance.CompleteStage("Jump");
            }
        }

        // Boost down
        if (isJumping && airTime > targetAirTimeBoost && !isBoosting) {
            // Check both keys
            if (Input.GetKey(jumpKey)) {
                // If this is the first time holding it then set the hold time
                if (!_holdingKeys) {
                    _curHoldTimeJump = Time.time;
                    _targetHoldTimeJump = Time.time + holdTimeJump;
                }

                _holdingKeys = true;

                // Need to hold these keys for a certain time to allow jumping
                if (_curHoldTimeJump > _targetHoldTimeJump) {
                    // Jump
                    BoostDown(2);
                    if (Tutorial.instance)
                        Tutorial.instance.CompleteStage("Boost");
                    _holdingKeys = false;
                } else {
                    _curHoldTimeJump = Time.time;
                }

            } else {
                _holdingKeys = false;
            }

            if (TouchController.controller.GetSwipe(SwipeLocations.Down)) {
                // Jump
                BoostDown(2);
                if (Tutorial.instance)
                    Tutorial.instance.CompleteStage("Boost");
                _holdingKeys = false;
            }
        }

        if (Tutorial.instance)
        {
            if (Tutorial.instance.stages[Tutorial.instance.currentStage].name == "Boost" && grounded)
            {
                Tutorial.instance.GoToStage("Jump");
            }
        }
        if(Input.GetKeyDown(toggleWeaponKey) || TouchController.controller.GetTouchUp(TouchLocations.Down, 250, 120) && canToggleWeapon)
        {
            if (Tutorial.instance)
            {
                if (Tutorial.instance.stages[Tutorial.instance.currentStage].name != "WeaponToggle" && !Tutorial.instance.finishedTutorial)
                    return;
                Tutorial.instance.CompleteStage("WeaponToggle");
            }
            toggleTimer = toggleTime + Time.time;
            LevelManager.instance.wepsEnabled = !LevelManager.instance.wepsEnabled;
        }

        //change this to delegate call!
        if (weapon.wepEnabled)
            if (!weapon.weaponAttribute.CheckIfWeaponAvailable())
            {
                EquipNextWeapon(weapon);
            }
    }

    public void EquipNextWeapon(BaseWeapon newWeapon)
    {
        weaponHandler.PickedUpWeapon(newWeapon);
        weapon = weaponHandler.GetNextWeapon(weapon);
    }

    private bool gotItems;
    public void GetItems()
    {
        //gotItems = true;
        //LevelManager.instance.boughtItems = Shop.GetBoughtItems(InfoManager.GetInfo("bought"));
        //foreach (var shopItem in Shop.itemDatabase) {
        //    LevelManager.instance.boughtItems
        //}
        //for(int i = 0; i <Shop.itemDatabase.Length; i ++)
        //{
        //    if(LevelManager.instance.boughtItems.Contains(i))
        //    {
        //        int timesBought = 0;
        //        for (int x = 0; x < LevelManager.instance.boughtItems.Count; x++)
        //        {
        //            if (LevelManager.instance.boughtItems[x] == i)
        //                timesBought++;
        //        }

        //        if (Shop.itemDatabase[i].itemType == GameShopItem.ItemType.health)
        //        {
        //            LevelManager.instance.healthAddition += (timesBought * Shop.itemDatabase[i].multiplyer);
        //        }
        //        if(Shop.itemDatabase[i].itemType == GameShopItem.ItemType.ammo)
        //        {
        //            LevelManager.instance.ammoAddition = (timesBought * Shop.itemDatabase[i].multiplyer);
        //        }
        //        if(Shop.itemDatabase[i].itemType == GameShopItem.ItemType.timer)
        //        {
        //            LevelManager.instance.timeAddition = (timesBought * Shop.itemDatabase[i].multiplyer);
        //        }
        //        if(Shop.itemDatabase[i].itemType == GameShopItem.ItemType.damageBoost)
        //        {
        //            LevelManager.instance.boostDamageAddition = (timesBought * Shop.itemDatabase[i].multiplyer);
        //        }
        //        if(Shop.itemDatabase[i].itemType == GameShopItem.ItemType.damagePistol)
        //        {
        //            LevelManager.instance.pistolDamageAddition = (timesBought * Shop.itemDatabase[i].multiplyer);
        //        }
        //    }
        //}
        //baseHealth.maxHealth += LevelManager.instance.healthAddition;
        //baseHealth.currentHealth = baseHealth.maxHealth;
        //boostDamage += LevelManager.instance.boostDamageAddition;
    }
}