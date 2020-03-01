using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstanceSpawner : MonoBehaviour {

    public GameObject PlayerInstance;

    public GameObject PlayerBulbsContainer;
    public GameObject PlayerBulb;

    private float bulbDistance = 250.0f;
    private float bulbWidth = 60.0f;
    private float bulbHeight = 60.0f;


    public void SpawnPlayerInstance() {
        // Resize player bulbs window
        float newWidth = ((bulbDistance - bulbWidth) * (GameManager.PlayerCount - 1)) + (bulbWidth * GameManager.PlayerCount);

        PlayerBulbsContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidth, bulbHeight);

        for (int i = 0; i < GameManager.PlayerCount; i++) {
            // Build basic player instance
            // GameObject newPlayerInstance = Instantiate(PlayerInstance);
            // newPlayerInstance.name = "Player " + i;
            // newPlayerInstance.GetComponent<PlayerSheet>().PlayerID = i;

            // Build and position player bulbs
            float newPositionX = bulbDistance * i;

            GameObject newPlayerBulb = Instantiate(PlayerBulb, PlayerBulbsContainer.transform, false);

            newPlayerBulb.name = "Player " + i;
            newPlayerBulb.GetComponent<PlayerSheet>().PlayerID = i;

            newPlayerBulb.transform.localPosition = new Vector2(
                newPlayerBulb.transform.localPosition.x + newPositionX,
                newPlayerBulb.transform.localPosition.y
            );

            // Attach player light to player
            // newPlayerInstance.GetComponent<PlayerSheet>().PlayerBulbGO = newPlayerBulb.gameObject;
            newPlayerBulb.GetComponent<PlayerSheet>().PlayerBulbGO = newPlayerBulb.gameObject;

            GameManager.PlayerInstances.Add(newPlayerBulb);
        }
    }

}
