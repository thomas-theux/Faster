using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class MenuManager : MonoBehaviour {

    public Canvas ShowControls;
    private bool showingControls = false;

    // REWIRED
    private bool actionButton = false;
    private bool quitButton = false;
    private bool controlsButton = false;
    // private bool restartButton = false;


    private void Update() {
        // Get input from player one
        GetInput();

        if (!GameManager.GameStarted) {

            // Start game
            if (!showingControls) {
                if (actionButton) {
                    AudioManager.instance.Play("ButtonPress");
                    GameManager.GameStarted = true;
                    SceneManager.LoadScene("2 Light Me Up");
                }
            }

            // Show controls
            if (controlsButton) {
                AudioManager.instance.Play("ShowControls");

                showingControls = !showingControls;
                ShowControls.enabled = showingControls;
            }

        } else {

            // Quit to main menu
            if (quitButton) {
                AudioManager.instance.Play("Cancel");

                GameManager.GameStarted = false;
                GameManager.PlayerLevelArr.Clear();
                GameManager.ResetPlayerCounts();
                
                SceneManager.LoadScene("1 Main Menu");
            }

        }

        // Reset everything
        // if (GameManager.ReadyToReset) {
        //     if (restartButton) {
        //         PlayNextRound();
        //     }
        // }
    }


    public static void PlayNextRound() {
        // Check if the game is over due to a player reaching the max level
        if (GameManager.GameOver) {
            // Game IS over
        } else {
            // Game is NOT over
            AudioManager.instance.Play("Restart");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }


    private void GetInput() {
        actionButton = ReInput.players.GetPlayer(0).GetButtonDown("Action");
        quitButton = ReInput.players.GetPlayer(0).GetButtonDown("Quit");
        controlsButton = ReInput.players.GetPlayer(0).GetButtonDown("Controls");
        // restartButton = ReInput.players.GetPlayer(0).GetButtonDown("Restart");
    }

}