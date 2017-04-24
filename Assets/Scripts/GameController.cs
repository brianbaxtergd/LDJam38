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
    int level = 0;
    [SerializeField]
    int levelMax;
    [SerializeField]
    float breakTimerMax;
    float breakTimer = 0;
    [SerializeField]
    float levelTimerMax;
    float levelTimer = 0;
    [SerializeField]
    float obstacleSpeedMin;
    [SerializeField]
    float obstacleSpeedMax;
    float obstacleSpeed;

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

    AudioSource audSrcMove;
    AudioSource audSrcBoost;
    AudioSource audSrcJumpUp;
    AudioSource audSrcJumpDown;
    AudioSource audSrcPortal;
    AudioSource audSrcDeath;
    AudioSource audSrcGroundColl;

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

        audSrcMove          = GameObject.Find("MovementAudio").GetComponent<AudioSource>();
        audSrcBoost         = GameObject.Find("BoostAudio").GetComponent<AudioSource>();
        audSrcJumpUp        = GameObject.Find("JumpUpAudio").GetComponent<AudioSource>();
        audSrcJumpDown      = GameObject.Find("JumpDownAudio").GetComponent<AudioSource>();
        audSrcPortal        = GameObject.Find("PortalAudio").GetComponent<AudioSource>();
        audSrcDeath         = GameObject.Find("DeathAudio").GetComponent<AudioSource>();
        audSrcGroundColl    = GameObject.Find("GroundCollisionAudio").GetComponent<AudioSource>();
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

        switch (_state)
        {
            case gameStates.MAIN_MENU:
                break;
            case gameStates.BREAK:
                // Increase level number.
                if (state != gameStates.DEATH)
                    level += 1;
                // Increase obstacle speed.
                float newSp = obstacleSpeedMin + ((float)level / (float)levelMax) * (obstacleSpeedMax - obstacleSpeedMin);
                obstacleSpeed = Mathf.Clamp(newSp, obstacleSpeedMin, obstacleSpeedMax);
                // Reset break & level timers.
                breakTimer = breakTimerMax;
                levelTimer = levelTimerMax;
                break;
            case gameStates.LEVEL:
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
}
