//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//******************************************************************************

public class HUD : MonoBehaviour 
{
#region Script Parameters
	public List<Button> DefaultBtns;
	public List<Button> SelectionBtns;
	public List<Image>  Hearts;
	public SwitchButton SwitchBtn;
#endregion

#region Properties

#endregion

#region Fields
	// Private -----------------------------------------------------------------

#endregion

#region Methods
	public void ResetAction()
	{
		Game.Get.CurrentAction = Game.EAction.Destroy;
		InputManager.Get.ResetSelectedObj();
		SwitchBtn.OnPress(DefaultBtns[0]);
	}

	public void SetDestroyAction()
	{
		Game.Get.CurrentAction = Game.EAction.Destroy;
		InputManager.Get.ResetSelectedObj();
	}

	public void SetShieldAction()
	{
		Game.Get.CurrentAction = Game.EAction.Shield;
		InputManager.Get.ResetSelectedObj();
	}

	public void SetSelectionAction()
	{
		Game.Get.CurrentAction = Game.EAction.Selection;
	}

	public void DestroySelection()
	{
		Game.Get.MakeAction(InputManager.Get.SelectedObj, Game.EAction.Destroy);
		EnableSelectionBtn(false);
	}

	public void LockSelection()
	{
		Game.Get.MakeAction(InputManager.Get.SelectedObj, Game.EAction.Shield);
		EnableSelectionBtn(false);
	}

	public void UnSelect()
	{
		InputManager.Get.ResetSelectedObj();
	}

	public void EnableSelectionBtn(bool enable)
	{
		if(SelectionBtns == null || SelectionBtns.Count <= 0)
			return;
		foreach(var btn in SelectionBtns)
		{
			btn.gameObject.SetActive(enable);
		}
		if(DefaultBtns == null || DefaultBtns.Count <= 0)
			return;
		foreach(var btn in DefaultBtns)
		{
			btn.gameObject.SetActive(!enable);
		}
	}

	public void UpdateHeart(int heart)
	{
		if(Hearts == null || Hearts.Count == 0)
			return;
		for(int i = 0; i < Hearts.Count; i++)
		{
			if(i < heart)
			{
				Hearts[i].transform.GetChild(0).gameObject.SetActive(true);
			}
			else
			{
				Hearts[i].transform.GetChild(0).gameObject.SetActive(false);
			}
		}
	}
#endregion

#region Implementation

#endregion
}
