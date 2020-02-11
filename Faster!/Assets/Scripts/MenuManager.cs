using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class MenuManager : MonoBehaviour {

    // REWIRED
    private bool actionButton = false;
    private bool quitButton = false;
    // private bool restartButton = false;


    private void Update() {
        // Get input from player one
        GetInput();

        if (!GameManager.GameStarted) {

            // Start game
            if (actionButton) {
                AudioManager.instance.Play("ButtonPress");
                GameManager.GameStarted = true;
                SceneManager.LoadScene("2 Light Me Up");
            }

        } else {

            // Quit to main menu
            if (quitButton) {
                GameManager.GameStarted = false;
                // GameManager.InitializedScoreArr = false;
                GameManager.PlayerLevelArr.Clear();
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
        // restartButton = ReInput.players.GetPlayer(0).GetButtonDown("Restart");
    }

}