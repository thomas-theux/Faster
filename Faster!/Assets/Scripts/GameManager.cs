using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private GamepadManager gamepadManagerScript;
    private PlayerInstanceSpawner playerInstanceSpawnerScript;
    private TimeManager timeManagerScript;

    public static int PlayerCount = 0;
    public static int ConnectedGamepads = 0;
    public static int PlayerMax = 4;

    public static int PlayerDoneCount = 0;
    public static bool ReadyToReset = false;

    public static List<GameObject> PlayerLightsArr = new List<GameObject>();

    // DEV STUFF
    public int ManualPlayerCount = 0;


    private void Awake() {
        // DEV STUFF
        ConnectedGamepads = ManualPlayerCount;
        PlayerCount = ManualPlayerCount;

        Cursor.visible = false;
        
        gamepadManagerScript = GetComponent<GamepadManager>();
        playerInstanceSpawnerScript = GetComponent<PlayerInstanceSpawner>();
        timeManagerScript = GetComponent<TimeManager>();

        InitializeGame();
    }


    private void InitializeGame() {
        gamepadManagerScript.InitializeGamepads();
        playerInstanceSpawnerScript.SpawnPlayerInstance();
        timeManagerScript.InitializeLightTimer();
    }


    private void OnDisable() {
        CleanUpGameManager();
    }


    public void CleanUpGameManager() {
        PlayerDoneCount = 0;
        ReadyToReset = false;
        PlayerLightsArr.Clear();
    }

}
