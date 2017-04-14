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
	public enum ESide
	{
		Top = 0,
		Down,
		Left,
		Right,
		Front,
		Back
	}

	public enum EState
	{
		Enable,
		Disable,
		Lock,
	}

	[System.Serializable]
	public class Side
	{
		public ESide		SidePos;
		public Renderer		FaceRenderer;
	}

#region Script Parameters
	public List<Side> Sides;
#endregion

#region Properties
	public Vector3		Position{ get; set; }
	public bool			Valid { get; set; }
	public EState		State {get;	set;}
	public bool			Visibility { get; set; }
	public Block		Parent{ get; set; }
#endregion

#region Fields
	// Private -----------------------------------------------------------------
#endregion

#region Unity Methods
	void Awake ()
	{
		Valid = false;
		State = EState.Enable;
		Visibility = true;
		EnableAllSide(false);
	}
#endregion

#region Methods

	public void SetValid()
	{
		Valid = true;
	}

	public void SetSideNumber(ESide sidePos, float number)
	{
		var side = GetSide(sidePos);
		if(side != null)
			SetTextureSide(side.FaceRenderer, number);
	}

	public void SetSideNumber(Dictionary<ESide, float> sides)
	{
		foreach(var item in sides)
		{
			var side = GetSide(item.Key);
			if(side != null)
				SetTextureSide(side.FaceRenderer, item.Value);
		}
	}

	public void	EnableSide(ESide sidePos, bool enable)
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

	public void SetState(EState state = EState.Enable)
	{
		if(State == state)
			return;
		State = state;
		var texManager = TextureManager.Get;
		if(texManager == null)
		{
			Debug.LogError("No textureManager loaded", this);
			return;
		}
		switch(State)
		{
			case EState.Enable:
				GetComponent<Renderer>().material = texManager.DefaultMat;
				break;
			case EState.Disable:
				gameObject.SetActive(false);
				break;
			case EState.Lock:
				GetComponent<Renderer>().material = texManager.LockMat;
				break;
		}
	}

	public void SetVisibility(bool visible)
	{
		if(Visibility == visible || State == EState.Disable)
			return;
		Visibility = visible;
		gameObject.SetActive(Visibility);
	}

#endregion

#region Implementation
	private Side GetSide(ESide sidePos)
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
	bool move;
	/* move cube */
	void OnMouseDown()
	{
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);

		offset = transform.position - Camera.main.ScreenToWorldPoint(
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		move = false;
	}

	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		//move Cube
		//transform.position = curPosition;

		//Rotate Block
		Vector3 curDirection = curScreenPoint - screenPoint;
		if(Mathf.Abs(curDirection.x) > 50 || Mathf.Abs(curDirection.y) > 50)
		{
			Parent.transform.Rotate(Vector3.up, -curDirection.x * Parent.SpeedRotation * Mathf.Deg2Rad, Space.World);
			Parent.transform.Rotate(Vector3.right, curDirection.y * Parent.SpeedRotation * Mathf.Deg2Rad, Space.World);
			move = true;
		}
		
	}

	/* Destroy cube */
	void OnMouseUp()
	{
		if(!move)
			Parent.DestroyCube(Position);
	}
	#endregion
}
