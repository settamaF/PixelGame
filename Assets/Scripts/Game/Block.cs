//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************

public class Block : MonoBehaviour 
{
#region Script Parameters
	public GameObject					CubePrefab;
#endregion

#region Properties

#endregion

#region Fields
	// Protected -----------------------------------------------------------------
	protected Cube[,,]						mCubes;
	protected Vector3						mCubeSize;
	protected Vector3Int					mMaxSize;
	protected Dictionary<Vector2, float>	mFrontValidCube;
	protected Dictionary<Vector2, float>	mLefttValidCube;
	protected Dictionary<Vector2, float>	mTopValidCube;
	protected Vector3						mGlobalPosition;
	protected GameObject					mModel;

	//temporary
	private bool						mInitialized = false;
	private int							mNumberValidCube;
	private int							mCurrentNumberCube;
#endregion

#region Unity Methods
	void Awake()
	{
		mInitialized = false;
	}
	
#endregion

#region Methods
	public void Initialization()
	{
		mGlobalPosition = Vector3.zero;
		mFrontValidCube = new Dictionary<Vector2, float>();
		mLefttValidCube = new Dictionary<Vector2, float>();
		mTopValidCube = new Dictionary<Vector2, float>();
		mCubeSize = CubePrefab.GetComponent<Renderer>().bounds.size;
	}

	public void Init(Model model)
	{
		Initialization();
		SetupData(model);
		GenerateBlock(mMaxSize);
		SetCenterBlock();
		CenterCamera();
		SetValidCube(model.ValidCube);
		CountAllValidCube();
		SetNumberOnFace();
		mInitialized = true;
	}

	public void DisableAllCube()
	{
		for(int z = 0; z < mMaxSize.z; z++)
		{
			for(int x = 0; x < mMaxSize.x; x++)
			{
				for(int y = 0; y < mMaxSize.y; y++)
				{
					mCubes[x, y, z].SetVisibility(false);
				}
			}
		}
	}

	public void DestroyCube(Vector3 position)
	{
		int x = (int)position.x;
		int y = (int)position.y;
		int z = (int)position.z;
		var cube = mCubes[x, y , z];
		if(cube.State == Cube.EState.Lock || cube.State == Cube.EState.LockWithError)
			return;
		if(cube.Valid)
		{
			cube.SetState(Cube.EState.LockWithError);
			Game.Get.RemoveLife();
			return;
		}
		cube.SetState(Cube.EState.Disable);
		mCurrentNumberCube--;
		//Front cube
		if(z - 1 >= 0)
		{
			cube = mCubes[x, y, z - 1];
			if(cube.State != Cube.EState.Disable)
				SetNumberOnFace(x, y, z - 1, Cube.ESide.Back);
		}
		//Back cube
		if(z + 1 < mMaxSize.z)
		{
			cube = mCubes[x, y, z + 1];
			if(cube.State != Cube.EState.Disable)
				SetNumberOnFace(x, y, z + 1, Cube.ESide.Front);
		}
		//Left cube
		if(x - 1 >= 0)
		{
			cube = mCubes[x - 1, y, z];
			if(cube.State != Cube.EState.Disable)
				SetNumberOnFace(x - 1, y, z, Cube.ESide.Right);
		}
		//Right cube
		if(x + 1 < mMaxSize.x)
		{
			cube = mCubes[x + 1, y, z];
			if(cube.State != Cube.EState.Disable)
				SetNumberOnFace(x + 1, y, z, Cube.ESide.Left);
		}
		//Top cube
		if(y + 1 < mMaxSize.y)
		{
			cube = mCubes[x, y + 1, z];
			if(cube.State != Cube.EState.Disable)
				SetNumberOnFace(x, y + 1, z, Cube.ESide.Down);
		}
		//Down cube
		if(y - 1 >= 0)
		{
			cube = mCubes[x, y - 1, z];
			if(cube.State != Cube.EState.Disable)
				SetNumberOnFace(x, y - 1, z, Cube.ESide.Top);
		}
	}

	public void LockCube(Vector3 position)
	{
		int x = (int)position.x;
		int y = (int)position.y;
		int z = (int)position.z;
		var cube = mCubes[x, y, z];
		if(cube.State == Cube.EState.Lock)
		{
			cube.SetState(Cube.EState.Enable);
		}
		else
		{
			cube.SetState(Cube.EState.Lock);
		}
	}

	public bool IsCompleted()
	{
		if(mCurrentNumberCube == mNumberValidCube && mInitialized)
		{
			foreach(Transform child in transform)
			{
				Destroy(child.gameObject);
			}
			transform.rotation = Quaternion.identity;
			var obj = Instantiate<GameObject>(mModel, transform);
			CenterModel3D(obj.transform);
			return true;
		}
		return false;
	}

	public List<Cube> SelectCubes(Vector3 pos, Vector3 dir)
	{
		Vector3 currentPos = pos + dir;
		List<Cube> ret = new List<Cube>();

		while(!IsOutOfRange(currentPos, 0, mMaxSize))
		{
			ret.Add(mCubes[(int)currentPos.x, (int)currentPos.y, (int)currentPos.z]);
			currentPos += dir;
		}
		return ret;
	}
#endregion

#region Implementation
	protected void SetupData(Model model)
	{
		mMaxSize = model.Size;
		if(model.ValidCube != null)
			mNumberValidCube = model.ValidCube.Length;
		mModel = model.Prefab;
		mCubes = new Cube[mMaxSize.x, mMaxSize.y, mMaxSize.z];
		mCurrentNumberCube = mCubes.Length;
	}

	protected void GenerateBlock(Vector3Int maxSize)
	{
		for (int z = 0; z < maxSize.z; z++)
		{
			for (int x = 0; x < maxSize.x; x++)
			{
				for (int y = 0; y < maxSize.y; y++)
				{
					mCubes[x, y, z] = GenerateCube(x, y, z);
				}
			}
		}
	}
	void SetValidCube(Vector3[] coords)
	{
		foreach(var pos in coords)
		{
			mCubes[(int)pos.x, (int)pos.y, (int)pos.z].SetValid();
		}
	}
	
	protected Cube GenerateCube(int x, int y, int z)
	{
		Vector3 pos = new Vector3()
		{
			x = mCubeSize.x * x + mCubeSize.x / 2,
			y = mCubeSize.y * y + mCubeSize.y / 2,
			z = mCubeSize.z * z + mCubeSize.z / 2
		};
		mGlobalPosition += pos;
		var obj = Instantiate(CubePrefab, pos, Quaternion.identity);
		var cube = obj.GetComponent<Cube>();
		cube.Position = new Vector3(x, y, z);
		cube.Parent = this;
		return cube;
	}

	
	void SetNumberOnFace()
	{
		//Front-Back
		for(int x = 0; x < mMaxSize.x; x++)
		{
			for(int y = 0; y < mMaxSize.y; y++)
			{
				SetNumberOnFace(x, y, 0, Cube.ESide.Front);
				SetNumberOnFace(x, y, mMaxSize.z - 1, Cube.ESide.Back);
			}
		}
		//Left-Right
		for(int y = 0; y < mMaxSize.y; y++)
		{
			for(int z = 0; z < mMaxSize.z; z++)
			{
				SetNumberOnFace(0, y, z, Cube.ESide.Left);
				SetNumberOnFace(mMaxSize.x - 1, y, z, Cube.ESide.Right);
			}
		}
		//Top-Down
		for(int x = 0; x < mMaxSize.x; x++)
		{
			for(int z = 0; z < mMaxSize.z; z++)
			{
				SetNumberOnFace(x, 0, z, Cube.ESide.Down);
				SetNumberOnFace(x, mMaxSize.y - 1, z, Cube.ESide.Top);
			}
		}
	}

	void SetNumberOnFace(int x, int y, int z, Cube.ESide side)
	{
		float validCount = 0;

		switch(side)
		{
			case Cube.ESide.Front:
			case Cube.ESide.Back:
				validCount = mFrontValidCube[new Vector2(x, y)];
				break;
			case Cube.ESide.Left:
			case Cube.ESide.Right:
				validCount = mLefttValidCube[new Vector2(y, z)];
				break;
			case Cube.ESide.Top:
			case Cube.ESide.Down:
				validCount = mTopValidCube[new Vector2(x, z)];
				break;
		}
		var cube = mCubes[x, y, z];
		cube.SetSideNumber(side, validCount);
		cube.EnableSide(side, true);
	}

	void CountAllValidCube()
	{
		float validCount = 0;
		//Front-Back
		for(int x = 0; x < mMaxSize.x; x++)
		{
			for(int y = 0; y < mMaxSize.y; y++)
			{
				validCount = CountValidCube(x, y, 0, Vector3.forward, mMaxSize.z);
				mFrontValidCube.Add(new Vector2(x, y), validCount);
			}
		}
		//Left-Right
		for(int y = 0; y < mMaxSize.y; y++)
		{
			for(int z = 0; z < mMaxSize.z; z++)
			{
				validCount = CountValidCube(0, y, z, Vector3.right, mMaxSize.x);
				mLefttValidCube.Add(new Vector2(y, z), validCount);
			}
		}
		//Top-Down
		for(int x = 0; x < mMaxSize.x; x++)
		{
			for(int z = 0; z < mMaxSize.z; z++)
			{
				validCount = CountValidCube(x, 0, z, Vector3.up, mMaxSize.y);
				mTopValidCube.Add(new Vector2(x, z), validCount);
			}
		}
	}

	float CountValidCube(int x, int y, int z, Vector3 dir, int maxSize)
	{
		bool space = false;
		float count = 0;
		Vector3 pos = new Vector3(x, y, z);
		for (int i = 0; i < maxSize; i++)
		{
			var tmpPos = pos + dir * i;
			var cube = mCubes[(int)tmpPos.x, (int)tmpPos.y, (int)tmpPos.z];
			if (cube && cube.Valid)
			{
				if (space)
				{
					count += 0.5f;
					space = false;
				}
				count++;
			}
			else
			{
				if (count != 0 && count - (int)count == 0)
					space = true;
			}
		}
		return count;
	}

	protected void SetCenterBlock()
	{
		Vector3 centerPosition;

		centerPosition = mGlobalPosition / (mMaxSize.x * mMaxSize.y * mMaxSize.z);
		transform.position = centerPosition;
		foreach(var cube in mCubes)
			cube.transform.SetParent(transform);
	}
	
	protected void CenterModel3D(Transform trans)
	{
		Vector3 centerPos = trans.localPosition;

		centerPos.x += mMaxSize.x % 2 == 1 ? 0.5f : 0;
		centerPos.y -= (float)mMaxSize.y / 2;
		centerPos.z += mMaxSize.z % 2 == 1 ? 0.5f : 0;

		trans.localPosition = centerPos;
	}

	void CenterCamera()
	{
		var cam = Camera.main;

		var pos = cam.transform.position;
		pos.x = transform.position.x;
		pos.y = transform.position.y;
		pos.z = Game.DEFAULTPOSZCAMERA;
		cam.transform.position = pos;
		InputManager.Get.DefaultCameraPosition = pos;
	}

	bool IsOutOfRange(Vector3 pos, float min, Vector3Int max)
	{
		if(pos.x < min || pos.x >= max.x)
			return true;
		if(pos.y < min || pos.y >= max.y)
			return true;
		if(pos.z < min || pos.z >= max.z)
			return true;
		return false;
	}
#endregion
}
