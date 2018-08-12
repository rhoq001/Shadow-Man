using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMain : MonoBehaviour {

	void Start() {
		Cursor.lockState = CursorLockMode.None;
	}

	void OnGUI() {
		float w = Screen.width;
		float h = Screen.height;
		if (GUI.Button(new Rect(w/4, 8*h/10, w/2, h/10), "Return To Main Menu")) {
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
		}
	}

}
