using UnityEngine;
using UnityEngine.UI;


//******************************************************************************
namespace Localization
{
	public class LocalizedText : LocalizedField
	{
	#region Script Parameters
		public string Key;
	#endregion
	
	#region Fields
		// Private -----------------------------------------------------------------
		private Text mText;
	#endregion
	
	#region Unity Methods
		protected override void Start () 
		{
			base.Start();
			mText = GetComponent<Text>();
			if(mText)
			{
				if(string.IsNullOrEmpty(Key))
					Key = mText.text;
				SetText();
			}
		}
	#endregion
	
	#region Methods
		public override void SetText()
		{
			if(mLocalizationManager == null)
				mText.text = Key;
			else
				mText.text = mLocalizationManager.GetValue(Key);
		}
	#endregion
	}
}
