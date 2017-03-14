using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour {

	string username = "Username";
	string password = "Password";

	bool Login() {
		if (username != "" && password != "") {
			return true;
		} else {
			return false;
		}
	}

	void OnGUI() {
		username = GUI.TextField (new Rect (Screen.width / 2 - 50, Screen.height - 100, 100, 20), username);
		password = GUI.TextField (new Rect (Screen.width / 2 - 50, Screen.height - 70, 100, 20), password);
		if (GUI.Button (new Rect (Screen.width / 2 - 50, Screen.height - 40, 100, 20), "Login")) {
			if (Login ()) {
				SceneManager.LoadScene("CharacterSelection");
			} else {
				Debug.Log ("Login invalid");
			}
		}
	}
}
