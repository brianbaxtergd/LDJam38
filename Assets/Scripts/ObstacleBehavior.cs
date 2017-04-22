using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour 
{
	Rigidbody RB;
	[SerializeField]
	float DestoryPoint = 6.0f;//Positive is behind player

	public void Go(float speed, Vector3 direction)
	{
		RB = GetComponent<Rigidbody>();
		Vector3 vel = direction * speed;
		RB.velocity = vel;
	}

	void Update()
	{
		if(transform.position.x > DestoryPoint)
		{
			//Destory or reset
			Destroy(gameObject);
		}
	}
}
