//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//******************************************************************************

public class SwitchButton : MonoBehaviour 
{
#region Script Parameters
	public List<Button> Buttons;
#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------

#endregion

#region Unity Methods
	void Awake()
	{
		if(Buttons != null && Buttons.Count > 0)
			OnPress(Buttons[0]);
	}
#endregion

#region Methods
	public void OnPress(Button currentBtn)
	{
		foreach(var button in Buttons)
		{
			if(button != currentBtn)
			{
				button.interactable = true;
			}
			else
				button.interactable = false;
		}
	}
#endregion

#region Implementation

#endregion
}
