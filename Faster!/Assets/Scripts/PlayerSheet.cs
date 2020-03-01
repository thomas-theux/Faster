using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PlayerSheet : MonoBehaviour {

    private GameObject GameManagerGO;
    private TimeManager timeManagerScript;
    private BulbManager bulbManagerScript;

    public int PlayerID = 0;
    // public int PlayerScore = 0;
    public float PlayerTime = -1;
    public bool PressedButton = false;

    // public float PlayerTim = 22;

    private Player player;

    public GameObject PlayerBulbGO;
    private GameObject PlayerLightGO;

    // REWIRED
    private bool actionBtn = false;


    private void Awake() {
        GameManagerGO = GameObject.Find("GameManager");
        timeManagerScript = GameManagerGO.GetComponent<TimeManager>();
    }


    void Start() {
        bulbManagerScript = GameManager.PlayerInstances[PlayerID].GetComponent<BulbManager>();

        player = ReInput.players.GetPlayer(PlayerID);
        PlayerLightGO = bulbManagerScript.PlayerLight;
    }


    private void Update() {
        GetInput();
        SetMyTime();
    }


    private void GetInput() {
        actionBtn = player.GetButtonDown("Action");
    }


    private void SetMyTime() {
        if (actionBtn) {
            if (!PressedButton) {
                PressedButton = true;

                if (TimeManager.LightIsOn) {
                    // Player presses the button properly (not too early)
                    AudioManager.instance.Play("ActivateLight");

                    // Add this players ID to the ranking array
                    // DO WE NEED THAT?
                    timeManagerScript.rankingArr.Add(PlayerID);

                    // Get player time
                    // timeManagerScript.GetTime(PlayerID, true);
                    PlayerTime = TimeManager.LightIsOnTimer;
                    PlayerTime *= 1000;

                    PlayerLightGO.SetActive(true);
                    
                    // Set color of the player light and particles
                    PlayerLightGO.GetComponent<Image>().color = ColorManager.KeyWhite;
                    ParticleSystem.MainModule main = bulbManagerScript.PlayerParticles.GetComponent<ParticleSystem>().main;
                    main.startColor = ColorManager.ParticlesWhite;
                } else {
                    // Player pressed the button too early
                    AudioManager.instance.Play("FailSound");

                    // timeManagerScript.GetTime(PlayerID, false);

                    PlayerLightGO.SetActive(true);
                    
                    PlayerLightGO.GetComponent<Image>().color = ColorManager.KeyRed;
                    ParticleSystem.MainModule main = bulbManagerScript.PlayerParticles.GetComponent<ParticleSystem>().main;
                    main.startColor = ColorManager.ParticlesRed;
                }

                // Check if every player pressed the button
                GameManager.PlayerDoneCount++;
                timeManagerScript.AllPlayersDone();
            } else {
                // Player already pressed the button
                AudioManager.instance.Play("FailSound");
            }
        }
    }


    private void OnDisable() {
        CleanUpPlayerSheet();
    }


    public void CleanUpPlayerSheet() {
        PlayerID = 0;
        PressedButton = false;
        PlayerTime = -1;
    }

}
