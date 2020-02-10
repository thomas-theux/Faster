using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // private GamepadManager gamepadManagerScript;
    private PlayerInstanceSpawner playerInstanceSpawnerScript;
    private TimeManager timeManagerScript;

    public static bool GameStarted = false;

    public static int PlayerCount = 0;
    public static int ConnectedGamepads = 0;
    public static int PlayerMax = 7;

    public static int LevelMax = 0;

    public static int PlayerDoneCount = 0;
    public static bool ReadyToReset = false;

    // public static bool InitializedScoreArr = false;

    public static bool GameOver = false;

    public static List<GameObject> PlayerLightsArr = new List<GameObject>();
    public static List<int> PlayerLevelArr = new List<int>();

    public static float BestTimeEver = 9999999;

    public static List<Sprite> BadgeImages = new List<Sprite>();

    // DEV STUFF
    public int ManualPlayerCount = 0;
    public int ManualLevelMax = 0;


    private void Awake() {
        // DEV STUFF
        // PlayerCount = GameManager.ConnectedGamepads;
        PlayerCount = ManualPlayerCount;
        LevelMax = ManualLevelMax;

        for (int i = 0; i < PlayerCount; i++) {
            // PlayerScoreArr.Add(0);
            PlayerLevelArr.Add(0);
        }
        
        // gamepadManagerScript = GetComponent<GamepadManager>();
        playerInstanceSpawnerScript = GetComponent<PlayerInstanceSpawner>();
        timeManagerScript = GetComponent<TimeManager>();

        // Load all badge images into a list and set level maximum
        // BadgeImages = new List<Sprite>(Resources.LoadAll<Sprite>("BadgeImages"));
        // LevelMax = BadgeImages.Count - 1;

        InitializeGame();
    }


    private void InitializeGame() {
        // gamepadManagerScript.InitializeGamepads();
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
        GameOver = false;
    }

}
