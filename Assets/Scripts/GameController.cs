using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool inputMenuPlay;
    bool inputMenuExit;

    // Unity interface.
	void Start ()
    {
        obstacleSpeed = obstacleSpeedMin;
    }
	
    void FixedUpdate()
    {
        // State-machine for gameplay.
        switch (state)
        {
            case gameStates.MAIN_MENU:
                break;
            case gameStates.BREAK:

                break;
            case gameStates.LEVEL:
                break;
            case gameStates.DEATH:
                break;
            case gameStates.PAUSE:
                break;
        }
    }

	void Update ()
    {
        // Check for user input.
        UpdateInput();



        /*
        switch (state)
        {
            case gameStates.MAIN_MENU:
                break;
            case gameStates.BREAK:
                break;
            case gameStates.LEVEL:
                break;
            case gameStates.DEATH:
                break;
            case gameStates.PAUSE:
                break;
        }
        */
    }

    // Public interface.
    public void SetState(gameStates _state)
    {

        switch (_state)
        {
            case gameStates.MAIN_MENU:
                break;
            case gameStates.BREAK:
                // Increase level number.
                level += 1;
                // Increase obstacle speed.
                float newSp = (level / levelMax) * (obstacleSpeedMax - obstacleSpeedMin) + obstacleSpeedMin;
                obstacleSpeed = Mathf.Clamp(newSp, obstacleSpeedMin, obstacleSpeedMax);
                // Reset break timer.
                breakTimer = 0;
                break;
            case gameStates.LEVEL:
                // Reset level timer.
                levelTimer = 0;
                break;
            case gameStates.DEATH:
                break;
            case gameStates.PAUSE:
                break;
        }
        // Update state data.
        state = _state;
    }

    // Private interface.
    void UpdateInput()
    {
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

    }
}
