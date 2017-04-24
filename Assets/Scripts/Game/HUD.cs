//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//******************************************************************************

public class HUD : MonoBehaviour 
{
#region Script Parameters
	public Button	DestroyBtn;
	public Button	ShieldBtn;
#endregion

#region Static

#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------

#endregion

#region Unity Methods

#endregion

#region Methods
	public void SetDestroyAction()
	{
		if(Game.Get.CurrentAction == Game.EAction.Selection)
		{
			Game.Get.CurrentAction = Game.EAction.Destroy;
			Game.Get.MakeAction(InputManager.Get.SelectedObj);
		}
		DestroyBtn.interactable = false;
		ShieldBtn.interactable = true;
		Game.Get.CurrentAction = Game.EAction.Destroy;
	}

	public void SetShieldAction()
	{
		if(Game.Get.CurrentAction == Game.EAction.Selection)
		{
			Game.Get.CurrentAction = Game.EAction.Shield;
			Game.Get.MakeAction(InputManager.Get.SelectedObj);
		}
		DestroyBtn.interactable = true;
		ShieldBtn.interactable = false;
		Game.Get.CurrentAction = Game.EAction.Shield;
	}

	public void SetSelectionAction()
	{
		DestroyBtn.interactable = true;
		ShieldBtn.interactable = true;
		Game.Get.CurrentAction = Game.EAction.Selection;
	}
#endregion

#region Implementation

#endregion
}
