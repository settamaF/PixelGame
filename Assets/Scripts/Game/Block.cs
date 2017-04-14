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
	[Header("Temporary")]
	public float						SpeedRotation;
#endregion

#region Properties

#endregion

#region Fields
	// Private -----------------------------------------------------------------
	private Cube[,,]					mCubes;
	private Vector3						mCubeSize;
	private int							mMaxSize;
	private Dictionary<Vector2, float>	mFrontValidCube;
	private Dictionary<Vector2, float>	mLefttValidCube;
	private Dictionary<Vector2, float>	mTopValidCube;
	private Vector3						mGlobalPosition;
#endregion

#region Unity Methods
	void Start () 
	{
		Init(4);
	}
#endregion

#region Methods
	public void Init(int size)
	{
		mGlobalPosition = Vector3.zero;
		mCubes = new Cube[size, size, size];
		mFrontValidCube = new Dictionary<Vector2, float>();
		mLefttValidCube = new Dictionary<Vector2, float>();
		mTopValidCube = new Dictionary<Vector2, float>();
		mCubeSize = CubePrefab.GetComponent<Renderer>().bounds.size;
		mMaxSize = size;
		for(int z = 0; z < mMaxSize; z++)
		{
			for(int x = 0; x < mMaxSize; x++)
			{
				for(int y = 0; y < mMaxSize; y++)
				{
					mCubes[x, y, z] = GenerateCube(x, y, z);
				}
			}
		}
		SetCenterBlock();
		CenterCamera();
		SetValidCube(GameData.Get.Data.Tabouret);
		CountAllValidCube();
		SetNumberOnFace();
	}

	public void DisableAllCube()
	{
		for(int z = 0; z < mMaxSize; z++)
		{
			for(int x = 0; x < mMaxSize; x++)
			{
				for(int y = 0; y < mMaxSize; y++)
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
		if(cube.Valid)
		{
			cube.SetState(Cube.EState.Lock);
			return;
		}
		cube.SetState(Cube.EState.Disable);
		//Front cube
		if(z - 1 >= 0)
		{
			cube = mCubes[x, y, z - 1];
			if(cube.State != Cube.EState.Disable)
				SetNumberOnFace(x, y, z - 1, Cube.ESide.Back);
		}
		//Back cube
		if(z + 1 < mMaxSize)
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
		if(x + 1 < mMaxSize)
		{
			cube = mCubes[x + 1, y, z];
			if(cube.State != Cube.EState.Disable)
				SetNumberOnFace(x + 1, y, z, Cube.ESide.Left);
		}
		//Top cube
		if(y + 1 < mMaxSize)
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

	public void LockCube()
	{

	}
#endregion

#region Implementation
	void SetValidCube(Vector3[] coords)
	{
		foreach(var pos in coords)
		{
			mCubes[(int)pos.x, (int)pos.y, (int)pos.z].SetValid();
		}
	}
	
	Cube GenerateCube(int x, int y, int z)
	{
		Vector3 pos = new Vector3()
		{
			x = mCubeSize.x * x + mCubeSize.x / 2,
			y = mCubeSize.y * y + mCubeSize.y / 2,
			z = mCubeSize.z * z + mCubeSize.z / 2
		};
		mGlobalPosition += pos;
		var obj = Instantiate<GameObject>(CubePrefab, pos, Quaternion.identity);
		var cube = obj.GetComponent<Cube>();
		cube.Position = new Vector3(x, y, z);
		cube.Parent = this;
		return cube;
	}

	
	void SetNumberOnFace()
	{
		//Front-Back
		for(int x = 0; x < mMaxSize; x++)
		{
			for(int y = 0; y < mMaxSize; y++)
			{
				SetNumberOnFace(x, y, 0, Cube.ESide.Front);
				SetNumberOnFace(x, y, mMaxSize - 1, Cube.ESide.Back);
			}
		}
		//Left-Right
		for(int y = 0; y < mMaxSize; y++)
		{
			for(int z = 0; z < mMaxSize; z++)
			{
				SetNumberOnFace(0, y, z, Cube.ESide.Left);
				SetNumberOnFace(mMaxSize - 1, y, z, Cube.ESide.Right);
			}
		}
		//Top-Down
		for(int x = 0; x < mMaxSize; x++)
		{
			for(int z = 0; z < mMaxSize; z++)
			{
				SetNumberOnFace(x, 0, z, Cube.ESide.Down);
				SetNumberOnFace(x, mMaxSize - 1, z, Cube.ESide.Top);
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
		for(int x = 0; x < mMaxSize; x++)
		{
			for(int y = 0; y < mMaxSize; y++)
			{
				validCount = CountValidCube(x, y, 0, Vector3.forward);
				mFrontValidCube.Add(new Vector2(x, y), validCount);
			}
		}
		//Left-Right
		for(int y = 0; y < mMaxSize; y++)
		{
			for(int z = 0; z < mMaxSize; z++)
			{
				validCount = CountValidCube(0, y, z, Vector3.right);
				mLefttValidCube.Add(new Vector2(y, z), validCount);
			}
		}
		//Top-Down
		for(int x = 0; x < mMaxSize; x++)
		{
			for(int z = 0; z < mMaxSize; z++)
			{
				validCount = CountValidCube(x, 0, z, Vector3.up);
				mTopValidCube.Add(new Vector2(x, z), validCount);
			}
		}
	}

	float CountValidCube(int x, int y, int z, Vector3 dir)
	{
		bool space = false;
		float count = 0;
		Vector3 pos = new Vector3(x, y, z);
		for(int i=0; i<mMaxSize; i++)
		{
			var tmpPos = pos + dir * i;
			var cube = mCubes[(int)tmpPos.x, (int)tmpPos.y, (int)tmpPos.z];
			if(cube && cube.Valid)
			{
				count++;
			}
			else
			{
				if(count != 0)
					space = true;
			}
		}
		return space ? count + 0.5f: count;
	}

	void SetCenterBlock()
	{
		Vector3 centerPosition;

		centerPosition = mGlobalPosition / (mMaxSize * mMaxSize * mMaxSize);
		transform.position = centerPosition;
		foreach(var cube in mCubes)
		{
			cube.transform.SetParent(transform);
		}
	}

	void CenterCamera()
	{
		var cam = Camera.main;

		var pos = cam.transform.position;
		pos.x = transform.position.x;
		pos.y = transform.position.y;
		cam.transform.position = pos;
	}
#endregion

#region Debug
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			DisableNotValideCube();
		}
	}

	void DisableNotValideCube()
	{
		for(int z = 0; z < mMaxSize; z++)
		{
			for(int x = 0; x < mMaxSize; x++)
			{
				for(int y = 0; y < mMaxSize; y++)
				{
					if(!mCubes[x, y ,z].Valid)
					{
						mCubes[x, y, z].SetState(Cube.EState.Disable);
					}
				}
			}
		}
	}

#endregion
}
