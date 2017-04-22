using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour 
{
	public void SpawnObstacle(GameObject obj, float speed)
	{
		GameObject instobj = Instantiate(obj, transform);
		instobj.GetComponent<ObstacleBehavior>().Go(speed);
	}
}
