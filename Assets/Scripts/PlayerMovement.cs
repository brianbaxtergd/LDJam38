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

    public enum groundStates
    {
        OUTER = 0,  // Riding along the outer-edge of the conductor-pipe.
        INNER       // Riding along the inner-edge of the conductor-pipe.
    }
    groundStates groundState = groundStates.OUTER;
    bool isChangingGroundStates = false;

    float pipeRatio_full = 1.0f;  // Origin to outer-edge.
    float pipeRatio_space = 0.84f; // Origin to inner-edge.
    float pipeRatio_edge = 0.16f; // Inner-edge to outer-edge.

    // Unity interface.
    void Start ()
    {
        playerTrans = GameObject.Find("Sphere").gameObject.transform;
        tubeTrans = GameObject.Find("Tube").gameObject.transform;

        /*orbitDist = tubeTrans.localScale.x * 0.05f * 0.5f + transform.localScale.x * 0.5f;
        orbitDistMin = orbitDist;
        orbitDistMax = orbitDistMin * 3;*/
        SetYBounds(groundState);
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
        // Check for and react to collisions.
        CheckCollisions();
        // Update translation.
        UpdateTranslation();
	}

    // Unique interface.
    public bool GetIsGrounded()
    {
        if (isChangingGroundStates)
            return false;

        float tarDist;
        if (groundState == groundStates.OUTER)
            tarDist = orbitDistMin;
        else
            tarDist = orbitDistMax;

        if (Mathf.Abs(orbitDist - tarDist) <= 0.05)
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

        if (isChangingGroundStates)
        {
            // Current state represents the state we're transitioning INTO, not out of.
            switch(groundState)
            {
                case groundStates.OUTER: // Transition to outer-edge of pipe.
                    orbitDist = Mathf.Min(orbitDist + velY, orbitDistMax); // Clamping the relative "ceiling" bound.
                    if (orbitDist >= orbitDistMin)
                        isChangingGroundStates = false;
                    break;
                case groundStates.INNER: // Transition to inner-edge of pipe.
                    orbitDist = Mathf.Max(orbitDist + velY, orbitDistMin); // Clamping the relative "ceiling" bound.
                    if (orbitDist <= orbitDistMax)
                        isChangingGroundStates = false;
                    break;
                default:
                    break;
            }
        }
        else
        {
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
    }

    void CheckCollisions()
    {
        // Portals.
        GameObject portal = GameObject.Find("Portal");
        if (portal != null)
        {
            bool canCollide = (Mathf.Abs(portal.transform.position.z - transform.position.z) <= 0.5); // Is the portal close enough to the player?
            if (canCollide
                && inputJumpDown) // <-- should this be speed based? perhaps just collision based w/o need for user input?
            {
                groundStates newState;
                if (groundState == groundStates.OUTER)
                    newState = groundStates.INNER;
                else
                    newState = groundStates.OUTER;
                SetGroundState(newState);
            }
        }

        // Obstacles.
        /*
        */

        // Power ups.
        /*
        */
    }

    void UpdateTranslation()
    {
        playerTrans.position = new Vector3(
            tubeTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            tubeTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
            playerTrans.position.z);
    }

    void SetGroundState(groundStates _state)
    {
        // Update y-axis bounds.
        SetYBounds(_state);

        // Flip y-axis related constants.
        velYResetMult = -velYResetMult;
        velYJump = -velYJump;
        velYBoost = -velYBoost;
        velYGravityHi = -velYGravityHi;
        velYGravityLo = -velYGravityLo;

        // Accelerate player toward old ground, away from new ground.
        velY = Mathf.Clamp(velY + velYJump * 0.5f, velYMin, velYMax);

        // Update state.
        groundState = _state;
        isChangingGroundStates = true; // Set to false after transition, within velocity clamp-logic function(s).
    }

    public groundStates GetGroundState()
    {
        return groundState;
    }

    void SetYBounds(groundStates _state)
    {
        // Update min & max height for velY (boosting / jumping / falling / riding).
        switch (_state)
        {
            case groundStates.OUTER:
                orbitDistMin = tubeTrans.localScale.x * 0.05f * 0.5f + transform.localScale.x * 0.5f;
                orbitDistMax = orbitDistMin * 3;
                break;
            case groundStates.INNER:
                orbitDistMin = 0.25f;
                orbitDistMax = tubeTrans.localScale.x * 0.05f * (0.5f * pipeRatio_space) - transform.localScale.x * 0.5f; // Falling thru rel-ceil inner groundState, changed tubeTrans to transform.
                break;
            default:
                break;
        }
    }
}
