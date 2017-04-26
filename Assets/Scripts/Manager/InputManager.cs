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
	[Header("Sensitivity")]
	public float	ZoomSpeed = 0.5f;
	public float	MoveSensibility = 2f;
	public float	SpeedRotation = 200f;
	public float	DelayPressed = 0.5f;
	public float    DelayDoubleTap = 0.2f;
	[Header("Camera param")]
	public Camera	Camera;
	public float	CameraMinPos = -4f;
	public float	CameraMaxPos = -20f;
	public bool		UseFov = false;
	public float	MinFov = 50f;
	public float	MaxFov = 70f;
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
	private bool					mMoved = false;
	private bool					mSelectObj = false;
	private bool					mOnUI = false;
	private Vector2					mStartPos;
	private float					mCurrentDelayPressed;
	private GameObject				mPrevObjSelected;
	private float					mLastTapTime;
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
	public void ResetSelectedObj()
	{
		foreach(var obj in SelectedObj)
		{
			var cube = obj.GetComponent<Cube>();
			if(cube)
				cube.SetState(Cube.EState.Enable);
		}
		SelectedObj.Clear();
		MenuManager.Get.Hud.EnableSelectionBtn(false);
	}

	public void SelectCube(Cube cube)
	{
		var obj = cube.gameObject;
		if(cube.State == Cube.EState.Selected && obj != mPrevObjSelected)
		{
			SelectedObj.Remove(obj);
			if(SelectedObj.Count == 0)
				MenuManager.Get.Hud.EnableSelectionBtn(false);
			cube.SetState();
			mPrevObjSelected = obj;
		}
		else if(cube.State == Cube.EState.Enable && obj != mPrevObjSelected)
		{
			SelectedObj.Add(obj);
			if(SelectedObj.Count == 1)
				MenuManager.Get.Hud.EnableSelectionBtn(true);
			cube.SetState(Cube.EState.Selected);
			mPrevObjSelected = obj;
		}
	}

	public void SelectCube(Cube cube, Cube.EState state)
	{
		var obj = cube.gameObject;
		if(cube.State == Cube.EState.Selected || cube.State == Cube.EState.Enable)
		{
			if(cube.State != state)
			{
				if(state == Cube.EState.Selected)
				{
					SelectedObj.Add(obj);
					if(SelectedObj.Count == 1)
						MenuManager.Get.Hud.EnableSelectionBtn(true);
					cube.SetState(Cube.EState.Selected);
					mPrevObjSelected = obj;
				}
				else
				{
					SelectedObj.Remove(obj);
					if(SelectedObj.Count == 0)
						MenuManager.Get.Hud.EnableSelectionBtn(false);
					cube.SetState();
					mPrevObjSelected = obj;
				}
			}
		}
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

	void SelectObj(Vector2 position)
	{
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit))
		{
			var obj = hit.collider.gameObject;
			var cube = obj.GetComponent<Cube>();
			if(cube)
				SelectCube(cube);
		}
	}

	bool IsPointerOverUIObject()
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
				}
				SelectObj(touch.position);
			}
		}
	}

	void WindowsInput()
	{
		if(ZoomCamera(false))
			return;
		if(Input.GetMouseButtonDown(0))
		{
			if(IsPointerOverUIObject())
			{
				mOnUI = true;
				return;
			}
			mStartPos = Input.mousePosition;
			mMoved = false;
			mSelectObj = false;
			mPrevObjSelected = null;
			mCurrentDelayPressed = 0;
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

			if(!mMoved && !mSelectObj && Game.Get.CurrentAction == Game.EAction.Selection)
			{
				mCurrentDelayPressed += Time.deltaTime;
				if(mCurrentDelayPressed >= DelayPressed)
				{
					mSelectObj = true;
				}
			}
			if(mSelectObj)
				SelectObj(Input.mousePosition);
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(mOnUI)
			{
				mOnUI = false;
				return;
			}
			if(Game.Get.CurrentAction == Game.EAction.Selection)
			{
				var deltaTime = Time.time - mLastTapTime;
				mLastTapTime = Time.time;
				if(deltaTime <= DelayDoubleTap)
				{
					SelectCubeInLine(Input.mousePosition);
					return;
				}
			}
			if(!mMoved && !mSelectObj)
				TouchCube(Input.mousePosition);
		}
	}

	void SelectCubeInLine(Vector2 position)
	{
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit))
		{
			var cube = hit.collider.GetComponent<Cube>();
			if(cube)
			{
				Cube.ESide side = cube.GetHitFace(hit);
				Vector3 dir = Vector3.zero;
				switch(side)
				{
					case Cube.ESide.Front:
						dir = Vector3.forward;
						break;
					case Cube.ESide.Back:
						dir = Vector3.back;
						break;
					case Cube.ESide.Left:
						dir = Vector3.right;
						break;
					case Cube.ESide.Right:
						dir = Vector3.left;
						break;
					case Cube.ESide.Top:
						dir = Vector3.down;
						break;
					case Cube.ESide.Down:
						dir = Vector3.up;
						break;
					default:
						return;
				}
				var cubes = cube.Parent.SelectCubes(cube.Position, dir);
				foreach(var cubeInList in cubes)
				{
					SelectCube(cubeInList, cube.State);
				}
			}
		}
	}


#endregion
}