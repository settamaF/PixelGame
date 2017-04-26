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
#endregion

#region Properties

#endregion

#region Fields
	// Private -----------------------------------------------------------------

#endregion

#region Methods
	public void SetDestroyAction()
	{
		Game.Get.CurrentAction = Game.EAction.Destroy;
	}

	public void SetShieldAction()
	{
		Game.Get.CurrentAction = Game.EAction.Shield;
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
#endregion

#region Implementation

#endregion
}
