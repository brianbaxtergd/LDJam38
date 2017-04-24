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
	GameObject[] EightFence = null;
	[SerializeField]
	GameObject[] QuarterFence = null;
	[SerializeField]
	GameObject[] HalfFence = null;
	[SerializeField]
	GameObject[] ComboFences = null;
	[SerializeField]
	float ComboFenceSpawnRate = 10;
	[SerializeField]
	GameObject FullFence = null;
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
	GameObject[] PowerUps = null;
	[SerializeField]
	float PercentageOfPowerUp = 10.0f;
	[SerializeField]
	GameObject Hole = null;

	const int NumPatters = 1;

	float[] SpawnRates = null;
	float[] Timers = null;
	float[] Angles = null;
	float ComboFenceTimer = 0;
	int NumOfSpawns = 0;
	int Spawn = 0;
	float PatternTimer = 0;

	bool doingPattern = false;
	bool doSpawning = true;

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
		if(!doSpawning)
			return;
		if(doingPattern)
			return;
		if(MoreOverTime)
		{
			if(SpawnRateRange.y >= MinSpawnRate.y)
				SpawnRateRange.y -= Time.deltaTime * 0.01f;
			if(SpawnRateRange.x >= MinSpawnRate.x)
				SpawnRateRange.x -= Time.deltaTime * 0.0065f;
		}

		//SpawnRing();
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
				float isPowerup = Random.Range(0.0f, 100.0f);
				transform.rotation = Quaternion.Euler(0,0,Angles[i]);
				if(PowerUps.Length > 0 && isPowerup <= PercentageOfPowerUp)
				{
					int power = Random.Range(0, PowerUps.Length);
					InnerOuterSpawns[Spawn].SpawnObstacle(PowerUps[power], ObstacleSpeed, Direction);
				} 
				else
				{
					int fenceType = Random.Range(0, 3);
					GameObject fence = null;
					switch(fenceType)
					{
					case 0:
						int f = Random.Range(0, EightFence.Length - 1);
						fence = EightFence[f];
						break;
					case 1:
						int fq = Random.Range(0, QuarterFence.Length - 1);
						fence = QuarterFence[fq];
						break;
					case 2:
						int fh = Random.Range(0, HalfFence.Length - 1);
						fence = HalfFence[fh];
						break;
					case 3:
						int fc = Random.Range(0, ComboFences.Length - 1);
						fence = ComboFences[fc];
						break;
					default:
						break;
					}
					InnerOuterSpawns[Spawn].SpawnObstacle(fence, ObstacleSpeed, Direction);
				}
				Timers[i] = 0.0f;
				SpawnRates[i] = Random.Range(SpawnRateRange.x, SpawnRateRange.y);
			}
		}
	}

	void SpawnComboFence()
	{
		ComboFenceTimer += Time.deltaTime;
		if(ComboFenceTimer >= ComboFenceSpawnRate)
		{
			for(int i = 0; i < NumOfSpawns; ++i)
			{
				Timers[i] -= SpawnRateRange.x * 0.5f;
			}
			transform.rotation = Quaternion.Euler(0,0,0);
			int fenceType = Random.Range(0, ComboFences.Length - 1);
			GameObject NewRing = Instantiate(ComboFences[fenceType], transform);
			NewRing.transform.SetParent(null);
			NewRing.GetComponent<ObstacleBehavior>().Go(ObstacleSpeed, Direction);
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
		GameObject fence = null;
		int fc = Random.Range(0, ComboFences.Length - 1);
		fence = ComboFences[fc];
		for(int i = 0; i < numberOfSpawns; ++i)
		{
			transform.rotation = Quaternion.Euler(0, 0, Angles[angle]);
			InnerOuterSpawns[Spawn].SpawnObstacle(fence, ObstacleSpeed, Direction);
			angle += direction;
			if(angle < 0)
				angle = Angles.Length - 1;
			else if(angle >= Angles.Length)
				angle = 0;
			yield return new WaitForSeconds(SpawnRateRange.x * 0.5f);
		}
		doingPattern = false;
	}

	IEnumerator PauseSpawning(float time)
	{
		doSpawning = false;
		yield return new WaitForSeconds(time);
		doSpawning = true;
	}

	public void SetOpenSpots(int spots)
	{
		NumOfAlwaysOpenSpots = Mathf.Max(spots,1);
	}

	public void SetObstacleSpeed(float speed)
	{
		ObstacleSpeed = speed;
	}

	public void SetPowerUpPercentage(float ratio)
	{
		PercentageOfPowerUp = ratio;
	}

	public void SetObstacleSpawnRate(Vector2 minMax)
	{
		SpawnRateRange = minMax;
	}

	public void SetComboSpawnRate(float rate)
	{
		ComboFenceSpawnRate = rate;
  	}
	//0 is inner 1 is outer
	public void SetIOState(int state)
	{
		Spawn = state;
	}

	public void StartSpawning()
	{
		doSpawning = true;	
	}

	public void ForceSpawnHole(float angleInDeg)
	{
		transform.rotation = Quaternion.Euler(0, 0, angleInDeg);
		if(Hole)
		{
			GameObject NewHole = Instantiate(Hole, transform);
			NewHole.transform.SetParent(null);
			NewHole.GetComponent<ObstacleBehavior>().Go(ObstacleSpeed, Direction);
		}
	}

	public void ForceSpawnFullRing()
	{
		GameObject NewRing = Instantiate(FullFence, transform);
		NewRing.transform.SetParent(null);
		NewRing.GetComponent<ObstacleBehavior>().Go(ObstacleSpeed, Direction);
	}

	//For Stoping use -1
	public void Pause(float time)
	{
		if(time == -1)
		{
			doSpawning = false;
			return;
		}
		StartCoroutine(PauseSpawning(time));
	}
}