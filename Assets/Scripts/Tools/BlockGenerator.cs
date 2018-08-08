//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : Block
{
	#region Script Parameters
	public GameObject   PrefabModel;
	public string		PathModel = "Assets/Data/Models";
	public Model        ModelToGenerate;
	#endregion

	#region Properties

	#endregion

	#region Fields
	// Const -------------------------------------------------------------------
	private const int MAX_SIZE = 9;
	public float OFFSET_COLLISION = 0.1f;
	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private GameObject mModel3D;
	private MeshCollider mModel3DCollider;
	private Vector3Int mSizeModel;
	private List<Vector3> mValidCube;
	#endregion

	#region Unity Methods
	#endregion

	#region Methods

	#endregion

	#region Implementation
	private GameObject mSphere;
	private bool IsValidCube(float x ,float y, float z)
	{
		Vector3 pos = new Vector3(x > 0 ? x + OFFSET_COLLISION : x,
								  y > 0 ? y + OFFSET_COLLISION : y,
								  z > 0 ? z + OFFSET_COLLISION : z);

		Debug.Log(pos);
		Collider[] hitColliders = Physics.OverlapSphere(pos, 0.001f);
		int i = 0;
		if (hitColliders.Length == 0)
			Debug.Log("Nothing");
		while (i < hitColliders.Length)
		{
			Debug.Log(hitColliders[i].name);
			i++;
		}
		mSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		mSphere.transform.position = pos;
		mSphere.transform.localScale = new Vector3(.001f, .001f, .001f);
		return false;
		if (mSphere != null)
			DestroyImmediate(mSphere);
		bool ret;
		
		if (Physics.CheckSphere(pos, .001f))
			ret = true;
		else
			ret = false;
		mSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		mSphere.transform.position = pos;
		mSphere.transform.localScale = new Vector3(.001f, .001f, .001f);
		return ret;
	}

	private void SetupModel()
	{
		ModelToGenerate.Name = ModelToGenerate.name;
		ModelToGenerate.Prefab = PrefabModel;
		ModelToGenerate.Size = mSizeModel;
	}

	private void SetupPositionModel3d()
	{
		var renderer = mModel3D.GetComponentInChildren<Renderer>();

		if(renderer == null)
		{
			Debug.LogError("No render in model3d");
			return;
		}
		Vector3 size = renderer.bounds.size;
		mSizeModel = Vector3Int.FloorToInt(size);
		Vector3 pos = size / 2;
		pos.y = 0;
		mModel3D.transform.position += pos;
		renderer.transform.localPosition = Vector3.zero;
	}

	private void GenerateFullBlock()
	{
		GenerateBlock(mMaxSize);
		foreach (var cube in mCubes)
			cube.transform.SetParent(transform);
	}

	private void GenerateValidCube(int x, int y, int z)
	{
		var cube = GenerateCube(x, y, z);
		cube.transform.SetParent(transform);
		mCubes[x, y, z] = cube;
		mValidCube.Add(new Vector3(x, y, z));
	}
	#endregion

	#region Editor
	[Button("Generation Model")]
	private void GenerateModel()
	{
		ClearModel();
		ModelToGenerate = Utils.CreateAsset<Model>(PathModel, PrefabModel.name);
		mModel3D = Instantiate(PrefabModel, transform);
		SetupPositionModel3d();
		SetupModel();
		Initialization();
		SetupData(ModelToGenerate);
	}

	[Button("Generation Max Block")]
	private void GenerateBlock()
	{
		ClearModel();
		ModelToGenerate = Utils.CreateAsset<Model>(PathModel, PrefabModel.name);
		mSizeModel = new Vector3Int(MAX_SIZE, MAX_SIZE, MAX_SIZE);
		SetupModel();
		Initialization();
		SetupData(ModelToGenerate);
		mModel3D = Instantiate(mModel, transform);
		SetupPositionModel3d();
	}

	[Button("GenerateValidCube")]
	private void GenerateValidCube()
	{
		if (ModelToGenerate == null || mModel3D == null)
		{
			Debug.LogError("Generation cube is not initialized");
			return;
		}
		mModel3DCollider = mModel3D.GetComponentInChildren<MeshCollider>();
		if (mModel3DCollider == null)
		{
			Debug.LogError("No mesh collider found on the model 3D");
			return;
		}
		mValidCube = new List<Vector3>();
		for(int x = 0; x < mSizeModel.x; x++)
		{
			for (int y = 0; y < mSizeModel.y; y++)
			{
				for (int z = 0; z < mSizeModel.z; z++)
				{
					if (IsValidCube(x, y, z))
					{
						GenerateValidCube(x, y, z);
						Debug.LogFormat("Ok {0} {1} {2}", x, y, z);
					}
					else
					{
						Debug.LogFormat("not ok {0} {1} {2}", x, y, z);
					}
				}
			}
		}
		if (mValidCube != null && mValidCube.Count > 0)
			ModelToGenerate.ValidCube = mValidCube.ToArray();
	}

	public Vector3 Value;
	[Button("Test")]
	private void Check()
	{
		if (IsValidCube(Value.x, Value.y, Value.z))
		{
			Debug.LogFormat("Ok {0} {1} {2}", Value.x, Value.y, Value.z);
		}
		else
		{
			Debug.LogFormat("not ok {0} {1} {2}", Value.x, Value.y, Value.z);
		}
	}

	[Button("Clear Generator")]
	private void ClearModel()
	{
		if (mCubes == null)
		{
			ClearScene();
			return;
		}
		foreach (var cube in mCubes)
		{
			if(cube != null)
				DestroyImmediate(cube.gameObject);
		}
		if (mModel3D)
			DestroyImmediate(mModel3D);
		mCubes = null;
		mModel3D = null;
	}

	private void ClearScene()
	{
		foreach(Transform child in transform)
		{
			DestroyImmediate(child.gameObject);
		}
		if (transform.childCount > 0)
			ClearScene();
	}
	#endregion
}