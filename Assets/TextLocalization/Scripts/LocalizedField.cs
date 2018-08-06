using UnityEngine;

//******************************************************************************

namespace Localization
{
	public abstract class LocalizedField : MonoBehaviour
	{
		#region Fields
		// Const -------------------------------------------------------------------

		// Static ------------------------------------------------------------------

		// Private -----------------------------------------------------------------
		protected LocalizationManager mLocalizationManager;
		#endregion

		#region Unity Methods
		protected virtual void Start()
		{
			mLocalizationManager = LocalizationManager.Get;
			if(mLocalizationManager)
				mLocalizationManager.AssignToManager(this);
			
		}
		#endregion

		#region Methods
		public abstract void SetText();
		#endregion

		#region Implementation

		#endregion
	}
}
