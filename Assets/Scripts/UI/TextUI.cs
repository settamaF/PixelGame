//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//******************************************************************************

public class TextUI : MonoBehaviour 
{
	#region Script Parameters
	public bool TextUpdate = false;
	#endregion

	#region Static

	#endregion

	#region Properties

	#endregion

	#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private Text	mText;
	private string	mTextID;
	#endregion

	#region Unity Methods
	void Awake()
	{

	}

	void Start () 
	{
		mText = this.GetComponent<Text>();
		if (mText == null)
		{
			Debug.LogError(this.name + " don't have component Text");
			this.enabled = false;
			return;
		}
		mTextID = mText.text.ToUpper().Trim();
		Refresh();
		if (!TextUpdate)
			this.enabled = false;
	}

	void Update()
	{
		Refresh();
	}
	#endregion

	#region Methods
	public void Refresh()
	{
		mText.text = TextManager.Get.RequestText(mTextID);
	}
	#endregion

	#region Implementation

	#endregion
}
