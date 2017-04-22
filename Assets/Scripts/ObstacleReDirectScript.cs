using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleReDirectScript : MonoBehaviour 
{
	[SerializeField]
	float NewSpeed = 10.0f;
	[SerializeField]
	Vector3 NewDirection = Vector3.zero;
	[SerializeField]
	bool RotateToDirection = false;

	void OnTriggerEnter(Collider col)
	{
		col.GetComponent<ObstacleBehavior>().Go(NewSpeed, NewDirection);
		if(RotateToDirection)
		{
			Vector3 oldForward = col.transform.forward;
			float angle = Vector3.Angle(oldForward, NewDirection);
			col.transform.Rotate(Vector3.up, -angle, Space.World);
		}
	}
}
