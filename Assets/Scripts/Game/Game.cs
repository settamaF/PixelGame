//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

using System.Collections.Generic;
using UnityEngine;

//******************************************************************************

public class Game : MonoBehaviour
{
	public enum EAction
	{
		Nothing,
		Destroy,
		Shield,
		Selection
	}

#region Script Parameters
	public GameObject   MainBlock;
#endregion

#region Properties
	public GameData.ModelData	CurrentModel { get; set; }
	public EAction				CurrentAction { get; set; }
#endregion

#region Fields
	//const
	private const int			LIFEBYDEFAULT = -1; //Infiny
	public const float			DEFAULTPOSZCAMERA = -10f;
	// static
	private static Game			mInstance;
	public static Game			Get { get{ return mInstance; } }

	//private
	private bool				mPause = false;
	private Block				mCurrentBlock = null;
	private int					mCurrentLife;
#endregion
	
#region Unity Methods
	void Awake()
	{
		mInstance = this;
		this.enabled = false;
	}

	void Update()
	{
		if (mPause)
			return;
		CheckVictory();
	}
#endregion

#region Methods

	public void StartGame(GameData.ModelData model)
	{
		if(mCurrentBlock)
			Destroy(mCurrentBlock.gameObject);
		mPause = false;
		InputManager.Get.enabled = true;
		this.enabled = true;
		var tmp = Instantiate<GameObject>(MainBlock, Vector3.zero, Quaternion.identity);
		mCurrentBlock = tmp.GetComponent<Block>();
		CurrentModel = model;
		mCurrentBlock.Init(model);
		InputManager.Get.Block = mCurrentBlock;
		MenuManager.Get.Hud.SetDestroyAction();
		mCurrentLife = LIFEBYDEFAULT;
	}

	public void PauseGame(bool value)
	{
		mPause = value;
		InputManager.Get.enabled = !value;
	}

	public void RestartGame()
	{
		StartGame(CurrentModel);
	}

	public void CloseGame()
	{
		if(mCurrentBlock)
		{
			Destroy(mCurrentBlock.gameObject);
			mCurrentBlock = null;
		}
		InputManager.Get.enabled = false;
		this.enabled = false;
	}

	public void MakeAction(List<GameObject> objs, EAction action)
	{
		var prevAction = CurrentAction;
		CurrentAction = action;
		foreach(var obj in objs)
		{
			if(this.enabled == false)
				return;
			MakeAction(obj);
		}
		InputManager.Get.ClearSelectedObj();
		CurrentAction = prevAction;
	}

	public void MakeAction(GameObject obj)
	{
		var cube = obj.GetComponent<Cube>();
		if(cube)
		{
			switch(CurrentAction)
			{
				case EAction.Destroy:
					mCurrentBlock.DestroyCube(cube.Position);
					break;
				case EAction.Shield:
					mCurrentBlock.LockCube(cube.Position);
					break;
				default:
					break;
			}
		}
	}

	public void RemoveLife()
	{
		mCurrentLife--;
		if(mCurrentLife == 0)
		{
			Defeat();
		}
	}
#endregion

#region Implementation

	bool CheckVictory()
	{
		if(mCurrentBlock.GetComponent<Block>().IsCompleted())
		{
			Victory();
			return true;
		}
		return false;
	}

	void Defeat()
	{
		MenuManager.Get.ShowDefeat();
		this.enabled = false;
		InputManager.Get.enabled = false;
	}

	void Victory()
	{
		MenuManager.Get.ShowVictory();
		this.enabled = false;
		InputManager.Get.enabled = false;
	}

	void ResetGame()
	{
		
	}

#endregion
}
