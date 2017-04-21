//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

using System.Collections.Generic;
using UnityEngine;

//******************************************************************************

public class Game : MonoBehaviour
{

#region Script Parameters
	public GameObject   MainBlock;
#endregion

#region Script Debug Parameters

#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------


	// static
	private static Game		mInstance;
	public static Game		Get { get{ return mInstance; } }

	//private
	private bool mPause = false;
	private GameObject  mCurrentBlock = null;
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

	public void StartGame()
	{
		if(mCurrentBlock)
			Destroy(mCurrentBlock);
		mPause = false;
		InputManager.Get.enabled = true;
		this.enabled = true;
		mCurrentBlock = Instantiate<GameObject>(MainBlock, Vector3.zero, Quaternion.identity);
		InputManager.Get.Block = mCurrentBlock.GetComponent<Block>();
	}

	public void PauseGame(bool value)
	{
		mPause = value;
		InputManager.Get.enabled = !value;
	}

	public void RestartGame()
	{
		StartGame();
	}

	public void CloseGame()
	{
		if(mCurrentBlock)
		{
			Destroy(mCurrentBlock);
			mCurrentBlock = null;
		}
		InputManager.Get.enabled = false;
		this.enabled = false;
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
