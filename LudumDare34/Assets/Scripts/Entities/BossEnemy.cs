using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossEnemy : BaseEntity
{

    public int damage;
    public float distanceToTarget;

    public float distanceToStop;
    public float distanceToJump;
    public Vector2 distanceToAxe;

    public float spawnTime = 2;
    private float curSpawnTime = 2;
    public bool landed;
    public bool beganBeginning;

    public float axeSpeed;
    public float axeSpeedVertical;

    public Rigidbody2D axeRigidbody;

    public Animator animator;

    public AudioClip music;
    private AudioClip original_music;

    private Transform axeParent;
    private ObjectFollower axeFollower;

    public Transform[] feetTransform;
    public GameObject landEffect;

    [HideInInspector]
    public AxeLogic axeLogic;

    public AudioClip roarSound;

    public bool canThrow = true;
    private float curAxeThrowTime;
    public float axeThrowTime = 8;
    public float throwHealthMin;

    public bool hasAxe;
    private float axeDistance;
    public float minAxeDistance;

    public bool attacking;
    public bool canAttack = true;
    public float curAttackTime;
    public float attackTime = 2;
    public float curAxepullback;
    public float axePullback = 4;

    public bool pullingback = false;
    public float pullingbackForce = 200;
    private bool setPullbackTrigger;

    public Transform landingSprite;
    public float landingSpriteOldYOffset;
    private float landingSpriteNewXOffset;
    private bool landingActive;
    private float curLandingTime;
    private bool setLandingXOffset;

    public float smallJumpSpeed;
    public float smallJumpSpeedX;
    public float largeJumpSpeed;
    public bool largeJumping;
    public bool largeJumpingPrep;
    public float largeJumpTime;
    public int minLargeJumpHealth = 16000;
    private float curLargeJumpTime;

    public float newJumpHeight;

    public float landingForce;

    private bool _Began;

    public override void OnEnable()
    {
        if (!baseHealth)
            baseHealth = GetComponent<BaseHealth>();


        canScale = true;
        beganBeginning = false;
        landed = false;

        base.OnEnable();

        LevelManager.instance.wepsEnabled = false;
        LevelManager.instance.player.cinematic = true;

        axeParent = axeRigidbody.transform.parent;
        axeFollower = axeRigidbody.GetComponent<ObjectFollower>();
        axeLogic = axeRigidbody.GetComponent<AxeLogic>();
        hasAxe = true;
        pullingback = false;
        attacking = false;
        canThrow = true;
        canBeKnockedback = false;
        jumping = false;
        largeJumping = false;
        largeJumpingPrep = false;
        setLandingXOffset = false;
        axeLogic.boss = this;
        _Began = false;
    }

    public bool deathZoomedin;

    private List<ParticleSystem> landingEffects = new List<ParticleSystem>();

    public override void UpdateMethod()
    {
        Animate();

        if(Time.time >= curLandingTime && !grounded && !landed)
        {
            curLandingTime = Time.time + ((transform.position.y - landingSpriteOldYOffset) / 80);
            LandingToggle();
        }

        distanceToTarget = Mathf.Abs(transform.position.x - targetEntity.transform.position.x);
        if (!landed)
        {
            if (!setLandingXOffset)
            {
                setLandingXOffset = true;
                landingSpriteNewXOffset = transform.position.x;
            }
            landingSprite.position = new Vector2(landingSpriteNewXOffset, landingSpriteOldYOffset);
            direction = (transform.position.x - targetEntity.transform.position.x < 0) ? -1 : 1;
            scaleTransform.localScale = new Vector3(direction, 1, 1);

            if (!grounded)
                animator.SetBool("Began", false);
            if (grounded && !beganBeginning)
            {
                LandingToggle(true);
                beganBeginning = true;
                curSpawnTime = Time.time + spawnTime;
                if (_Began)
                    curSpawnTime -= 1.5f;
                original_music = LevelManager.instance.musicPlayer.clip;
                bool makeNewEffects = (landingEffects.Count <= 0) ? true : false;
                for (int i = 0; i < feetTransform.Length; i++)
                {
                    if (landEffect)
                    {
                        if (makeNewEffects)
                            landingEffects.Add((Instantiate(landEffect, feetTransform[i].position, feetTransform[i].rotation) as GameObject).GetComponent<ParticleSystem>());
                        else
                        {
                            landingEffects[i].transform.position = feetTransform[i].position;
                            landingEffects[i].gameObject.SetActive(false);
                            landingEffects[i].gameObject.SetActive(true);
                        }
                    }
                }
                if(!largeJumping)
                    CameraManager.ZoomIn(8, 3f, 4, 0.3f, transform.position, 5, spawnTime/1.5f);
                CameraManager.ShakeScreen(2, 1.2f);
                animator.SetTrigger("Land");

                // Player didn't move! Attack him
                if (distanceToTarget <= (distanceToStop + 0.4f) && _Began)
                {
                    targetEntity.Knockback(landingForce, 0.2f, ((transform.position.x - targetEntity.transform.position.x) < 0) ? -1 : 1);
                    targetEntity.baseHealth.DealDamage(damage + 1);
                }
            }

            if(Time.time >= curSpawnTime && beganBeginning)
            {
                if (!_Began)
                {
                    _Began = true;
                    LevelManager.instance.wepsEnabled = true;
                }
                LevelManager.instance.player.cinematic = false;

                animator.SetBool("Began", true);
                landed = true;
                largeJumping = false;
            }

            return;
        }

        if (baseHealth._died)
        {
            if(!deathZoomedin)
            {
                CameraManager.ZoomIn(8, 3f, 4, 0.3f, transform.position, 5, spawnTime / 1.5f);
                CameraManager.ShakeScreen(2, 1.2f);
                deathZoomedin = true;
            }
            return;
        }

        axeDistance = Mathf.Abs(axeRigidbody.position.x - axeFollower.original.position.x);

        if(pullingback)
        {
            axeRigidbody.transform.position = Vector3.MoveTowards(axeRigidbody.position, axeFollower.original.position, pullingbackForce * Time.deltaTime);
        }

        if (!pullingback && Time.time >= curAxepullback && !hasAxe && !setPullbackTrigger)
        {
            setPullbackTrigger = true;
            animator.SetTrigger("Pullback");
        }

        if (axeDistance < minAxeDistance && !hasAxe && Time.time >= curAxepullback)
        {
            CatchAxe();
            setPullbackTrigger = false;
        }

        if (Time.time >= curLargeJumpTime && !largeJumping && !attacking && hasAxe && baseHealth.currentHealth <= minLargeJumpHealth)
        {
            if (!largeJumpingPrep && distanceToTarget > distanceToAxe.y + 0.5f)
            {
                largeJumpingPrep = true;
                curJumpTime = Time.time + 4f;
                curLargeJumpTime = Time.time + 0.6f;
                return;
            }
            if (Time.time >= curLargeJumpTime && largeJumpingPrep)
                LargeJump();
        }

        if (distanceToTarget < distanceToAxe.x && distanceToTarget > distanceToAxe.y && baseHealth.currentHealth <= throwHealthMin && Time.time >= curAxeThrowTime && canThrow && hasAxe && !isJumping && !largeJumping && !largeJumpingPrep)
        {
            Throw();
            return;
        }

        if (Time.time >= curAttackTime)
            attacking = false;
        if(distanceToTarget <= distanceToStop && canAttack && !attacking && hasAxe && !isJumping && !jumping)
        {
            Attack();
        }

        if(largeJumping)
        {
            if(rigidbody.velocity.y < 0 && landed)
            {
                landed = false;
                curSpawnTime = Time.time + spawnTime;
                beganBeginning = false;
                transform.position = new Vector2(targetEntity.transform.position.x, newJumpHeight);
            }
        }

        if (grounded && hasAxe)
        {
            if(distanceToTarget < distanceToJump && !isJumping && hasAxe && (transform.position.y > targetEntity.transform.position.y) && !largeJumping)
            {
                if(!jumping)
                {
                    jumping = true;
                    curJumpTime = Time.time + 0.25f;
                }
                if(Time.time >= curJumpTime && jumping)
                    SmallJump();
            }
            if (distanceToTarget > distanceToJump)
                jumping = false;
            if (attacking || jumping || largeJumping || largeJumpingPrep)
                return;

            if (targetEntity.transform.position.x < transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
            if (distanceToTarget >= distanceToStop)
            {
                if (direction == 1)
                    MoveLeft();
                else if (direction == -1)
                    MoveRight();
            }
        }
    }

    public void LandingToggle(bool disable = false)
    {
        landingSprite.gameObject.SetActive(false);
        if (disable)
            return;

        landingActive = !landingActive;
        landingSprite.gameObject.SetActive(landingActive);
    }

    public void SmallJump()
    {
        jumping = false;
        Jump(smallJumpSpeed);
        if (transform.position.x < 0)
            MoveRight(true, 1, smallJumpSpeedX);
        else
            MoveLeft(true, -1, smallJumpSpeedX);
    }

    public void LargeJump()
    {
        largeJumpingPrep = false;
        jumping = false;
        Jump(largeJumpSpeed);
        largeJumping = true;
        setLandingXOffset = false;
        curLargeJumpTime = Time.time + largeJumpTime;
    }

    public void Throw()
    {
        if (hasAxe)
        {
            canThrow = false;
            canAttack = false;
            animator.SetTrigger("Throw");
        }
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        attacking = true;
        curAttackTime = Time.time + attackTime;
    }

    public void ThrowAxe()
    {
        if (hasAxe)
        {
            axeRigidbody.transform.parent = null;
            axeRigidbody.isKinematic = false;
            axeFollower.enabled = false;
            axeLogic.beingThrown = true;
            hasAxe = false;
            curAxepullback = Time.time + axePullback;
            axeRigidbody.AddForce(new Vector2((direction == -1) ? axeSpeed : -axeSpeed, axeSpeedVertical), ForceMode2D.Impulse);
        }
    }

    public void CatchAxe()
    {
        if (!hasAxe)
        {
            canAttack = true;
            canThrow = true;
            axeRigidbody.transform.parent = axeParent;
            axeRigidbody.isKinematic = true;
            axeFollower.enabled = true;
            axeLogic.beingThrown = false;
            curAxeThrowTime = axeThrowTime + Time.time;
            hasAxe = true;
            pullingback = false;
        }
    }

    public void PullAxeBack()
    {
        axeLogic.beingThrown = false;
        axeRigidbody.isKinematic = true;
        pullingback = true;
    }

    private bool jumping;
    private float curJumpTime;
    private float curLJumpTime;
    public void Animate()
    {
        animator.SetFloat("Velocity", rigidbody.velocity.magnitude);
        animator.SetBool("Jumping", jumping);
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Dead", grounded);
        animator.SetBool("_Began", _Began);
    }
}
