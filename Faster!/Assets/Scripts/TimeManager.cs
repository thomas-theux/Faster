using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Rewired;

public class TimeManager : MonoBehaviour {

    public GameObject lightBulbGO;
    public TMP_Text BestTimeEver;
    public GameObject MSText;

    public static bool LightIsOn = false;

    private float delayTime = 0;
    public float minDelay = 1.0f;
    public float maxDelay = 10.0f;

    private float lightOnTime = 0;
    private bool ShowPopUps = false;

    private bool timerIsRunning = false;

    // public List<Hashtable> timerDict = new List<Hashtable>();
    public List<float> timesArr = new List<float>();


    public void InitializeLightTimer() {
        SetRandomTime();
        timerIsRunning = true;

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            timesArr.Add(0);
            GameManager.PlayerLightsArr[i].GetComponent<BulbManager>().ScoreText.GetComponent<TMP_Text>().text = GameManager.PlayerLevelArr[i] + "";
            GameManager.PlayerLightsArr[i].GetComponent<BulbManager>().PlayerBadge.GetComponent<Image>().sprite = GameManager.BadgeImages[GameManager.PlayerLevelArr[i]];
        }

        // Display best time ever
        if (GameManager.BestTimeEver < 999999) {
            BestTimeEver.text = GameManager.BestTimeEver.ToString("F1");

            if (!MSText.activeSelf) {
                MSText.SetActive(true);
            }
        }
    }


    private void Update() {
        // Start counting down to turn the light on
        if (timerIsRunning) {
            TimerCountdown();
        }

        // Start counting the time when light is turned on
        if (LightIsOn) {
            LightOnTime();
        }

        // Show time popups
        if (GameManager.PlayerDoneCount == GameManager.PlayerCount) {
            if (!ShowPopUps) {
                StartCoroutine(ShowPopUpsDelay());
            }
        }

        // Reset everything
        if (GameManager.ReadyToReset) {
            if (ReInput.players.GetPlayer(0).GetButtonDown("Restart")) {

                // Check if the game is over due to a player reaching the max level
                if (GameManager.GameOver) {
                    // Game IS over
                    print("game over");
                } else {
                    // Game is NOT over
                    AudioManager.instance.Play("Restart");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }


    private void SetRandomTime() {
        delayTime = Random.Range(minDelay, maxDelay);
    }


    private void TimerCountdown() {
        delayTime -= Time.deltaTime;

        if (delayTime <= 0) {
            AudioManager.instance.Play("LightOn");

            timerIsRunning = false;
            lightBulbGO.SetActive(true);
            lightBulbGO.GetComponent<Image>().color = ColorManager.KeyWhite;
            LightIsOn = true;
        }
    }


    private void LightOnTime() {
        lightOnTime += Time.deltaTime;
    }


    public void GetTime(int playerID, bool rightOnTime) {
        float newTime = 999999;

        if (rightOnTime) {
            newTime = lightOnTime;

            // Calculate to milliseconds
            newTime *= 1000;
            newTime = Mathf.Round(newTime * 100f) / 100f;
            // newTime *= 1000;
            // newTime = Mathf.Round(newTime);
        }

        // timerDict[playerID]["Player ID"] = playerID;
        // timerDict[playerID]["Player Time"] = newTime;
        timesArr[playerID] = newTime;

        // Hashtable dictEntry = new Hashtable();

        // dictEntry["Player ID"] = playerID;
        // dictEntry["Player Time"] = newTime;

        // timerDict.Add(dictEntry);

        // print((float)timerDict[playerID]["Player Time"]);
        // print("Player " + playerID + ": " + newTime);
    }


    private IEnumerator ShowPopUpsDelay() {
        ShowPopUps = true;

        yield return new WaitForSeconds(1.0f);

        ShowTimePopUps();
        GameManager.ReadyToReset = true;
    }


    private void ShowTimePopUps() {
        AudioManager.instance.Play("PopUp");

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            BulbManager bulbManagerScript = GameManager.PlayerLightsArr[i].GetComponent<BulbManager>();

            GameObject newPopUp = bulbManagerScript.TimePopUp;
            newPopUp.SetActive(true);

            // float playerTime = (float)timerDict[i]["Player Time"];
            float playerTime = timesArr[i];
            
            if (playerTime > 99999) {
                bulbManagerScript.PopUpTime.GetComponent<TMP_Text>().text = "–";
            } else {
                bulbManagerScript.PopUpTime.GetComponent<TMP_Text>().text = playerTime.ToString("F1");
            }
        }

        FindWinner();
    }


    private void FindWinner() {
        float bestTime = 9999999;

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            if (timesArr[i] < bestTime) {
                bestTime = timesArr[i];
            }
        }

        int findWinnerIndex = timesArr.IndexOf(bestTime);

        // Find best time ever
        if (bestTime < GameManager.BestTimeEver) {
            GameManager.BestTimeEver = bestTime;
            BestTimeEver.text = GameManager.BestTimeEver.ToString("F1");
        }

        BulbManager bulbManagerScript = GameManager.PlayerLightsArr[findWinnerIndex].GetComponent<BulbManager>();

        // Add one level and display the current level
        GameManager.PlayerLevelArr[findWinnerIndex]++;
        bulbManagerScript.ScoreText.GetComponent<TMP_Text>().text = GameManager.PlayerLevelArr[findWinnerIndex] + "";

        // Display new badge/level
        bulbManagerScript.PlayerBadge.GetComponent<Image>().sprite = GameManager.BadgeImages[GameManager.PlayerLevelArr[findWinnerIndex]];

        // print("Lvl " + GameManager.PlayerLevelArr[findWinnerIndex] + "/" + GameManager.LevelMax - 1);

        // Check if the game is over due to a player reaching the max level
        if (GameManager.PlayerLevelArr[findWinnerIndex] == GameManager.LevelMax) {
            GameManager.GameOver = true;
        }


        if (!MSText.activeSelf) {
            MSText.SetActive(true);
        }
    }


    private void OnDisable() {
        CleanUpLightTimer();
    }


    public void CleanUpLightTimer() {
        LightIsOn = false;
        delayTime = 0;
        lightOnTime = 0;
        ShowPopUps = false;
        timerIsRunning = false;
        timesArr.Clear();
    }

}
