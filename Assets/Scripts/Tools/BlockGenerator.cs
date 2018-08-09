//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
		Vector3 Point;
		Vector3 Start = new Vector3(x + mCubeSize.x / 2, y + mCubeSize.y / 2, mCubeSize.z / 2 - 100);
		Vector3 Goal = new Vector3(x + mCubeSize.x / 2, y + mCubeSize.y / 2, z + mCubeSize.z / 2);
		Vector3 Direction = Goal - Start;
		Direction.Normalize();
		int Itterations = 0;
		Point = Start;

		while (Point != Goal)
		{
			RaycastHit hit;
			if (Physics.Linecast(Point, Goal, out hit))
			{
				Itterations++;
				Point = hit.point + (Direction / 100.0f);
			}
			else
				Point = Goal;
		}
		while (Point != Start)
		{
			RaycastHit hit;
			if (Physics.Linecast(Point, Start, out hit))
			{
				Itterations++;
				Point = hit.point + (-Direction / 100.0f);
			}
			else
				Point = Start;
		}
		if (Itterations % 2 == 0)
			return false;
		return true;
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
		GenerateValidCube();
		EditorUtility.SetDirty(ModelToGenerate);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	[Button("GenerateValidCube")]
	private void GenerateValidCube()
	{
		if (ModelToGenerate == null || mModel3D == null)
		{
			Debug.LogError("Generation cube is not initialized");
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
						mValidCube.Add(new Vector3(x, y, z));
				}
			}
		}
		if (mValidCube != null && mValidCube.Count > 0)
		{
			foreach(var cube in mValidCube)
				GenerateValidCube((int)cube.x, (int)cube.y, (int)cube.z);
			ModelToGenerate.ValidCube = mValidCube.ToArray();
		}
			
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
#endif