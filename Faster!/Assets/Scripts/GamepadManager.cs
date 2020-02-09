using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GamepadManager : MonoBehaviour {

    // public void InitializeGamepads() {}

	public void Awake() {
		ReInput.ControllerConnectedEvent += OnControllerConnected;
		ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

		// DEV STUFF
		GameManager.ConnectedGamepads = ReInput.controllers.joystickCount;
		// GameManager.ConnectedGamepads = 4;

		GameManager.PlayerCount = GameManager.ConnectedGamepads;
	}


    void OnControllerConnected(ControllerStatusChangedEventArgs args) {
		if (GameManager.ConnectedGamepads < GameManager.PlayerMax) {
			GameManager.ConnectedGamepads = ReInput.controllers.joystickCount;
		} else {
			print("No more controllers allowed");
		}
	}


    void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {
		if (GameManager.ConnectedGamepads > 0) {
			GameManager.ConnectedGamepads = ReInput.controllers.joystickCount;
		} else {
			print("No more controllers to disconnect");
		}
	}

}
