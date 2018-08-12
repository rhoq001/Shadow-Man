using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Health))]
public class ViewHealth : Editor {

	// Use this for initialization
	void OnSceneGUI() {
		Health hlt = target as Health;
		Vector3 center = hlt.transform.position + hlt.offset;

		float v = 0.05f;
		float h = 0.25f;

		Vector3[] vertices = {
			center + new Vector3(-h,-v,0),
			center + new Vector3(-h,v,0),
			center + new Vector3(h,v,0),
			center + new Vector3(h,-v,0)
		};
		Handles.DrawSolidRectangleWithOutline(vertices, Color.red, Color.red);
	}
}
