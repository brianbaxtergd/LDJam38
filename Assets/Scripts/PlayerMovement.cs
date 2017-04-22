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
    float velYGravityHi;
    [SerializeField]
    float velYGravityLo;

    float velY;

    float orbitSpeed;
    float orbitDist;
    float orbitDistMin;

    bool inputLeft;
    bool inputRight;

    Transform playerTrans;
    Transform cylinderTrans;

    // Private interface.
    void Start ()
    {
        inputLeft = false;
        inputRight = false;
        orbitSpeed = 0;

        playerTrans = GameObject.Find("Sphere").gameObject.transform;
        cylinderTrans = GameObject.Find("Cylinder").gameObject.transform;

        orbitDist = cylinderTrans.localScale.x * 0.5f + transform.localScale.x * 0.5f;
        orbitDistMin = orbitDist;

        velY = 0;
    }
	
    void FixedUpdate()
    { 
    }

	void Update ()
    {
        // Check for movement input.
        inputLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        inputRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

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

        // Check for jump input.
        bool inputJumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
        bool inputJumpHold = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

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
        velY = Mathf.Clamp(velY, velYMin, velYMax);
        orbitDist = Mathf.Clamp(orbitDist + velY, orbitDistMin, 1000);
        if (orbitDist == orbitDistMin)
        {
            // Kill velY on landing.
            if (velY != 0)
                velY = 0;
        }

        // Update player position.
        Vector3 t = playerTrans.position;
        t = new Vector3(
            cylinderTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            cylinderTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
            t.z);
        playerTrans.position = t;
	}

    // Public interface.
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
}
