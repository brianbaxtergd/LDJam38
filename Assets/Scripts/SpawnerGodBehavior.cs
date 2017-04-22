using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerGodBehavior : MonoBehaviour 
{
	[SerializeField]
	GameObject[] Obstacles;

	//This should be in spawnbehavior
	[SerializeField]
	float spawnRate = 1.0f;//Rate of Spawn Per Second

	SpawnerBehavior[] childSpawners;
	float timer = 0.0f;

	void Start()
	{
		childSpawners = GetComponentsInChildren<SpawnerBehavior>();
	}

	void Update () 
	{
		timer += Time.deltaTime;
		if(timer > spawnRate)
		{
			foreach(var child in childSpawners)
			{
				child.SpawnObstacle(Obstacles[0]);
			}
			timer = 0.0f;
		}
	}
}
