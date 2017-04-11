//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections.Generic;

//******************************************************************************
public class InputManager : MonoBehaviour
{
#region Script Parameters
#endregion

#region Properties
#endregion

#region Fields
	// Const -------------------------------------------------------------------
	private const float SPEED = 1;
	private const float DELAYDOUBLECLICK = 0.4f;
	//static
	private static InputManager     mInstance;
	public static InputManager Get { get { return mInstance; } }

	// Private -----------------------------------------------------------------
	private GameObject              mObjectGrab = null;
	private Vector3                 mLastPosition;
	private float                   mLastClickTimer;
	private GameObject              mLastObject;
#endregion

#region Unity Methods
	void Awake()
	{
		if(mInstance != null && mInstance != this)
		{
			DestroyImmediate(this, true);
			return;
		}
		mInstance = this;
		Debug.Log("InputManager loaded", this);
	}

	void Update()
	{
		DebugInputCube();
	}

#endregion

#region Methods

#endregion

#region Implementation

#endregion

#region Debug
	public void DebugInputCube()
	{
	}
#endregion
}
