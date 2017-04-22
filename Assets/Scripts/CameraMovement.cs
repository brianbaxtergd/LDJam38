﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    GameObject playerObj;
    [SerializeField]
    float orbitAngle;
    [SerializeField]
    float orbitDistOffsetPlayer;
    [SerializeField]
    float followSmoothnessGround;
    [SerializeField]
    float followSmoothnessAir;
    [SerializeField]
    float followDistZ;
    [SerializeField]
    float followDistZMin;
    [SerializeField]
    float followDistzMax;

    float orbitDist; // Scales automatically with xScale component of pipe localScale Vec3.

    Transform cylinderTrans;
    Transform playerTrans;

    // Private interface.
    void Start ()
    {
        cylinderTrans = GameObject.Find("Cylinder").gameObject.transform;
        playerTrans = GameObject.Find("Sphere").gameObject.transform;
	}
	
    void FixedUpdate()
    {

    }

	void Update ()
    {
        // Update orbit distance.
        orbitDist = playerObj.GetComponent<PlayerMovement>().GetOrbitDist() + orbitDistOffsetPlayer;

        // Update position.
        float angleDelta = AngleDiff(playerObj.GetComponent<PlayerMovement>().GetOrbitAngle(), orbitAngle);
        float orbitSpeed;
        if (playerObj.GetComponent<PlayerMovement>().GetIsGrounded())
            orbitSpeed = angleDelta / followSmoothnessGround;
        else
            orbitSpeed = angleDelta / followSmoothnessAir;

        if (Mathf.Abs(angleDelta) > 1)
            orbitAngle = WrapValue(orbitAngle + orbitSpeed, 360);
        else
            orbitAngle = playerObj.GetComponent<PlayerMovement>().GetOrbitAngle();

        transform.position = new Vector3(
            cylinderTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            cylinderTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
            followDistZ/*transform.position.z*/);

        // Update orientation.
        transform.rotation = Quaternion.Euler(
            transform.rotation.x, // Todo. Angle x-axis rot downward toward pipe's center or player's position.
            transform.rotation.y, 
            WrapValue(orbitAngle - 90, 360));
    }

    // Public interface.
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

    float AngleDiff(float _tarAng, float _curAng)
    {
        return ((((_tarAng - _curAng) % 360) + 540) % 360) - 180;
    }
}