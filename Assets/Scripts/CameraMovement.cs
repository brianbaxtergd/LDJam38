using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    GameObject playerObj;
    [SerializeField]
    float orbitAngle;
    //[SerializeField]
    //float rotationSmoothness;
    [SerializeField]
    float offsetAngleXAxis;
    [SerializeField]
    float followSmoothness;
    [SerializeField]
    float orbitDist;
    [SerializeField]
    float followDistZ;
    [SerializeField]
    float followDistZMin;
    [SerializeField]
    float followDistzMax;

    Transform cylinderTrans;

    // Private interface.
    void Start ()
    {
        cylinderTrans = GameObject.Find("Cylinder").gameObject.transform;
	}
	
	void Update ()
    {
        // Update position.
        float angleDelta = Vector3.Angle(
            new Vector3(0, 0, orbitAngle),
            new Vector3(0, 0, playerObj.GetComponent<PlayerMovement>().GetOrbitAngle()));
        float orbitSpeed = angleDelta / followSmoothness;

        if (Mathf.Abs(angleDelta) > 0.2)
            orbitAngle = WrapValue(orbitAngle + orbitSpeed, 360);
        else
            orbitAngle = playerObj.GetComponent<PlayerMovement>().GetOrbitAngle();

        transform.position = new Vector3(
            cylinderTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            cylinderTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
            transform.position.z);

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
}
