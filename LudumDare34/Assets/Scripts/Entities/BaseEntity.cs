using UnityEngine;
using System.Collections;
using System.Linq;

public class BaseEntity : MonoBehaviour
{

    public BaseEntity targetEntity;

    // 1 is left || -1 is right
    public int direction = 1;

    // Face
    public bool faceCheck = false;
    public LayerMask faceCheckMask;
    public Transform faceTransform;
    public bool faceCheckHit;
    public float faceCheckDist;
    [HideInInspector]
    public RaycastHit2D faceCheckRaycastHit;

    // Moving
    public float jumpSpeed;
    public float jumpHeight;

    public float speed;
    public float maxVelocity;
    [HideInInspector]
    public float regSpeed;
    
    // Jumping
    public float jumpCooldownTime;
    public float airTimeNeeded = 0.3f;
    public float airTimeNeededToBoostDown = 1.2f;
    [HideInInspector]
    public float airTime;
    [HideInInspector]
    public float targetAirTime;
    [HideInInspector]
    public float targetAirTimeBoost;
    [HideInInspector]
    public float curJumpCooldownTime;
    [HideInInspector]
    public float totalJumpCooldownTime;

    // Grounded
    public LayerMask groundedMask;
    public bool grounded;
    public float groundCheckDist;
    private RaycastHit2D _groundHit;

    // Variables
    public bool canMove = true;
    public bool canJump = true;
    public bool canScale = true;
    public bool knockedBack = false;
    public bool isJumping = false;
    public bool isBoosting = false;
    private bool _firstTrigger = false;

    private float _targetRecoverTime;
    private float _currentRecoverTime;
    protected extWepPoolManager _poolManager;

    [HideInInspector]
    public BaseHealth baseHealth;
    [HideInInspector]
    public new Rigidbody2D rigidbody;
    [HideInInspector]
    public new Transform transform;

    public virtual void Awake()
    {
        regSpeed = speed;
        rigidbody = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        baseHealth = GetComponent<BaseHealth>();

        _poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<extWepPoolManager>();
        AwakeMethod();
    }

    public virtual void OnEnable()
    {
        airTime = targetAirTime = targetAirTimeBoost = 0;
        curJumpCooldownTime = totalJumpCooldownTime= 0;
        faceCheckHit = false;
        canJump = true;
        isJumping = false;
        grounded = false;
    }

    public virtual void OnDisable() {
        if(_firstTrigger)
            if (SpawnWeapon()) {
                SpawnObject obj = _poolManager.RandomWeapon();
                _poolManager.SpawnAt(obj, this.transform);
            }
        if(!_firstTrigger)
            _firstTrigger = true;
    }

    protected bool SpawnWeapon() {
        bool bOk = false;
        float num = Random.Range(0.0f, 1.0f);
        //Percent chance of spawning a weapon.
        if (num < 0.25f)
            bOk = true;
        return bOk;
    }

    public virtual void AwakeMethod() { }

    public void Start()
    {
        StartMethod();
        targetEntity = LevelManager.instance.player;
    }

    public virtual void StartMethod() { }

    public void Update()
    {
        if (baseHealth)
        {
            if (baseHealth._died)
            {
                //rigidbody.isKinematic = true;
                return;
            }
        }

        // Check Face
        if (faceCheck)
        {
            faceCheckHit = FaceHitCheck();
        }

        // Check grounded
        grounded = Grounded();

        // Update our scale based on direction
        if (canScale)
        {
            transform.localScale = new Vector3(direction, 1, 1);
        }

        // Restrict speed
        if (rigidbody.velocity.magnitude > maxVelocity)
        {
            speed = 0;
        }
        else
        {
            speed = regSpeed;
        }

        // Stop jumping when we hit the ground if we are jumping and we've been in the air long enough
        if (grounded && isJumping && airTime >= targetAirTime) {
            FinishJump();
        }

        if (!grounded)
        {
            airTime += Time.deltaTime;
        }

        if (!canJump)
            curJumpCooldownTime += Time.deltaTime;

        if (curJumpCooldownTime > totalJumpCooldownTime && !isJumping && !canJump)
        {
            canJump = true;
        }

        _currentRecoverTime += Time.deltaTime;
        if(_currentRecoverTime > _targetRecoverTime && knockedBack && !canMove)
        {
            knockedBack = false;
            canMove = true;
            canJump = true;
            canScale = true;
        }

        UpdateMethod();
    }

    public virtual void UpdateMethod() { }

    public bool Grounded()
    {
        _groundHit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, groundedMask);

        if (_groundHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool FaceHitCheck()
    {
        faceCheckRaycastHit = Physics2D.Raycast(faceTransform.position, Vector2.left, faceCheckDist, faceCheckMask);

        if (faceCheckRaycastHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MoveLeft()
    {
        if(canScale)
            direction = 1;
        if(canMove)
            rigidbody.AddForce(new Vector2(-speed, 0) * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void MoveRight()
    {
        if(canScale)
            direction = -1;
        if(canMove)
            rigidbody.AddForce(new Vector2(speed, 0) * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void Jump()
    {
        // Disable movement
        canJump = false;
        isJumping = true;

        airTime = Time.time;
        targetAirTime = Time.time + airTimeNeeded;
        targetAirTimeBoost = Time.time + airTimeNeededToBoostDown;

        // Give air boost
        rigidbody.AddForce(new Vector2(0, jumpHeight * jumpSpeed) * Time.deltaTime / Time.timeScale, ForceMode2D.Impulse);
    }

    public void BoostDown(float modifier = 1)
    {
        isBoosting = true;
        // Give air boost
        rigidbody.AddForce(new Vector2(0, -jumpHeight * jumpSpeed * modifier) * Time.deltaTime / Time.timeScale, ForceMode2D.Impulse);
    }

    void FinishJump()
    {
        totalJumpCooldownTime = Time.time + jumpCooldownTime;
        curJumpCooldownTime = Time.time;
        canMove = true;
        isJumping = false;
    }

    public void Knockback(float force, float recoverTime, int inputDir)
    {
        _targetRecoverTime = Time.time + recoverTime;
        _currentRecoverTime = Time.time;
        canMove = false;
        canJump = false;
        canScale = false;
        knockedBack = true;
        if(inputDir != direction)
            rigidbody.AddForce(new Vector2((direction == 1) ? force : -force, 0), ForceMode2D.Impulse);
        else
            rigidbody.AddForce(new Vector2((direction == 1) ? -force : force, 0), ForceMode2D.Impulse);
    }
}
