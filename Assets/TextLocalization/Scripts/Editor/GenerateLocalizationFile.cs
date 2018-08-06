#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Localization;
//******************************************************************************

public class GenerateLocalizationFile : EditorWindow
{
#region Fields
	// Gui Fields -----------------------------------------------------------------
	private string mFileName = "localizedText";
	private bool mXmlFile = false;
	private bool mJsonFile = false;
	private int mMetodSelected = 0;
	private List<string> mOptions;

	// Private -----------------------------------------------------------------
	private List<LocalizedField> mLocalizedFields;
#endregion

#region Unity Methods
	private void Awake()
	{
		mOptions = new List<string>();
		mOptions.Add(LocalizationManager.eDownloadMethod.RESOURCES.ToString());
		mOptions.Add(LocalizationManager.eDownloadMethod.STREAMINGASSET.ToString());
	}

	void OnGUI()
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		mFileName = EditorGUILayout.TextField("Filename", mFileName);
		mXmlFile = EditorGUILayout.Toggle("Generate Xml", mXmlFile);
		mJsonFile = EditorGUILayout.Toggle("Generate Json", mJsonFile);
		mMetodSelected = EditorGUILayout.Popup("Method", mMetodSelected, mOptions.ToArray());
		if(GUILayout.Button("Generate localization"))
			GenerateFile();
	}

#endregion

#region Methods
	[MenuItem("Tools/LocalizationEditor")]
	public static void ShowWindow()
	{
		 EditorWindow.GetWindow<GenerateLocalizationFile>();
	}
#endregion

#region Implementation
	private void GenerateFile()
	{
		if(!mXmlFile && !mJsonFile)
		{
			Debug.LogWarning("No type of file selected, generation failed");
			return;
		}
	}
#endregion
}
#endif
