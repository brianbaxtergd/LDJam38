using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum gameStates
    {
        MAIN_MENU = 0,
        BREAK,   // Short calm period before/between obstacle spawning stages.
        LEVEL,          // Obstacle spawning level following break between levels.
        DEATH,
        PAUSE
    }
    gameStates state = gameStates.MAIN_MENU;

    [SerializeField]
    int level;
    [SerializeField]
    int levelMax;
    [SerializeField]
    float obstacleSpeedMin;
    [SerializeField]
    float obstacleSpeedMax;
    float obstacleSpeed;
    [SerializeField]
    float barCountBreak;
    [SerializeField]
    float barCountLevel;

	[SerializeField]
	SpawnerGodBehavior SpawnerGod = null;

    float breakTimerMax;
    float breakTimer = 0;
    float levelTimerMax;
    float levelTimer = 0;

    float highScoreLevel;

    bool inputMenuPlay = false;
    bool inputMenuExit = false;
    bool inputDeathRestart = false;
    bool inputDeathMainMenu = false;

    bool newRecord = false; // Whether or not player has reached a new highscore at death.

    Text textGameState;
    Text textLevel;
    Text textObstacleSpeed;
    Text textBreakTimer;
    Text textLevelTimer;

    AudioSource audSrcMenuMusic;
    AudioSource audSrcBreakMusic;
    AudioSource[] audSrcLevelMusic;

    // Unity interface.
	void Start ()
    {
        obstacleSpeed       = obstacleSpeedMin;
        highScoreLevel      = 0;

        textGameState       = GameObject.Find("GameStateText").GetComponent<Text>();
        textLevel           = GameObject.Find("LevelText").GetComponent<Text>();
        textObstacleSpeed   = GameObject.Find("ObstacleSpeedText").GetComponent<Text>();
        textBreakTimer      = GameObject.Find("BreakTimerText").GetComponent<Text>();
        textLevelTimer      = GameObject.Find("LevelTimerText").GetComponent<Text>();

        audSrcMenuMusic = GameObject.Find("MenuMusicAudio").GetComponent<AudioSource>();
        audSrcBreakMusic = GameObject.Find("BreakMusicAudio").GetComponent<AudioSource>();
        audSrcLevelMusic = new AudioSource[levelMax];
        audSrcLevelMusic[0] = GameObject.Find("LevelMusicLayer00Audio").GetComponent<AudioSource>();
        audSrcLevelMusic[1] = GameObject.Find("LevelMusicLayer01Audio").GetComponent<AudioSource>();
        audSrcLevelMusic[2] = GameObject.Find("LevelMusicLayer02Audio").GetComponent<AudioSource>();
        audSrcLevelMusic[3] = GameObject.Find("LevelMusicLayer03Audio").GetComponent<AudioSource>();
        audSrcLevelMusic[4] = GameObject.Find("LevelMusicLayer04Audio").GetComponent<AudioSource>();
        audSrcLevelMusic[5] = GameObject.Find("LevelMusicLayer05Audio").GetComponent<AudioSource>();
        audSrcLevelMusic[6] = GameObject.Find("LevelMusicLayer06Audio").GetComponent<AudioSource>();
        audSrcLevelMusic[7] = GameObject.Find("LevelMusicLayer07Audio").GetComponent<AudioSource>();

        breakTimerMax = audSrcBreakMusic.clip.length * barCountBreak;
        levelTimerMax = audSrcLevelMusic[0].clip.length * barCountLevel;

        SetState(state);
    }

    void FixedUpdate()
    {
        // State-machine for gameplay.
        switch (state)
        {
            case gameStates.MAIN_MENU:
                // Switch to break-state on user input.
                if (inputMenuPlay)
                    SetState(gameStates.BREAK);
                break;
            case gameStates.BREAK:
                // Timer tick toward level-state.
                breakTimer = Mathf.Max(breakTimer - Time.deltaTime, 0);
                if (breakTimer == 0)
                    SetState(gameStates.LEVEL);
                break;
            case gameStates.LEVEL:
                // Timer tick toward break-state.
                levelTimer = Mathf.Max(levelTimer - Time.deltaTime, 0);
                if (levelTimer == 0)
                    SetState(gameStates.BREAK);
                break;
            case gameStates.DEATH:
                if (inputDeathRestart)
                    SetState(gameStates.BREAK);
                else if (inputDeathMainMenu)
                    SetState(gameStates.MAIN_MENU);
                break;
            case gameStates.PAUSE:
                break;
        }
    }

	void Update ()
    {
        // Check for user input.
        UpdateInput();

        // Update UI.
        UpdateUI();
    }

    // Public interface.
    public float GetObstacleSpeed()
    {
        if (state == gameStates.DEATH || state == gameStates.PAUSE)
            return 0.0f;
        else
            return obstacleSpeed;
    }

    public float GetObstacleSpeedMin()
    {
        return obstacleSpeedMin;
    }

    public float GetObstacleSpeedMax()
    {
        return obstacleSpeedMax;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetLevelMax()
    {
        return levelMax;
    }

    public float GetBreakTimer()
    {
        return breakTimer;
    }

    public float GetBreakTimerMax()
    {
        return breakTimerMax;
    }

    public float GetLevelTimer()
    {
        return levelTimer;
    }

    public float GetLevelTimerMax()
    {
        return levelTimerMax;
    }

    public void SetState(gameStates _state)
    {
        // Reset misc data.
        if (newRecord)
            newRecord = false;

        // Stop all music.
        KillAllMusic();

        switch (_state)
        {
            case gameStates.MAIN_MENU:
                // Play music.
                audSrcMenuMusic.Play();
                break;
		case gameStates.BREAK:
                // Play music.
                audSrcBreakMusic.Play();
                // Increase level number.
				if(state != gameStates.DEATH)
					level += 1;
                // Increase obstacle speed.
				float ratio = (float)level / (float)levelMax;
				float newSp = obstacleSpeedMin + (ratio * (obstacleSpeedMax - obstacleSpeedMin));
				obstacleSpeed = Mathf.Clamp(newSp, obstacleSpeedMin, obstacleSpeedMax);
				SpawnerGod.SetObstacleSpeed(newSp);
				SpawnerGod.SetObstacleSpawnRate(new Vector2(1.2f - ratio, 3.0f - ratio));
				SpawnerGod.SetOpenSpots((int)(1 - ratio) * 4);
				SpawnerGod.SetPowerUpPercentage(1.1f - ratio * 0.10f);
            	    // Reset break & level timers.
				breakTimer = breakTimerMax;
				levelTimer = levelTimerMax;
				SpawnerGod.Pause(breakTimer);
                break;
            case gameStates.LEVEL:
                // Play music layers contingent on level.
                for (int i = 0; i < Mathf.Clamp(level, 0, audSrcLevelMusic.Length); i++)
                    audSrcLevelMusic[i].Play();
                break;
            case gameStates.DEATH:
                // Compare score with highscore, updating when greater.
                if (level > highScoreLevel)
                {
                    newRecord = true;
                    highScoreLevel = level;
                }
                else
                    newRecord = false;
                break;
            case gameStates.PAUSE:
                break;
        }
        // Update state data.
        state = _state;
    }
    // Private interface.
    void UpdateUI()
    {
        textGameState.text = "GameState: " + state.ToString();
        textLevel.text = "Level: " + level.ToString();
        textObstacleSpeed.text = "ObstacleSpeed: " + obstacleSpeed.ToString();
        float bT = (1.0f - breakTimer / breakTimerMax) * 100.0f;
        int bT_int = (int)bT;
        textBreakTimer.text = "BreakTimer: " + bT_int.ToString() + " %";
        float lT = (1.0f - levelTimer / levelTimerMax) * 100.0f;
        int lT_int = (int)lT;
        textLevelTimer.text = "LevelTimer: " + lT_int.ToString() + " %";
    }

    void UpdateInput()
    {
        // Main Menu.
        if (state == gameStates.MAIN_MENU)
        {
            inputMenuPlay =
                Input.GetKey(KeyCode.W)
                || Input.GetKey(KeyCode.A)
                || Input.GetKey(KeyCode.S)
                || Input.GetKey(KeyCode.D)
                || Input.GetKey(KeyCode.Space)
                || Input.GetKey(KeyCode.UpArrow)
                || Input.GetKey(KeyCode.DownArrow)
                || Input.GetKey(KeyCode.LeftArrow)
                || Input.GetKey(KeyCode.RightArrow)
                || Input.GetMouseButton(0)
                || Input.GetMouseButton(1)
                || (Input.GetAxis("Mouse ScrollWheel") != 0);
            inputMenuExit = Input.GetKey(KeyCode.Escape);
        }
        else
        {
            inputMenuPlay = false;
            inputMenuExit = false;
        }

        // Death.
        if (state == gameStates.DEATH)
        {
            inputDeathRestart = Input.GetKey(KeyCode.R);
            inputDeathMainMenu = Input.GetKey(KeyCode.Escape);
        }
        else
        {
            inputDeathRestart = false;
            inputDeathMainMenu = false;
        }
    }

    void KillAllMusic()
    {
        audSrcMenuMusic.Stop();
        audSrcBreakMusic.Stop();
        for (int i = 0; i < audSrcLevelMusic.Length; i++)
            audSrcLevelMusic[i].Stop();
    }
}
