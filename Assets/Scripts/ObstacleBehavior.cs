using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour 
{
	Rigidbody RB;
	[SerializeField]
	float DestoryDistance = 6.0f;//Positive is behind player

	Vector3 originalPos;

	public void Go(float speed, Vector3 direction)
	{
		originalPos = transform.position;
		RB = GetComponent<Rigidbody>();
		Vector3 vel = direction * speed;
		RB.velocity = vel;
	}

	void Update()
	{
		Vector3 pos = transform.position - originalPos;
		if(Vector3.Dot(pos,pos) > DestoryDistance * DestoryDistance)
		{
			//Destory or reset
			Destroy(gameObject);
		}
	}
}
