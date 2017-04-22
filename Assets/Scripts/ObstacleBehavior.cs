using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour 
{
	Rigidbody RB;
	[SerializeField]
	float DestoryDistance = 6.0f;

	Vector3 originalPos;

	public void Go(float speed, Vector3 direction)
	{
		originalPos = transform.position;
		if(RB == null)
			RB = GetComponent<Rigidbody>();
		RB.velocity = Vector3.zero;
		Vector3 vel = direction * speed;
		RB.velocity = vel;
	}

	void Update()
	{
		/*Vector3 pos = transform.position - originalPos;
		if(Vector3.Dot(pos,pos) > DestoryDistance * DestoryDistance)
		{
			//Destory or reset
			Destroy(gameObject);
		}*/
	}
}
