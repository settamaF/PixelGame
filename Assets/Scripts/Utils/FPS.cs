//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;

//******************************************************************************

public class FPS : MonoBehaviour 
{
	#region Fields
	private float mDeltaTime = 0f;
	#endregion

	#region Unity Methods
	void Update()
	{
		mDeltaTime += (Time.deltaTime - mDeltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = Color.white;
		float msec = mDeltaTime * 1000.0f;
		float fps = 1.0f / mDeltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
	#endregion

	#region Methods

	#endregion

	#region Implementation

	#endregion
}
