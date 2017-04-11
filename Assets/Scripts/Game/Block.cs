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
	public GameObject	CubePrefab;
#endregion

#region Properties

#endregion

#region Fields
	// Private -----------------------------------------------------------------
	private Cube[,,]	mCubes;
	private Vector3		mCubeSize;
	private int			mMaxSize;
	private Dictionary<Vector2, float>  FrontValidCube;
	private Dictionary<Vector2, float>  LefttValidCube;
	private Dictionary<Vector2, float>  UpValidCube;
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
		mCubes = new Cube[size, size, size];
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

	public void SetVisibilityCube()
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
		Vector3 pos = new Vector3();

		pos.x = mCubeSize.x * x + mCubeSize.x / 2;
		pos.y = mCubeSize.y * y + mCubeSize.y / 2;
		pos.z = mCubeSize.z * z + mCubeSize.z / 2;
		var obj = Instantiate<GameObject>(CubePrefab, pos, Quaternion.identity, transform);
		var cube = obj.GetComponent<Cube>();
		return cube;
	}

	
	void SetNumberOnFace()
	{
	
	}

	void SetNumberOnFace(int x, int y, int z)
	{

	}

	void CountAllValidCube()
	{
		//Front-Back
		for(int x = 0; x < mMaxSize; x++)
		{
			for(int y = 0; y < mMaxSize; y++)
			{
				float validCount = CountValidCube(x, y, 0, Vector3.forward);

			}
		}
		//Left-Right

		//Up-Down
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
						mCubes[x, y, z].SetState(Cube.eState.Disable);
					}
				}
			}
		}
	}
#endregion
}
