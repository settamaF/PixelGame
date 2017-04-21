//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections.Generic;

//******************************************************************************
public class InputManager : MonoBehaviour
{
#region Script Parameters
	public Block	Block;
	public Camera	Camera;
	[Header("Sensitivity")]
	public float	ZoomSpeed = 0.5f;
	public float	CameraMinPos = -4f;
	public float	CameraMaxPos = -20f;
	public bool		UseFov = false;
	public float	MinFov = 50f;
	public float	MaxFov = 70f;
	public float	SpeedRotation = 200f;
	[Header("Unity remote")]
	public bool		UnityRemote = true;
#endregion

#region Properties
#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------
	private static InputManager		mInstance;
	public static InputManager		Get { get { return mInstance; } }

	// Private -----------------------------------------------------------------
	private bool mMoved = false;
	private Vector2 mStartPos;
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
		Camera = Camera.main;
	}

	void Update()
	{
		if(ZoomCamera())
			return;
		#if !UNITY_EDITOR
		if(Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);

			switch(touch.phase)
			{
				case TouchPhase.Began:
					mStartPos = touch.position;
					mMoved = false;
					break;

				case TouchPhase.Moved:
					if(RotateBlock(touch.position))
					{
						mStartPos = touch.position;
						mMoved = true;
					}
					break;
				case TouchPhase.Ended:
					if(mMoved == false)
						TouchCube(touch.position);
					break;
			}
		}
		#else
		if(Input.GetMouseButtonDown(0))
		{
			mStartPos = Input.mousePosition;
			mMoved = false;
		}
		else if(Input.GetMouseButton(0))
		{
			if(RotateBlock(Input.mousePosition))
			{
				mStartPos = Input.mousePosition;
				mMoved = true;
			}
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(mMoved == false)
				TouchCube(Input.mousePosition);
		}
#endif
	}

#endregion

#region Methods

#endregion

#region Implementation

	bool ZoomCamera()
	{
#if !UNITY_EDITOR
		if(Input.touchCount == 2)
		{
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			UpdateZoomCamera(-deltaMagnitudeDiff);
			return true;
		}
		return false;
#else
		if(UnityRemote)
		{
			if(Input.touchCount == 2)
			{
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

				UpdateZoomCamera(-deltaMagnitudeDiff);
				return true;
			}
			return false;
		}

		if(Input.GetKey(KeyCode.PageDown))
		{
			UpdateZoomCamera(-1);
			return true;
		}
		else if(Input.GetKey(KeyCode.PageUp))
		{
			UpdateZoomCamera(1);
			return true;
		}
		else
			return false;
#endif
	}

	void UpdateZoomCamera(float delta)
	{
		if(UseFov)
		{
			Camera.fieldOfView += delta * ZoomSpeed;
			Camera.fieldOfView = Mathf.Clamp(Camera.fieldOfView, MinFov, MaxFov);
		}
		else
		{
			Vector3 pos = Camera.transform.position;
			float z = pos.z;
			z += delta * ZoomSpeed;
			z = Mathf.Clamp(z, CameraMaxPos, CameraMinPos);
			pos.z = z;
			Camera.transform.position = pos;
		}
	}
	bool RotateBlock(Vector2 position)
	{
		if(mStartPos == position)
			return false;
		Vector2 direction = position - mStartPos;
		direction = direction.normalized;
		Block.transform.Rotate(Vector3.down, direction.x * SpeedRotation * Mathf.Deg2Rad, Space.World);
		Block.transform.Rotate(Vector3.right, direction.y * SpeedRotation * Mathf.Deg2Rad, Space.World);
		return true;
	}

	void TouchCube(Vector2 position)
	{
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit))
		{
			Block.DestroyObj(hit.collider.gameObject);
		}
	
	}
#endregion
}