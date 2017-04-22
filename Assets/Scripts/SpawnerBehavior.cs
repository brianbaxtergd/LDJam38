using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour 
{
	public void SpawnObstacle(GameObject obj)
	{
		Instantiate(obj, transform);
	}
}
