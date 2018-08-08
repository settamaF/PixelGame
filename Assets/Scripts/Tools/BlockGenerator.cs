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
	public Vector3Int   MaxSize = new Vector3Int(9, 9, 9);
	public string		PathModel = "Assets/Data/Models";
	public Model        ModelToGenerate;
	#endregion

	#region Properties

	#endregion

	#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private GameObject mModel3D;
	#endregion

	#region Unity Methods
	#endregion

	#region Methods

	#endregion

	#region Implementation
	private void SetupModel()
	{
		ModelToGenerate.Name = ModelToGenerate.name;
		ModelToGenerate.Prefab = PrefabModel;
		ModelToGenerate.Size = MaxSize;
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
		Vector3 pos = size / 2;
		pos.y = 0;
		mModel3D.transform.position += pos;
		renderer.transform.localPosition = Vector3.zero;
	}
	#endregion

	#region Editor
	[Button("Generation")]
	private void GenerateModel()
	{
		ClearModel();
		ModelToGenerate = Utils.CreateAsset<Model>(PathModel, PrefabModel.name);
		SetupModel();
		Initialization();
		SetupData(ModelToGenerate);
		GenerateBlock(mMaxSize);
		foreach (var cube in mCubes)
			cube.transform.SetParent(transform);
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
		foreach(var cube in mCubes)
			DestroyImmediate(cube.gameObject);
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
	}
	#endregion
}