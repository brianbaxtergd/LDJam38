using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerGodBehavior : MonoBehaviour 
{
	[SerializeField]
	GameObject[] Obstacles = null;
	[SerializeField]
	float ObstacleSpeed = 10.0f;


	float[] SpawnRates = null;
	float[] Timers = null;

	SpawnerBehavior[] childSpawners;

	void Start()
	{
		childSpawners = GetComponentsInChildren<SpawnerBehavior>();
		SpawnRates = new float[childSpawners.Length];
		Timers = new float[childSpawners.Length];
		for(int i = 0; i < childSpawners.Length; ++i)
		{
			Timers[i] = 0.0f;
			SpawnRates[i] = Random.Range(0.5f, 3.0f);
		}
	}

	void Update ()
	{
		for(int i = 0; i < Timers.Length; ++ i)
		{
			Timers[i] += Time.deltaTime;
			if(Timers[i] > SpawnRates[i])
			{
				int obstacle = Random.Range(0, Obstacles.Length);
				childSpawners[i].SpawnObstacle(Obstacles[obstacle], ObstacleSpeed);
				Timers[i] = 0.0f;
				SpawnRates[i] = Random.Range(0.75f, 3.0f);
			}
		}
	}
}
