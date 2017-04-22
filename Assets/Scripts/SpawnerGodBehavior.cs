using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerGodBehavior : MonoBehaviour 
{
	[SerializeField]
	GameObject[] Obstacles = null;

	//This should be per spawner
	float spawnRate = 1.0f;
	float timer = 0.0f;

	SpawnerBehavior[] childSpawners;

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
				int obstacle = Random.Range(0, Obstacles.Length);
				child.SpawnObstacle(Obstacles[obstacle], Random.Range(7.0f,15.0f));
			}
			timer = 0.0f;
			spawnRate = Random.Range(0.75f, 3.0f);
		}
	}
}
