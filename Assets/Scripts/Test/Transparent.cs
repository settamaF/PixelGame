//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;

//******************************************************************************

public class Transparent : MonoBehaviour 
{
#region Script Parameters

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
		if(Input.GetKeyDown(KeyCode.A))
		{
			SetTransparent(true);	
		}
		else if (Input.GetKeyDown(KeyCode.Z))
		{
			SetTransparent(false);
		}
		else if (Input.GetKeyDown(KeyCode.W))
		{
			SetColor(Color.white);
		}
		else if(Input.GetKeyDown(KeyCode.B))
		{
			SetColor(Color.blue);
		}
	}
#endregion

#region Methods

#endregion

#region Implementation
	private void SetTransparent(bool transparent)
	{
		if(transparent)
			Debug.Log("Transparent");
		else
			Debug.Log("Opaque");
		var renderers = gameObject.GetComponentsInChildren<Renderer>();
		if(renderers != null)
		{
			foreach(var rend in renderers)
			{
				var mats = rend.materials;
				if(mats != null)
				{
					foreach(var mat in mats)
					{
						var color = mat.color;
						color.a = transparent ? 0 : 1;
						mat.color = color;
					}
				}
			}
		}
	}

	private void SetColor(Color color)
	{
		var mats = GetComponent<Renderer>().materials;
		if(mats != null)
		{
			foreach(var mat in mats)
			{
				var a = mat.color.a;
				color.a = a;
				mat.color = color;
			}
		}
	}
#endregion
}
