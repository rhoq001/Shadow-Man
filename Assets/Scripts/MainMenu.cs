using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GUIStyle style;

	void OnGUI() {
		float h = Screen.height;
		float w = Screen.width;
		GUI.Label(new Rect(w/9, h/13, 7*w/9, 3*h/13), "Shadowman", style);
		if (GUI.Button(new Rect(w/9, 5*h/13, 7*w/9, 3*h/13), "Start")) {
			SceneManager.LoadScene("Transition", LoadSceneMode.Single);
		}
		if (GUI.Button(new Rect(w/9, 9*h/13, 7*w/9, 3*h/13), "Quit")) {
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}
	}
}
