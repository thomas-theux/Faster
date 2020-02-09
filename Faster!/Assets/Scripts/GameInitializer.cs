using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour {

    private void Awake() {
        SceneManager.LoadScene("1 Main Menu");
    }

}
