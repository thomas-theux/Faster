﻿using System.Collections;
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

    public static int ManualPlayerCountDEF = 2;
    public static int ManualPlayerCount = 2;

    public static int LevelMax = 0;

    public static int PlayerDoneCount = 0;
    public static bool ReadyToReset = false;

    public static bool GameOver = false;

    public static List<GameObject> PlayerInstances = new List<GameObject>();
    public static List<int> PlayerLevelArr = new List<int>();

    public static float BestTimeEver = 9999999;

    public static List<Sprite> BadgeImages = new List<Sprite>();

    // DEV STUFF
    // public int ManualPlayerCount = 0;
    // public int ManualLevelMax = 0;


    private void Awake() {
        //////////////////////////////////////////////////
        // DEV STUFF
        // PlayerCount = ManualPlayerCount;
        // LevelMax = ManualLevelMax;
        //////////////////////////////////////////////////
        PlayerCount = GameManager.ConnectedGamepads;
        LevelMax = 10;
        //////////////////////////////////////////////////

        if (PlayerCount < 1) {
            PlayerCount = ManualPlayerCount;
        }

        for (int i = 0; i < PlayerCount; i++) {
            // PlayerScoreArr.Add(0);
            PlayerLevelArr.Add(0);
        }
        
        playerInstanceSpawnerScript = GetComponent<PlayerInstanceSpawner>();
        timeManagerScript = GetComponent<TimeManager>();

        InitializeGame();
    }


    private void InitializeGame() {
        playerInstanceSpawnerScript.SpawnPlayerInstance();
        timeManagerScript.InitializeLightTimer();
    }


    private void OnDisable() {
        CleanUpGameManager();
    }


    public void CleanUpGameManager() {
        PlayerDoneCount = 0;
        ReadyToReset = false;
        PlayerInstances.Clear();
        GameOver = false;
    }


    public static void ResetPlayerCounts() {
        PlayerCount = 0;
        // ManualPlayerCount = ManualPlayerCountDEF;
    }

}
