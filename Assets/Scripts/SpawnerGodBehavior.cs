using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerGodBehavior : MonoBehaviour 
{
	[SerializeField]
	GameObject[] Obstacles = null;
	[SerializeField]
	float ObstacleSpeed = 10.0f;
	[SerializeField]
	Vector3 Direction = Vector3.zero;
	[SerializeField]
	Vector2 SpawnRateRange = Vector2.zero;
	[SerializeField]
	bool MoreOverTime = false;
	[SerializeField]
	float MinSpawnRate = 2.5f;


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
			SpawnRates[i] = Random.Range(SpawnRateRange.x, SpawnRateRange.y);
		}
	}

	void Update ()
	{
		if(MoreOverTime && SpawnRateRange.y > MinSpawnRate)
		{
			SpawnRateRange.y -= Time.deltaTime * 0.01f;
		}
		for(int i = 0; i < Timers.Length; ++ i)
		{
			Timers[i] += Time.deltaTime;
			if(Timers[i] > SpawnRates[i])
			{
				int obstacle = Random.Range(0, Obstacles.Length);
				childSpawners[i].SpawnObstacle(Obstacles[obstacle], ObstacleSpeed, Direction);
				Timers[i] = 0.0f;
				SpawnRates[i] = Random.Range(SpawnRateRange.x, SpawnRateRange.y);
			}
		}
	}
}
