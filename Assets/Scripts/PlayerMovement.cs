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

    float orbitSpeed;
    float orbitDist;

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
    }
	
	void Update ()
    {
        // Check for user input.
        inputLeft = Input.GetMouseButton(0);
        inputRight = Input.GetMouseButton(1);

        // React to input.
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

        // Update player position.
        Vector3 t = playerTrans.position;
        t = new Vector3(
            cylinderTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            cylinderTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
            t.z);
        playerTrans.position = t;
	}

    // Public interface.
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
