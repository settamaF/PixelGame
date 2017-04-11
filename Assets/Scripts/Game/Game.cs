//******************************************************************************
// Author: Frederic SETTAMA
//******************************************************************************

using System.Collections.Generic;
using UnityEngine;

//******************************************************************************

public class Game : MonoBehaviour
{

#region Script Parameters

#endregion

#region Script Debug Parameters

#endregion

#region Properties

#endregion

#region Fields
	// const


	// static
	private static Game		mInstance;
	public static Game		Get { get{ return mInstance; } }

	//private
	private bool mPause = false;
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
	}
#endregion

#region Methods

	public void StartGame()
	{
		mPause = false;
		InputManager.Get.enabled = true;
		this.enabled = true;
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
		InputManager.Get.enabled = false;
		this.enabled = false;
	}
#endregion

#region Implementation

	bool CheckVictory()
	{
		return false;
	}

	void Victory()
	{
		MenuManager.Get.ShowVictory();
		InputManager.Get.enabled = false;
	}

	void ResetGame()
	{
		
	}

#endregion
}
