using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float speedMax;
    [SerializeField]
    float speedAccel;
    [SerializeField]
    float speedDecel;
    [SerializeField]
    float orbitAngle;
    [SerializeField]
    float velYMax;
    [SerializeField]
    float velYMin;
    [SerializeField]
    float velYJump;
    [SerializeField]
    float velYBoost;
    [SerializeField]
    float velYGravityHi;
    [SerializeField]
    float velYGravityLo;

    float velY = 0;

    float orbitSpeed = 0;
    float orbitDist;
    float orbitDistMin;
    float orbitDistMax;

    bool inputLeft = false;
    bool inputRight = false;
    bool inputJumpHold = false;
    bool inputJumpPressed = false;
    bool inputBoost = false;

    bool canJump = true;
    float jumpCooldownTimer = 0;
    [SerializeField]
    float jumpCooldownTimerMax;

    Transform playerTrans;
    Transform tubeTrans;

    public enum jumpTypes
    {
        SINGLE = 0,
        BOOST,
    }
    jumpTypes jumpType = jumpTypes.BOOST;

    // Unity interface.
    void Start ()
    {
        playerTrans = GameObject.Find("Sphere").gameObject.transform;
        tubeTrans = GameObject.Find("Tube").gameObject.transform;

        orbitDist = tubeTrans.localScale.x * 0.05f * 0.5f + transform.localScale.x * 0.5f;
        orbitDistMin = orbitDist;
        orbitDistMax = orbitDistMin * 3;
    }
	
    void FixedUpdate()
    { 
    }

	void Update ()
    {
        // Update user input.
        UpdateInput();
        // React to input.
        UpdateMovement();
        UpdateJumpBoost();
        // Update translation.
        UpdateTranslation();
	}

    // Unique interface.
    public bool GetIsGrounded()
    {
        if (Mathf.Abs(orbitDist - orbitDistMin) <= 0.05)
            return true;
        else
            return false;
    }

    public float GetOrbitDist()
    {
        return orbitDist;
    }

    public float GetOrbitAngle()
    {
        return orbitAngle;
    }

    float WrapValue(float _val, float _wrapAmt)
    {
        if (_val <= 0)
            _val += _wrapAmt;
        else if (_val >= 360)
            _val -= _wrapAmt;

        return _val;
    }

    float lengthdir_x(float len, float dir)
    {
        return len * Mathf.Cos(dir * Mathf.Deg2Rad);
    }

    float lengthdir_y(float len, float dir)
    {
        return len * Mathf.Sin(dir * Mathf.Deg2Rad);
    }

    void UpdateInput()
    {
        // Check for movement input.
        inputLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        inputRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        // Check for jump input.
        inputJumpPressed = Input.GetMouseButtonDown(1);
        inputJumpHold = Input.GetMouseButton(1);

        // Check for boost input.
        inputBoost = Input.GetMouseButton(0);
    }

    void UpdateMovement()
    {
        // React to movement input.
        if (inputLeft)
        {
            orbitSpeed += speedAccel;
        }
        if (inputRight)
        {
            orbitSpeed -= speedAccel;
        }

        // Apply friction when not accelerating.
        if (!inputRight && !inputLeft)
        {
            if (orbitSpeed < 0)
                orbitSpeed = Mathf.Min(orbitSpeed + speedDecel, 0);
            else if (orbitSpeed > 0)
                orbitSpeed = Mathf.Max(0, orbitSpeed - speedDecel);
        }

        // Clamp speed.
        orbitSpeed = Mathf.Clamp(orbitSpeed, 0.0f - speedMax, speedMax);

        // Update orbit angle.
        orbitAngle = WrapValue(orbitAngle + orbitSpeed, 360);
    }

    void UpdateJumpSingle()
    {
        // React to jump input.
        if (GetIsGrounded())
        {
            if (inputJumpPressed)
            {
                // Add vertical velocity.
                velY += velYJump;
            }
        }
        else
        {
            // Apply gravity if player is floating.
            if (inputJumpHold)
                velY -= velYGravityLo;
            else
                velY -= velYGravityHi;
        }
        // Clamp velocity between min & max.
        velY = Mathf.Clamp(velY, velYMin, velYMax);
    }

    void UpdateJumpBoost()
    {
        // Jumping cooldown.
        if (!canJump)
        {
            jumpCooldownTimer = Mathf.Max(jumpCooldownTimer - Time.deltaTime, 0);
            if (jumpCooldownTimer == 0)
                canJump = true;
        }

        // Boosting & jumping.
        if (inputBoost || inputJumpPressed)
        {
            if (canJump && inputJumpPressed)
            {
                canJump = false;
                jumpCooldownTimer = jumpCooldownTimerMax;

                if (GetIsGrounded())
                    velY = Mathf.Clamp(velY + velYJump, velYMin, velYMax);
                else
                    velY = Mathf.Clamp(velY - velYJump, velYMin, velYMax);
            }
            else if (inputBoost)
            {
                velY = Mathf.Clamp(velY + velYBoost, velYMin, velYMax);
            }
        }
        else
        {
            velY = Mathf.Clamp(velY - velYGravityLo, velYMin, velYMax);
        }

        // Clamp velocity & apply to orbit distance (relative height).
        orbitDist = Mathf.Clamp(orbitDist + velY, orbitDistMin, orbitDistMax);

        // Check if player has landed.
        if (orbitDist == orbitDistMin || orbitDist == orbitDistMax)
        {
            // Kill velY on landing.
            if (velY != 0)
                velY = 0;
        }

    }

    void UpdateTranslation()
    {
        playerTrans.position = new Vector3(
            tubeTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            tubeTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
            playerTrans.position.z);
        /*Vector3 t = playerTrans.position;
        t = new Vector3(
            tubeTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            tubeTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
            t.z);
        playerTrans.position = t;*/
    }
}
