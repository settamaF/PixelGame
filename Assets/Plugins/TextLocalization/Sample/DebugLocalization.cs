using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Localization;
//******************************************************************************

public class DebugLocalization : MonoBehaviour 
{
#region Script Parameters
	public Text DebugType;
	public Text DebugMethod;
#endregion

#region Static

#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private LocalizationManager mLocalManager;
#endregion

#region Unity Methods
	void Awake()
	{
		
	}

	void Start () 
	{
		mLocalManager = LocalizationManager.Get;
		if(mLocalManager == null)
		{
			Debug.LogWarning("No LocalizationManager instance found");
			this.enabled = false;
		}
		DebugType.text = mLocalManager.TypeOfFile.ToString();
		DebugMethod.text = mLocalManager.DownloadMethod.ToString();
	}

	public void SwitchTo(string language)
	{
		mLocalManager.SwitchLocalization(language);
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.J))
		{
			DebugType.text = "Json";
			mLocalManager.TypeOfFile = LocalizationManager.eTypeOfFile.JSON;
			mLocalManager.SwitchLocalization(mLocalManager.CurrentLanguage);
		}
		else if(Input.GetKeyDown(KeyCode.X))
		{
			DebugType.text = "Xml";
			mLocalManager.TypeOfFile = LocalizationManager.eTypeOfFile.XML;
			mLocalManager.SwitchLocalization(mLocalManager.CurrentLanguage);
		}
		else if(Input.GetKeyDown(KeyCode.R))
		{
			DebugMethod.text = "Resources";
			mLocalManager.DownloadMethod = LocalizationManager.eDownloadMethod.RESOURCES;
			mLocalManager.SwitchLocalization(mLocalManager.CurrentLanguage);
		}
		else if(Input.GetKeyDown(KeyCode.S))
		{
			DebugMethod.text = "StreamingAsset";
			mLocalManager.DownloadMethod = LocalizationManager.eDownloadMethod.STREAMINGASSET;
			mLocalManager.SwitchLocalization(mLocalManager.CurrentLanguage);
		}
	}
	#endregion

	#region Methods

	#endregion

	#region Implementation

	#endregion
}
