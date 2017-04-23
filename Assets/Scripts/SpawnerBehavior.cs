using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour 
{
	public void SpawnObstacle(GameObject obj)
	{
        float newZ = (float)Random.Range(0, 359);
		//Instantiate(obj, transform);
        Instantiate(
            obj, 
            transform.position, 
            Quaternion.Euler(0, 0, newZ), 
            gameObject.transform);
	}
}
