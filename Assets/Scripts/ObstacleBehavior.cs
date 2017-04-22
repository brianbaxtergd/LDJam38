using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour 
{
	Rigidbody RB;
	[SerializeField, Range(1,50)]
	float Speed = 10.0f;
	float DestoryPoint = 6.0f;

	//Or when spawned
	void Start()
	{
		RB = GetComponent<Rigidbody>();//Only needed once so keep in start
		Vector3 vel = Vector3.zero;
		vel.x = Speed;
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
