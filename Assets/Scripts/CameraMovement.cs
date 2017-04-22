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
    float orbitDistOffsetPlayer;
    //[SerializeField]
    //float rotationSmoothness;
    [SerializeField]
    float followSmoothness;
    [SerializeField]
    float followDistZ;
    [SerializeField]
    float followDistZMin;
    [SerializeField]
    float followDistzMax;

    float orbitDist; // Scales automatically with xScale component of pipe localScale Vec3.

    Transform cylinderTrans;

    // Private interface.
    void Start ()
    {
        cylinderTrans = GameObject.Find("Cylinder").gameObject.transform;
	}
	
	void Update ()
    {
        // Update orbit distance.
        orbitDist = playerObj.GetComponent<PlayerMovement>().GetOrbitDist() + orbitDistOffsetPlayer;

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
            followDistZ/*transform.position.z*/);

        // Update orientation.
        transform.rotation = Quaternion.Euler(
            transform.rotation.x, // Todo. Angle x-axis rot downward toward pipe's center or player's position.
            transform.rotation.y, 
            WrapValue(orbitAngle - 90, 360));
        // Attempts at updating orientation with x-axis offset 10 degrees toward pipe center.
        /*
        transform.rotation = Quaternion.Euler(
            transform.position.x - playerObj.transform.position.x, // Todo. Angle x-axis rot downward toward pipe's center or player's position.
            transform.position.y - playerObj.transform.position.y,
            WrapValue(orbitAngle - 90, 360));
        */
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
