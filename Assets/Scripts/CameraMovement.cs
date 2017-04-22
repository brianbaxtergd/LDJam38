using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    GameObject playerObj;
    [SerializeField]
    float orbitAngle;
    [SerializeField]
    float rotationSmoothness;
    [SerializeField]
    float offsetAngleXAxis;

    float orbitDist;
    float speedDiv;

    Transform cylinderTrans;

    // Private interface.
    void Start ()
    {
        orbitDist = 4;
        speedDiv = 12;

        cylinderTrans = GameObject.Find("Cylinder").gameObject.transform;
	}
	
	void Update ()
    {
        // Update position.
        float angleDelta = Vector3.Angle(
            new Vector3(0, 0, orbitAngle),
            new Vector3(0, 0, playerObj.GetComponent<PlayerMovement>().GetOrbitAngle()));
        float orbitSpeed = angleDelta / speedDiv;

        if (Mathf.Abs(angleDelta) > 0.2)
            orbitAngle = WrapValue(orbitAngle + orbitSpeed, 360);
        else
            orbitAngle = playerObj.GetComponent<PlayerMovement>().GetOrbitAngle();

        Vector3 t = transform.position;
        t = new Vector3(
            cylinderTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            cylinderTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
            t.z);
        transform.position = t;

        // Update orientation.
        Quaternion rotCur = transform.rotation;
        Quaternion rotTar = Quaternion.LookRotation(
            new Vector3(0, 0, 1),
            new Vector3(0, playerObj.transform.position.y - cylinderTrans.position.y, 0));
        Quaternion rTar = Quaternion.Euler(rotTar.x, rotTar.y, rotTar.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, rTar, Time.deltaTime * rotationSmoothness);
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
}
