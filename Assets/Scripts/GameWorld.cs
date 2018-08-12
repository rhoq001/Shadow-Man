using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWorld : MonoBehaviour {

	[HideInInspector]
	public bool paused;
	[HideInInspector]
	public GameObject current_player;
	public GameObject player;
	public GameObject boss;
	Texture2D red;
	Texture2D back;
	public float hb_width = 100;
	public float hb_height = 20;
	Health hb;
	public string nextScene;


	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		paused = false;

		red = new Texture2D(1, 1);
		red.SetPixel(0,0,Color.red);
		red.Apply();
		back = new Texture2D(1, 1);
		back.SetPixel(0,0,new Color(0.75f,0,0));
		back.Apply();
		hb = player.GetComponent<Health>();
		current_player = player;
	}

	// Update is called once per frame
	void Update () {
		if (boss == null) {
			SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (!paused) Cursor.lockState = CursorLockMode.None;
			else Cursor.lockState = CursorLockMode.Locked;
			paused = !paused;
		}
	}

	void DrawQuad(Rect position, Texture2D color) {
		GUI.skin.box.normal.background = color;
		GUI.Box(position, GUIContent.none);
	}

	void OnGUI() {
		if (!paused) {
			float current = hb.CurrentHealth();
			DrawQuad(new Rect(5, 5, hb_width, hb_height), back);
			DrawQuad(new Rect(5, 5, hb_width * current / hb.max, hb_height), red);
			GUI.Label(new Rect(5, 5, hb_width, hb_height), current + "/" + hb.max);
		}
		else {
			float h = Screen.height;
			float w = Screen.width;
			if (GUI.Button(new Rect(w/9, h/9, 7*w/9, h/3), "Resume")) {
				Cursor.lockState = CursorLockMode.Locked;
				paused = false;
			}
			if (GUI.Button(new Rect(w/9, 5*h/9, 7*w/9, h/3), "Quit")) {
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
				#else
				Application.Quit();
				#endif
			}
		}
	}
}
