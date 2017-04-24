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
    float velYResetMult; // Multiplied to velYBoost to clamp negative velocity before boost/jump.
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
    bool inputJumpUp = false; // Press-only.
    bool inputJumpDown = false; // Press-only.
    bool inputBoost = false;
    bool inputExit = false;

    Transform playerTrans;
    Transform tubeTrans;

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

    public float GetOrbitDistMax()
    {
        return orbitDistMax;
    }

    public float GetOrbitDistMin()
    {
        return orbitDistMin;
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
        inputJumpUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetAxis("Mouse ScrollWheel") > 0);
        inputJumpDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) || (Input.GetAxis("Mouse ScrollWheel") < 0);

        // Check for boost input.
        inputBoost = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

        // Check for exit input.
        inputExit = Input.GetKey(KeyCode.Escape);
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

    void UpdateJumpBoost()
    {
        // Boosting & jumping.
        if (inputBoost || inputJumpUp || inputJumpDown)
        {
            if (inputJumpUp || inputJumpDown)
            {
                if (inputJumpUp && GetIsGrounded())
                    velY = Mathf.Clamp(velY + velYJump, velYMin, velYMax);
                else if (inputJumpDown && !GetIsGrounded())
                    velY = Mathf.Clamp(velY - velYJump, velYMin, velYMax);
            }
            else if (inputBoost)
            {
                if (velY < velYBoost * velYResetMult)
                    velY = velYBoost * velYResetMult;
                velY = Mathf.Clamp(velY + velYBoost, velYMin, velYMax);
            }
        }
        else
        {
            velY = Mathf.Clamp(velY - velYGravityHi, velYMin, velYMax);
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
    }
}
