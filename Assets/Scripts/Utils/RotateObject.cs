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
	private Vector3		mFromCameraPos;
	private Vector3		mToCameraPos;
	private float		mCurrentTime = 0;
	private Camera		mMainCamera;
#endregion

#region Unity Methods

	void Start()
	{
		if(transform.rotation != Quaternion.identity)
		{
			mResetRotation = true;
			mFromRotation = transform.rotation;
		}
		mMainCamera = Camera.main;
		mToCameraPos = InputManager.Get.DefaultCameraPosition;
		mFromCameraPos = mMainCamera.transform.position;

	}
	void Update()
	{
		if(mResetRotation)
		{
			mCurrentTime += Time.deltaTime / Duration;
			transform.rotation = Quaternion.Lerp(mFromRotation, Quaternion.identity, mCurrentTime);
			mMainCamera.transform.position = Vector3.Lerp(mFromCameraPos, mToCameraPos, mCurrentTime);
			if(mCurrentTime >= 1)
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
