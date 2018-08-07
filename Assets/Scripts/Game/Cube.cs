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
		Nothing = 0,
		Top,
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
		LockWithError,
		Selected,
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
	private CubeTexture mCubeTexture;
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
		if (mCubeTexture == null)
		{
			mCubeTexture = GameData.Get.CubeTextureData;
			if (mCubeTexture == null)
			{
				Debug.LogError("No textureManager loaded", this);
				return;
			}
		}
		switch (State)
		{
			case EState.Enable:
				GetComponent<Renderer>().material = mCubeTexture.DefaultMat;
				break;
			case EState.Disable:
				gameObject.SetActive(false);
				break;
			case EState.LockWithError:
				GetComponent<Renderer>().material = mCubeTexture.WrongMat;
				break;
			case EState.Lock:
				GetComponent<Renderer>().material = mCubeTexture.LockMat;
				break;
			case EState.Selected:
				GetComponent<Renderer>().material = mCubeTexture.SelectedMat;
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

	public ESide GetHitFace(RaycastHit hit)
	{
		int triIndex = hit.triangleIndex;
		if(triIndex == 0 || triIndex == 1)
			return ESide.Front;
		else if(triIndex == 2 || triIndex == 3)
			return ESide.Back;
		else if(triIndex == 4 || triIndex == 5)
			return ESide.Left;
		else if(triIndex == 6 || triIndex == 7)
			return ESide.Down;
		else if(triIndex == 8 || triIndex == 9)
			return ESide.Right;
		else if(triIndex == 10 || triIndex == 11)
			return ESide.Top;
		return ESide.Nothing;
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
		if(mCubeTexture == null)
		{
			mCubeTexture = GameData.Get.CubeTextureData;
			if(mCubeTexture == null)
			{
				Debug.LogError("No textureManager loaded", this);
				return;
			}
		}
		var tex = mCubeTexture.GetTexture((int)number, number - (int)number > 0 ? true: false);
		sideRenderer.material.mainTexture = tex;
	}
#endregion
}
