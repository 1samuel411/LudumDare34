using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
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

    // Moving
    public float maxSpeed = 2;
    public float speed;
    private float _regSpeed;
    public float jumpSpeed;
    public float jumpHeight;

    // Jumping
    public float jumpTime;

    // Variables
    public bool canMove = true;
    public bool canJump = true;
    public bool canScale = true;
    public bool isJumping = false;

    // Grounded
    public LayerMask groundedMask;
    public bool grounded;
    public float groundCheckDist;
    private RaycastHit2D _groundHit;

    // 1 is left | -1 is right
    private int _direction = 1;

    private new Transform transform;
    private new Rigidbody2D rigidbody;

    void Start ()
    {
        _regSpeed = speed;
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        // Update our scale based on direction
        if (canScale)
        {
            transform.localScale = new Vector3(_direction, 1, 1);
        }

        // Restrict speed
        if(rigidbody.velocity.magnitude > maxSpeed)
        {
            speed = 0;
        }
        else
        {
            speed = _regSpeed;
        }

        // Check grounded
        grounded = Grounded();

        // Stop jumping when we hit the ground if we are jumping
        if(grounded && isJumping)
        {
            FinishJump();
        }

        if (canMove)
        {
            // Check left key
            if (Input.GetKey(leftKey))
            {
                // If this is the first time holding it then set the hold time
                if (!_holdingLeftKey)
                {
                    _curHoldTimeLeft = Time.time;
                    _targetHoldTimeLeft = Time.time + holdTimeMove;
                }

                _holdingLeftKey = true;

                // Need to hold these keys for a certain time to allow left movement
                if (_curHoldTimeLeft > _targetHoldTimeLeft && !_holdingKeys)
                {
                    // Move left
                    MoveLeft();
                }
                else
                {
                    _curHoldTimeLeft += Time.deltaTime;
                }
            }
            else
            {
                _holdingLeftKey = false;
            }

            // Check right key
            if (Input.GetKey(rightKey))
            {
                // If this is the first time holding it then set the hold time
                if (!_holdingRightKey)
                {
                    _curHoldTimeRight = Time.time;
                    _targetHoldTimeRight = Time.time + holdTimeMove;
                }

                _holdingRightKey = true;

                // Need to hold these keys for a certain time to allow right movement
                if (_curHoldTimeRight > _targetHoldTimeRight && !_holdingKeys)
                {
                    // Move right
                    MoveRight();
                }
                else
                {
                    _curHoldTimeRight += Time.deltaTime;
                }
            }
            else
            {
                _holdingRightKey = false;
            }
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
                }
                else
                {
                    _curHoldTimeJump += Time.deltaTime;
                }

            }
            else
            {
                _holdingKeys = false;
            }
        }
	}

    public bool Grounded()
    {
        _groundHit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, groundedMask);

        if(_groundHit)
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
        _direction = 1;
        rigidbody.AddForce(new Vector2(-speed, 0) * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void MoveRight()
    {
        _direction = -1;
        rigidbody.AddForce(new Vector2(speed, 0) * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void Jump()
    {
        // Disable movement
        _holdingKeys = false;
        canJump = false;
        canMove = false;
        canScale = false;
        isJumping = true;

        // Give air boost
        rigidbody.AddForce(new Vector2(0, jumpHeight * jumpSpeed) * Time.deltaTime, ForceMode2D.Impulse);
    }

    void FinishJump()
    {
        canJump = true;
        canMove = true;
        canScale = true;
        isJumping = false;
    }
}
