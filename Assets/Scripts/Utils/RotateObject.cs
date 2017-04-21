//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;

//******************************************************************************

public class RotateObject : MonoBehaviour 
{
#region Script Parameters
	public float	Speed = 0.1f;
	public float	Duration = 0.5f;
#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private bool		mResetRotation = false;
	private Quaternion	mFromRotation;
	private float		mCurrentTime = 0;
#endregion

#region Unity Methods

	void Start()
	{
		if(transform.rotation != Quaternion.identity)
		{
			mResetRotation = true;
			mFromRotation = transform.rotation;
		}
	}
	void Update()
	{
		if(mResetRotation)
		{
			mCurrentTime += Time.deltaTime / Duration;
			transform.rotation = Quaternion.Lerp(mFromRotation, Quaternion.identity, mCurrentTime);
			if(transform.rotation == Quaternion.identity)
				mResetRotation = false;
			
		}
		else
			transform.Rotate(Vector3.down, Speed * Mathf.Deg2Rad, Space.World);
	}
#endregion

#region Methods

#endregion

#region Implementation
	
#endregion
}
