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
	//x is min y is max
	Vector2 SpawnRateRange = Vector2.zero;
	[SerializeField]
	bool MoreOverTime = false;//the longer time goes the more obstacles spawn
	[SerializeField]
	Vector2 MinSpawnRate = Vector2.zero;//the minimum spawn rate
	[SerializeField]
	int NumOfAlwaysOpenSpots = 2;


	float[] SpawnRates = null;
	float[] Timers = null;

	SpawnerBehavior[] childSpawners = null;


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
		if(MoreOverTime)
		{
			if(SpawnRateRange.y >= MinSpawnRate.y)
				SpawnRateRange.y -= Time.deltaTime * 0.01f;
			if(SpawnRateRange.x >= MinSpawnRate.x)
				SpawnRateRange.x -= Time.deltaTime * 0.0065f;
		}
		int currNumOpen = 0;
		for(int i = 0; i < Timers.Length; ++ i)
		{
			Timers[i] += Time.deltaTime;
			if(Timers[i] > SpawnRates[i])
			{
				if(currNumOpen < NumOfAlwaysOpenSpots)
				{
					Timers[(i + 1) % Timers.Length] -= SpawnRateRange.x * 0.75f;
					++currNumOpen;
				}
				int obstacle = Random.Range(0, Obstacles.Length);
				childSpawners[i].SpawnObstacle(Obstacles[obstacle], ObstacleSpeed, Direction);
				Timers[i] = 0.0f;
				SpawnRates[i] = Random.Range(SpawnRateRange.x, SpawnRateRange.y);
			}
		}
	}

	public void SetOpenSpots(int spots)
	{
		NumOfAlwaysOpenSpots = spots;
	}
	public void SetObstacleSpeed(float speed)
	{
		ObstacleSpeed = speed;
	}
}
