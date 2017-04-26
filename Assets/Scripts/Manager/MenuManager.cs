//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections.Generic;

//******************************************************************************

public enum eMenuType
{
	MainMenu,
	Pause,
	Option,
	Victory,
	Defeat
}

[System.Serializable]
public class Menu
{
	public GameObject Object;
	public eMenuType Type;
}

public class MenuManager : MonoBehaviour 
{
#region Script Parameters
	public List<Menu> Menus;
	public HUD Hud;
#endregion

#region Static
	private static MenuManager mInstance = null;
	public static MenuManager Get { get { return mInstance; } }
#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private Menu				mActualMenu;
#endregion

#region Unity Methods
	void Awake()
	{
		if (mInstance != null && mInstance != this)
		{
			DestroyImmediate(this, true);
			return;
		}
		DontDestroyOnLoad(this);
		mInstance = this;
		Debug.Log("MenuManager loaded", this);
	}

	void Start () 
	{
		ShowMainMenu();
		InputManager.Get.enabled = false;
	}
#endregion

#region Methods

	public void ShowMainMenu()
	{
		var menu = GetMenu(eMenuType.MainMenu);

		if (menu == null)
			return;
		menu.Object.SetActive(true);
		mActualMenu = menu;
	}

	public void ShowPause()
	{
		var menu = GetMenu(eMenuType.Pause);

		if (menu == null)
			return;
		if(Hud)
			ShowHUD(false);
		menu.Object.SetActive(true);
		Game.Get.PauseGame(true);
		mActualMenu = menu;
	}

	public void ShowHUD(bool value)
	{
		Hud.gameObject.SetActive(value);
	}

	public void ShowOption()
	{

	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void ShowPrev()
	{

	}

	public void ShowVictory()
	{
		var menu = GetMenu(eMenuType.Victory);

		if (menu == null)
			return;
		if(Hud)
			ShowHUD(false);
		menu.Object.SetActive(true);
		mActualMenu = menu;
	}

	public void ShowDefeat()
	{
		var menu = GetMenu(eMenuType.Defeat);

		if(menu == null)
			return;
		if(Hud)
			ShowHUD(false);
		menu.Object.SetActive(true);
		mActualMenu = menu;
	}

	public void CloseMenu()
	{
		if (mActualMenu == null)
			return;
		mActualMenu.Object.SetActive(false);
		if (mActualMenu.Type == eMenuType.Pause)
		{
			Game.Get.PauseGame(false);
			if(Hud)
				ShowHUD(true);
		}
	}

	public void LaunchGame()
	{
		int id = Random.Range(0, GameData.Get.ModelsData.Count);

		Game.Get.StartGame(GameData.Get.ModelsData[id]);
		if (Hud)
			ShowHUD(true);
		CloseMenu();
	}

	public void RestartGame()
	{
		Game.Get.RestartGame();
		if(Hud)
			ShowHUD(true);
		CloseMenu();
	}

	public void ReturnToMainMenu()
	{
		Game.Get.CloseGame();
		mActualMenu.Object.SetActive(false);
		ShowMainMenu();
		if (Hud)
			ShowHUD(false);
	}

#endregion

#region Implementation
	Menu GetMenu(eMenuType type)
	{
		foreach(var menu in Menus)
		{
			if (menu.Type == type)
				return menu;
		}
		Debug.LogError(type.ToString() + " not found in menu manager");
		return null;
	}
#endregion

#region Debug
	public void LaunchModel(int id)
	{
		GameData.ModelData model;
		if(id < GameData.Get.ModelsData.Count)
			model = GameData.Get.ModelsData[id];
		else
		{
			Debug.LogError("No model with id: " + id + " available. Load default model", this);
			model = GameData.Get.ModelsData[0];
		}
		Game.Get.StartGame(model);
		if(Hud)
			ShowHUD(true);
		CloseMenu();
	}
#endregion
}
