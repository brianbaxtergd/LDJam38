using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerGodBehavior : MonoBehaviour 
{
	[SerializeField]
	SpawnerBehavior[] InnerOuterSpawns = null;
	[SerializeField]
	float AngleDiff = 40.0f;
	[SerializeField]
	GameObject[] Obstacles = null;
	[SerializeField]
	GameObject[] Rings = null;
	[SerializeField]
	float RingSpawnRate = 6.0f;
	[SerializeField]
	GameObject[] PowerUps = null;
	[SerializeField]
	float PercentageOfPowerUp = 35.0f;
	[SerializeField]
	GameObject Hole = null;
	[SerializeField]
	float ObstacleSpeed = 10.0f;
	[SerializeField]
	Vector3 Direction = Vector3.zero;
	[SerializeField]
	Vector2 SpawnRateRange = Vector2.zero;//x is min y is max
	[SerializeField]
	bool MoreOverTime = false;//the longer time goes the more obstacles spawn
	[SerializeField]
	Vector2 MinSpawnRate = Vector2.zero;//the minimum spawn rate
	[SerializeField]
	int NumOfAlwaysOpenSpots = 2;

	float[] SpawnRates = null;
	float[] Timers = null;
	float[] Angles = null;
	float RingTimer = 0;
	int NumOfSpawns = 0;
	int Spawn = 0;

	void Start()
	{
		NumOfSpawns = (int)(360 / AngleDiff);
		SpawnRates = new float[NumOfSpawns];
		Timers = new float[NumOfSpawns];
		Angles = new float[NumOfSpawns];
		for(int i = 0; i < NumOfSpawns; ++i)
		{
			Timers[i] = 0.0f;
			SpawnRates[i] = Random.Range(SpawnRateRange.x, SpawnRateRange.y);
			Angles[i] = i * 40;
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
		SpawnRing();
		SpawnCheck();
	}

	void SpawnCheck()
	{
		int currNumOpen = 0;
		for(int i = 0; i < NumOfSpawns; ++ i)
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
				float isPowerup = Random.Range(0.0f, 100.0f);
				transform.rotation = Quaternion.Euler(0,0,Angles[i]);
				if(PowerUps.Length > 0 && isPowerup <= PercentageOfPowerUp)
				{
					int power = Random.Range(0, PowerUps.Length);
					InnerOuterSpawns[Spawn].SpawnObstacle(PowerUps[power], ObstacleSpeed, Direction);
				}
				else
					InnerOuterSpawns[Spawn].SpawnObstacle(Obstacles[obstacle], ObstacleSpeed, Direction);
				Timers[i] = 0.0f;
				SpawnRates[i] = Random.Range(SpawnRateRange.x, SpawnRateRange.y);
			}
		}
	}

	void SpawnRing()
	{
		if(Rings.Length > 0)
		{
			RingTimer += Time.deltaTime;
			if(RingTimer >= RingSpawnRate)
			{
				for(int i = 0; i < NumOfSpawns; ++i)
				{
					Timers[i] -= 0.45f;
				}
				transform.rotation = Quaternion.Euler(0,0,0);
				int ring = Random.Range(0, Rings.Length);
				GameObject NewRing = Instantiate(Rings[ring], transform);
				NewRing.transform.SetParent(null);
				NewRing.GetComponent<ObstacleBehavior>().Go(ObstacleSpeed, Direction);
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

	public void SetRingSpawnRate(float rate)
	{
		RingSpawnRate = rate;
	}

	//0 is inner 1 is outer
	public void SetIOState(int state)
	{
		Spawn = state;
	}
}
