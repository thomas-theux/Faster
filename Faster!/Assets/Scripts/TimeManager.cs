using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Rewired;

public class TimeManager : MonoBehaviour {

    public GameObject lightBulbGO;

    public static bool LightIsOn = false;

    private float delayTime = 0;
    public float minDelay = 0.5f;
    public float maxDelay = 5.0f;

    private float lightOnTime = 0;
    private bool ShowPopUps = false;

    private bool timerIsRunning = false;

    // public List<Hashtable> timerDict = new List<Hashtable>();
    public List<float> timesArr = new List<float>();


    public void InitializeLightTimer() {
        SetRandomTime();
        timerIsRunning = true;

        // Hashtable dictEntry = new Hashtable();
        // dictEntry.Add("Player ID", 0);
        // dictEntry.Add("Player Time", 0);

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            // timerDict.Add(dictEntry);
            timesArr.Add(0);
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }


    private void SetRandomTime() {
        delayTime = Random.Range(minDelay, maxDelay);
    }


    private void TimerCountdown() {
        delayTime -= Time.deltaTime;

        if (delayTime <= 0) {
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
            newTime = Mathf.Round(newTime);
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
        for (int i = 0; i < GameManager.PlayerCount; i++) {
            GameObject newPopUp = GameManager.PlayerLightsArr[i].transform.GetChild(1).gameObject;
            newPopUp.SetActive(true);

            // float playerTime = (float)timerDict[i]["Player Time"];
            float playerTime = timesArr[i];
            
            if (playerTime > 99999) {
                newPopUp.transform.GetChild(0).GetComponent<TMP_Text>().text = "–";
            } else {
                newPopUp.transform.GetChild(0).GetComponent<TMP_Text>().text = playerTime + "";
            }
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
