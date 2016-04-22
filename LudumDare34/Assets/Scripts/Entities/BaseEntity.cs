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
    public float originalMaxVelocity;
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
    public bool takeDamage = true;
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
    [HideInInspector]
    public new AudioSource audio;

    public AudioClip[] soundEffects;

    public virtual void Awake()
    {
        originalMaxVelocity = maxVelocity;
        regSpeed = speed;
        rigidbody = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        baseHealth = GetComponent<BaseHealth>();
        audio = GetComponent<AudioSource>();

        _poolManager = GameObject.FindGameObjectWithTag("PoolManager").GetComponent<extWepPoolManager>();
        AwakeMethod();

        if(baseHealth.type != BaseHealth.Type.player)
        {
            InvokeRepeating("PlayRandomSoundEffect", Random.Range(4.0f, 8.0f), Random.Range(4.0f, 8.0f));
        }
    }

    public void PlayRandomSoundEffect()
    {
        if(gameObject.activeSelf)
            audio.PlayOneShot(soundEffects[Random.Range(0, soundEffects.Length)]);
    }

    public virtual void OnEnable()
    {
        airTime = targetAirTime = targetAirTimeBoost = 0;
        curJumpCooldownTime = totalJumpCooldownTime= 0;
        faceCheckHit = false;
        canJump = true;
        isJumping = false;
        grounded = false;
        AwakeMethod();
        StartMethod();
    }

    public virtual void OnDisable() {

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

        airTime += Time.deltaTime;

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
        faceCheckRaycastHit = Physics2D.Raycast(faceTransform.position, (direction == 1) ? Vector2.left : Vector2.right, faceCheckDist, faceCheckMask);

        if (faceCheckRaycastHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MoveLeft(bool changeDir = true, int targetDir = 0, float speed = -1)
    {
        if(changeDir && canScale)
            direction = (targetDir == 0) ? 1 :  targetDir;
        if(canMove)
            rigidbody.AddForce(new Vector2(-((speed == -1) ? this.speed : speed), 0) * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void MoveRight(bool changeDir = true, int targetDir = 0, float speed = -1)
    {
        if(changeDir && canScale)
            direction = (targetDir == 0) ? -1 :  targetDir;
        if(canMove)
            rigidbody.AddForce(new Vector2((speed == -1) ? this.speed : speed, 0) * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void Jump()
    {
        // Disable movement
        canJump = false;
        isJumping = true;

        airTime = Time.time;
        targetAirTime = Time.time + airTimeNeeded;
        targetAirTimeBoost = Time.time + airTimeNeededToBoostDown;

        float amount = (jumpHeight * jumpSpeed);
        // Give air boost
        rigidbody.AddForce(new Vector2(0, amount) , ForceMode2D.Impulse);
    }

    public void BoostDown(float modifier = 1)
    {
        StartCoroutine(BoostCooldown());
        isBoosting = true;
        // Give air boost
        float amount = (-jumpHeight * jumpSpeed);
        rigidbody.AddForce(new Vector2(0, amount * modifier), ForceMode2D.Impulse);
    }

    IEnumerator BoostCooldown()
    {
        takeDamage = false;
        yield return new WaitForSeconds(1);
        takeDamage = true;
    }

    void FinishJump()
    {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
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
