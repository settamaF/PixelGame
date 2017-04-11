//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************

public class SwitchTexture : MonoBehaviour 
{
#region Script Parameters
	public List<Texture> Textures;
#endregion

#region Static

#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------

#endregion

#region Unity Methods
	void Awake()
	{

	}

	void Start () 
	{

	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Keypad1))
		{
			SetTexture(1);
		}
		else if(Input.GetKeyDown(KeyCode.Keypad2))
		{
			SetTexture(2);
		}
		else if(Input.GetKeyDown(KeyCode.Keypad3))
		{
			SetTexture(3);
		}
		else if(Input.GetKeyDown(KeyCode.Keypad4))
		{
			SetTexture(4);
		}
		else if(Input.GetKeyDown(KeyCode.Keypad4))
		{
			SetTexture(4);
		}
	}
	#endregion

	#region Methods

	#endregion

	#region Implementation
	private void SetTexture(int num)
	{
		if(num > Textures.Count)
		{
			Debug.LogError("no texture for num " + num);
			return;
		}
		Debug.Log("Set texture num " + num);
		var mat = gameObject.GetComponent<Renderer>().material;
		if (mat)
		{
			mat.mainTexture = Textures[num - 1];
		}
	}
	#endregion
}
