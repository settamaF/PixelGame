//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
	public float	MoveSensibility = 2f;
	public float	SpeedRotation = 200f;
	public float	DelayPressed = 0.5f;
	[Header("Unity remote")]
	public bool		UnityRemote = true;
#endregion

#region Properties
	public Vector3			DefaultCameraPosition { get; set; }
	public List<GameObject>	SelectedObj{ get; set; }
#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------
	private static InputManager		mInstance;
	public static InputManager		Get { get { return mInstance; } }

	// Private -----------------------------------------------------------------
	private bool				mMoved = false;
	private bool				mSelectObj = false;
	private bool				mOnUI = false;
	private Vector2				mStartPos;
	public float				mCurrentDelayPressed;
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
		SelectedObj = new List<GameObject>();
	}

	void Update()
	{
#if !UNITY_EDITOR
		TouchInput();
#else
		if(UnityRemote)
			TouchInput();
		else
			WindowsInput();
#endif
	}

	void OnEnable()
	{
		ResetSelectedObj();
	}
#endregion

#region Methods
	public void ClearSelectedObj()
	{
		SelectedObj.Clear();
		mSelectObj = false;
		mCurrentDelayPressed = 0;
	}
#endregion

#region Implementation

	bool ZoomCamera(bool TouchInput)
	{
		if(TouchInput)
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
		else
		{
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
			return false;
		}
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
		if(mStartPos == position || Vector3.Distance(mStartPos, position) <= MoveSensibility)
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
			Game.Get.MakeAction(hit.collider.gameObject);
		}
	}

	void SelectCube(Vector2 position)
	{
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit))
		{
			var cube = hit.collider.gameObject.GetComponent<Cube>();
			if(cube)
			{
				if(cube.State == Cube.EState.Enable)
				{
					SelectedObj.Add(hit.collider.gameObject);
					cube.SetState(Cube.EState.Selected);
				}
			}
		}
	}

	void ResetSelectedObj()
	{
		foreach(var obj in SelectedObj)
		{
			var cube = obj.GetComponent<Cube>();
			if(cube)
				cube.SetState(Cube.EState.Enable);
		}
		SelectedObj.Clear();
		mCurrentDelayPressed = 0;
	}

	private bool IsPointerOverUIObject()
	{
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	void TouchInput()
	{
		if(ZoomCamera(true))
			return;
		if(Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);

			switch(touch.phase)
			{
				case TouchPhase.Began:
					mStartPos = touch.position;
					mMoved = false;
					if(IsPointerOverUIObject())
					{
						mOnUI = true;
						Debug.Log("On UI");
					}
					else
					{
						if(!mSelectObj)
						{
							mCurrentDelayPressed = -touch.deltaTime;
							ResetSelectedObj();
						}
					}
					break;

				case TouchPhase.Moved:
					if(mOnUI)
						return;
					if(mSelectObj)
						break;
					if(RotateBlock(touch.position))
					{
						mStartPos = touch.position;
						mMoved = true;
					}
					break;
				case TouchPhase.Ended:
					if(mMoved == false && SelectedObj.Count == 0 && mOnUI == false)
						TouchCube(touch.position);
					mOnUI = false;
					break;
			}
			if(mOnUI)
				return;
			mCurrentDelayPressed += touch.deltaTime;
			if(mMoved == false && mCurrentDelayPressed >= DelayPressed)
			{
				if(!mSelectObj)
				{
					mSelectObj = true;
					Game.Get.SetSelectionAction();
				}
				SelectCube(touch.position);
			}
		}
	}

	void WindowsInput()
	{
		if(ZoomCamera(false))
			return;
		if(Input.GetMouseButtonDown(0))
		{
			Debug.Log("Mouse input");

			mStartPos = Input.mousePosition;
			mMoved = false;
			if(IsPointerOverUIObject())
				mOnUI = true;
			else
			{
				if(!mSelectObj)
				{
					ResetSelectedObj();
					mCurrentDelayPressed = 0;
				}
			}
		}
		else if(Input.GetMouseButton(0))
		{
			if(mOnUI)
				return;
			if(!mSelectObj)
			{
				if(RotateBlock(Input.mousePosition))
				{
					mStartPos = Input.mousePosition;
					mMoved = true;
				}
			}
			mCurrentDelayPressed += Time.deltaTime;
			if(mMoved == false && mCurrentDelayPressed >= DelayPressed)
			{
				if(!mSelectObj)
				{
					mSelectObj = true;
					Game.Get.SetSelectionAction();
				}
				SelectCube(Input.mousePosition);
			}
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(mOnUI)
			{
				mOnUI = false;
				return;
			}
			if(mMoved == false && SelectedObj.Count == 0)
				TouchCube(Input.mousePosition);
			mOnUI = false;
		}
	}
#endregion
}