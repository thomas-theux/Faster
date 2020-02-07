using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class PlayerSheet : MonoBehaviour {

    private GameObject GameManagerGO;
    private TimeManager timeManagerScript;

    public int PlayerID = 0;
    private Player player;

    public GameObject PlayerBulbGO;
    private GameObject PlayerLightGO;

    public bool pressedButton = false;

    // REWIRED
    private bool actionBtn = false;


    private void Awake() {
        GameManagerGO = GameObject.Find("GameManager");
        timeManagerScript = GameManagerGO.GetComponent<TimeManager>();
    }


    void Start() {
        player = ReInput.players.GetPlayer(PlayerID);
        PlayerLightGO = PlayerBulbGO.transform.GetChild(0).gameObject;
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
            if (!pressedButton) {
                pressedButton = true;
                GameManager.PlayerDoneCount++;

                if (TimeManager.LightIsOn) {
                    // Player presses the button properly (not too early)
                    AudioManager.instance.Play("ButtonPress");

                    timeManagerScript.GetTime(PlayerID, true);

                    PlayerLightGO.SetActive(true);
                    
                    PlayerLightGO.GetComponent<Image>().color = ColorManager.KeyGreen;
                    ParticleSystem.MainModule main = PlayerLightGO.transform.GetChild(0).GetComponent<ParticleSystem>().main;
                    main.startColor = ColorManager.ParticlesGreen;
                } else {
                    // Player pressed the button too early
                    AudioManager.instance.Play("FailSound");

                    timeManagerScript.GetTime(PlayerID, false);

                    PlayerLightGO.SetActive(true);
                    
                    PlayerLightGO.GetComponent<Image>().color = ColorManager.KeyRed;
                    ParticleSystem.MainModule main = PlayerLightGO.transform.GetChild(0).GetComponent<ParticleSystem>().main;
                    main.startColor = ColorManager.ParticlesRed;
                }
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
        pressedButton = false;
    }

}
