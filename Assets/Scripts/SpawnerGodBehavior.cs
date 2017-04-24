﻿using System.Collections;
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
	[SerializeField]
	Vector2 PatternMinMaxRate = Vector2.zero;
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

	const int NumPatters = 1;

	float[] SpawnRates = null;
	float[] Timers = null;
	float[] Angles = null;
	float RingTimer = 0;
	int NumOfSpawns = 0;
	int Spawn = 0;
	float PatternTimer = 0;

	bool doingPattern = false;

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
		if(doingPattern)
			return;
		if(MoreOverTime)
		{
			if(SpawnRateRange.y >= MinSpawnRate.y)
				SpawnRateRange.y -= Time.deltaTime * 0.01f;
			if(SpawnRateRange.x >= MinSpawnRate.x)
				SpawnRateRange.x -= Time.deltaTime * 0.0065f;
		}

		SpawnRing();
		SpawnObstacles();

		PatternTimer -= Time.deltaTime;
		if(PatternTimer <= 0)
		{
			int pat = Random.Range(1, NumPatters);
			switch(pat)
			{
			case 1://CorkScrew
				StartCoroutine(Corkscrew());
				break;
			default:
				break;
			}
			PatternTimer = Random.Range(PatternMinMaxRate.x, PatternMinMaxRate.y);
		}
	}

	void SpawnObstacles()
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

	IEnumerator Corkscrew()
	{
		doingPattern = true;
		int numberOfSpawns = Random.Range(NumOfSpawns, NumOfSpawns * 5);
		int direction = Random.Range(0,3);
		if(direction < 2)
			direction = -1;
		else
			direction = 1;
		int angle = Random.Range(0, Angles.Length - 1);
		for(int i = 0; i < numberOfSpawns; ++i)
		{
			transform.rotation = Quaternion.Euler(0, 0, Angles[angle]);
			InnerOuterSpawns[Spawn].SpawnObstacle(Obstacles[0], ObstacleSpeed, Direction);
			angle += direction;
			if(angle < 0)
				angle = Angles.Length - 1;
			else if(angle >= Angles.Length)
				angle = 0;
			yield return new WaitForSeconds(SpawnRateRange.x * 0.5f);
		}
		doingPattern = false;
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
