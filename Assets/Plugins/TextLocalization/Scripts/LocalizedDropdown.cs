using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//******************************************************************************
namespace Localization
{
	public class LocalizedDropdown : LocalizedField
	{
		#region Script Parameters

		#endregion

		#region Properties

		#endregion

		#region Fields
		// Private -----------------------------------------------------------------
		private Dropdown mDropdown;
		private List<string> mKeyOptions = new List<string>();
		#endregion

		#region Unity Methods
		protected override void Start()
		{
			base.Start();
			mDropdown = GetComponent<Dropdown>();
			if(mDropdown)
			{
				if(InitDictionary())
				{
					mDropdown.onValueChanged.AddListener(delegate {DropdownValueChanged(mDropdown);});
					SetText();
				}
			}
		}
		#endregion

		#region Methods
		public override void SetText()
		{
			UpdateOption();
			UpdateLabel();
		}
		#endregion

		#region Implementation
		private void UpdateOption()
		{
			for(int i = 0; i < mKeyOptions.Count; i++)
			{
				mDropdown.options[i].text = mLocalizationManager.GetValue(mKeyOptions[i]);
			}
		}

		private void UpdateLabel()
		{
			mDropdown.captionText.text = mLocalizationManager.GetValue(mKeyOptions[mDropdown.value]);
		}
		
		private bool InitDictionary()
		{
			foreach(var option in mDropdown.options)
			{
				if(mKeyOptions.Contains(option.text))
				{
					Debug.LogWarningFormat(this, "{0} define two or more time in the same dropdown", option.text);
					mKeyOptions.Clear();
					return false;
				}
				mKeyOptions.Add(option.text);
			}
			return true;
		}

		private void DropdownValueChanged(Dropdown change)
		{
			UpdateLabel();
		}
		#endregion
	}
}
