//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using UnityEngine.UI;
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
	public static MenuManager Get { get; private set; }
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
		if (Get != null && Get != this)
		{
			Destroy(gameObject);
			return;
		}
		if (Get == null)
			Get = this;
		if (transform.parent == null)
			DontDestroyOnLoad(gameObject);
	}

	void Start ()
	{
		ShowMainMenu();
		InputManager.Get.enabled = false;
		DebugText.text = "(1/" + GameData.Get.ModelsData.Count + ")";
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
	public InputField DebugIdInputField;
	public Text       DebugText;

	public void LaunchModel()
	{
		LaunchModel(int.Parse(DebugIdInputField.text));
	}

	public void LaunchModel(int id)
	{
		id = id - 1;
		Model model;
		if (id < GameData.Get.ModelsData.Count)
			model = GameData.Get.ModelsData[id];
		else
		{
			Debug.LogError("No model with id: " + id + " available.", this);
			DebugIdInputField.text = "";
			return;
		}
		Game.Get.StartGame(model);
		if(Hud)
			ShowHUD(true);
		CloseMenu();
	}
#endregion
}
