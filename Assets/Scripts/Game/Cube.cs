//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************

public class Cube : MonoBehaviour
{
	[System.Serializable]
	public enum eSide
	{
		Top = 0,
		Down,
		Left,
		Right,
		Front,
		Back
	}

	public enum eState
	{
		Enable,
		Disable,
		Lock,
	}

	[System.Serializable]
	public class Side
	{
		public eSide		SidePos;
		public Renderer		FaceRenderer;
	}

#region Script Parameters
	public List<Side> Sides;
#endregion

#region Properties
	public bool			Valid { get; set; }
	public eState		State {get;	set;}
	public bool			Visibility { get; set; }
#endregion

#region Fields
	// Private -----------------------------------------------------------------
#endregion

#region Unity Methods
	void Awake ()
	{
		Valid = false;
		State = eState.Enable;
		Visibility = true;
		EnableAllSide(false);
	}
#endregion

#region Methods

	public void SetValid()
	{
		Valid = true;
	}

	public void SetSideNumber(eSide sidePos, float number)
	{
		var side = GetSide(sidePos);
		if(side != null)
			SetTextureSide(side.FaceRenderer, number);
	}

	public void SetSideNumber(Dictionary<eSide, float> sides)
	{
		foreach(var item in sides)
		{
			var side = GetSide(item.Key);
			if(side != null)
				SetTextureSide(side.FaceRenderer, item.Value);
		}
	}

	public void	EnableSide(eSide sidePos, bool enable)
	{
		var side = GetSide(sidePos);
		if(side != null)
			side.FaceRenderer.gameObject.SetActive(enable);
	}

	public void EnableAllSide(bool enable)
	{
		foreach(var side in Sides)
		{
			if(side.FaceRenderer != null)
				side.FaceRenderer.gameObject.SetActive(enable);
		}
	}

	public void SetState(eState state = eState.Enable)
	{
		if(State == state)
			return;
		State = state;
		var mat = GetComponent<Renderer>().material;
		var texManager = TextureManager.Get;
		if(texManager == null)
		{
			Debug.LogError("No textureManager loaded", this);
			return;
		}
		switch(State)
		{
			case eState.Enable:
				mat = texManager.DefaultMat;
				break;
			case eState.Disable:
				gameObject.SetActive(false);
				break;
			case eState.Lock:
				mat = texManager.LockMat;
				break;
		}
	}

	public void SetVisibility(bool visible)
	{
		if(Visibility == visible || State == eState.Disable)
			return;
		Visibility = visible;
		gameObject.SetActive(Visibility);
	}

#endregion

#region Implementation
	private Side GetSide(eSide sidePos)
	{
		foreach(var side in Sides)
		{
			if(side.SidePos == sidePos)
				return side;
		}
		return null;
	}

	private void SetTextureSide(Renderer sideRenderer, float number)
	{
		var texManager = TextureManager.Get;
		if(texManager == null)
		{
			Debug.LogError("No textureManager loaded", this);
			return;
		}
		var tex = texManager.GetTexture((int)number, number - (int)number > 0 ? true: false);
		sideRenderer.material.mainTexture = tex;
	}
#endregion

#region Debug
	Vector3 screenPoint;
	Vector3 offset;


	void OnMouseDown()
	{
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);

		offset = transform.position - Camera.main.ScreenToWorldPoint(
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));


	}

	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;
	}

#endregion
}
