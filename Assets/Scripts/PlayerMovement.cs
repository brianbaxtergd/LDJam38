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

    bool inputLeft;
    bool inputRight;

    float distSolid;

    UnityEngine.Transform playerTrans;
    UnityEngine.Transform cylinderTrans;
    float cylinderRadius;

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

	// Use this for initialization
	void Start ()
    {
        inputLeft = false;
        inputRight = false;
        distSolid = 3;
        orbitSpeed = 0;

        playerTrans = GameObject.Find("Sphere").gameObject.transform;

        cylinderTrans = GameObject.Find("Cylinder").gameObject.transform;
        cylinderRadius = cylinderTrans.localScale.x;
    }
	
	// Update is called once per frame
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
        /*else if (orbitSpeed < 0)
        {
            orbitSpeed = Mathf.Min(orbitSpeed + speedDecel, 0);
        }*/

        if (inputRight)
        {
            orbitSpeed -= speedAccel;
        }
        /*else if (orbitSpeed > 0)
        {
            orbitSpeed = Mathf.Max(orbitSpeed - speedDecel, 0);
        }*/

        // Clamp speed.
        orbitSpeed = Mathf.Clamp(orbitSpeed, 0.0f - speedMax, speedMax);

        // Update orbit angle.
        orbitAngle = WrapValue(orbitAngle + orbitSpeed, 360);

        // Update player position.
        Vector3 t = playerTrans.position;
        t = new Vector3(
            cylinderTrans.position.x + lengthdir_x(distSolid, orbitAngle),
            cylinderTrans.position.y + lengthdir_y(distSolid, orbitAngle),
            t.z);
        playerTrans.position = t;
	}
}
