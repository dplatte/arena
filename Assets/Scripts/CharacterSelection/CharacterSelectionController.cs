using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {
		if (GUI.Button (new Rect (Screen.width / 2 - 50, Screen.height - 40, 100, 20), "Enter World")) {
			SceneManager.LoadScene("Arena");
		}
	}
}
