//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using Localization;

[CreateAssetMenu(fileName = "Model", menuName = "Scriptable Object/Model", order = 1)]
public class Model : ScriptableObject
{
	#region Script Parameters
	public string		Name;
	public GameObject	Prefab;
	public Vector3Int	Size;
	public Vector3[]	ValidCube;
	#endregion

	#region Properties

	#endregion

	#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private LocalizationManager mLocalization;
	#endregion

	#region Methods
	public string GetNameModel()
	{
		if (mLocalization == null)
			mLocalization = LocalizationManager.Get;
		if (mLocalization)
			return mLocalization.GetValue(Utils.TextToKey(Name));
		return Name;
	}
	#endregion

	#region Implementation

	#endregion
}
