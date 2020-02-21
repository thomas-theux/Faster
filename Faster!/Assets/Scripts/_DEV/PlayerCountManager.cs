using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rewired;

public class PlayerCountManager : MonoBehaviour {

    public TMP_Text PlayerCountText;

    // REWIRED
    private bool arrowLeft = false;
    private bool arrowRight = false;


    private void Awake() {
        DisplayPlayerCountText();
    }


    private void Update() {
        GetInput();
        ChangePlayerCount();
    }


    private void GetInput() {
        arrowLeft = ReInput.players.GetPlayer(0).GetButtonDown("Arrow Left");
        arrowRight = ReInput.players.GetPlayer(0).GetButtonDown("Arrow Right");
    }


    private void ChangePlayerCount() {
        if (arrowLeft) {
            if (GameManager.ManualPlayerCount > 1) {
                GameManager.ManualPlayerCount--;
            } else {
                GameManager.ManualPlayerCount = GameManager.PlayerMax;
            }

            DisplayPlayerCountText();
        }

        if (arrowRight) {
            if (GameManager.ManualPlayerCount < GameManager.PlayerMax) {
                GameManager.ManualPlayerCount++;
            } else {
                GameManager.ManualPlayerCount = 1;
            }

            DisplayPlayerCountText();
        }
    }


    private void DisplayPlayerCountText() {
        if (GameManager.ManualPlayerCount > 1) {
            PlayerCountText.text = GameManager.ManualPlayerCount + " players";
        } else {
            PlayerCountText.text = GameManager.ManualPlayerCount + " player";
        }
    }

}
