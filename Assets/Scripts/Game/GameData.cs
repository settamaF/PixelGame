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
		public int[]	ModelIdCompleted;
	}

#region Static	
	public static GameData Get { get; private set; }
#endregion

#region Fields
	// Public ------------------------------------------------------------------
	public SaveData		PlayerData;
	public CubeTexture	CubeTextureData;
	public List<Model>	ModelsData;
#endregion

#region Unity Methods
	void Awake()
	{
		if (Get != null && Get != this)
		{
			Destroy(gameObject);
			return;
		}
		if (Get == null)
			Get = this;
		if (transform.parent == null)
			DontDestroyOnLoad(gameObject);
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
