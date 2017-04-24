//******************************************************************************
// Authors: Frédéric SETTAMA
//    
//******************************************************************************

using UnityEngine;
using System.IO;
using System.Collections.Generic;

//******************************************************************************
public class GameData : MonoBehaviour
{
	[System.Serializable]
	public class SaveData
	{
		public int[]		ModelIdCompleted;
	}

	[System.Serializable]
	public class ModelData
	{
		public int			Id;
		public string		Name;
		public GameObject	Prefab;
		public int			Size;
		public Vector3[]	ValidCube;
	}

#region Static
	private static GameData mInstance;
	public static GameData Get { get{ return mInstance; } }
#endregion

#region Fields
	// Public ------------------------------------------------------------------
	public SaveData		PlayerData;
	public List<ModelData>	ModelsData;
#endregion

#region Unity Methods
	void Awake()
	{
		if(mInstance != null && mInstance != this)
		{
			DestroyImmediate(this.gameObject, true);
			return;
		}
		
		DontDestroyOnLoad (this);
		mInstance = this;
		LoadPlayerData();
		//if(ModelsData == null || ModelsData.Count == 0)
		//	LoadModels();
	}

	void OnApplicationQuit()
	{
		SavePlayerData();
	}
#endregion

#region Methods
	public void LoadPlayerData()
	{
		var dataPath = Path.Combine(Application.persistentDataPath, "SaveData.xml");
		if(File.Exists(dataPath))
		{
			PlayerData = Serialization.FromFile<SaveData>(dataPath);
		}
		else
		{
			PlayerData = new SaveData();
		}
	}

	public void SavePlayerData()
	{
		var dataPath = Path.Combine(Application.persistentDataPath, "SaveData.xml");
		Serialization.ToFile<SaveData>(PlayerData, dataPath);
	}
#endregion

#region Implementation
	/*void LoadModels()
	{
		var textAsset = Resources.Load("DefaultData") as TextAsset;
		ModelsData = Serialization.FromString<List<ModelData>>(textAsset.text);
	}

	void NewFile()
	{
		ModelsData = new List<ModelData>();
	}

	void SaveNewFile()
	{
		var dataPath = Path.Combine(Application.persistentDataPath, "TestData.xml");
		Serialization.ToFile<List<ModelData>>(ModelsData, dataPath);
	}*/
#endregion
}
