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
    [SerializeField]
    float orbitDistSmoothness; // Used to divide delta between player orbitDist & camera orbitDist.
    [SerializeField]
    float followSmoothnessGround;
    [SerializeField]
    float followSmoothnessAir;
    [SerializeField]
    float followDistZ;
    [SerializeField]
    float followDistZMin;
    [SerializeField]
    float followDistZMax;

    float orbitDist; // Scales automatically with xScale component of pipe localScale Vec3.
    float orbitDistTar;
    float orbitDistMin;
    float orbitDistMax;

    Transform tubeTrans;
    Transform playerTrans;

    // Private interface.
    void Start ()
    {
        tubeTrans = GameObject.Find("Tube").gameObject.transform;
        playerTrans = GameObject.Find("Sphere").gameObject.transform;
	}
	
    void FixedUpdate()
    {

    }

	void Update ()
    {
        // Update orbit distance min & max, contingent on player values.
        orbitDistMin = playerObj.GetComponent<PlayerMovement>().GetOrbitDistMin() + orbitDistOffsetPlayer;
        orbitDistMax = playerObj.GetComponent<PlayerMovement>().GetOrbitDistMax() - orbitDistOffsetPlayer * 0.5f;

        // Update orbit distance.
        orbitDistTar = Mathf.Clamp(
            playerObj.GetComponent<PlayerMovement>().GetOrbitDist() + orbitDistOffsetPlayer, 
            orbitDistMin, 
            orbitDistMax);
        float delta = orbitDistTar - orbitDist;
        orbitDist = Mathf.Clamp(orbitDist + delta / orbitDistSmoothness, orbitDistMin, orbitDistMax);

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
            tubeTrans.position.x + lengthdir_x(orbitDist, orbitAngle),
            tubeTrans.position.y + lengthdir_y(orbitDist, orbitAngle),
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
