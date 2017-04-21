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
		public Vector3[]	Tabouret;
	}

#region Static
	private static GameData mInstance;
	public static GameData Get { get{ return mInstance; } }
#endregion

#region Fields
	// Public ------------------------------------------------------------------
	public SaveData		Data;
	// Temporary
	public GameObject	TabouretModel;
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
		Load();
	}

	void OnApplicationQuit()
	{
		Save();
	}
#endregion

#region Methods
	public void Load()
	{
		var dataPath = Path.Combine(Application.persistentDataPath, "SaveData.xml");
		if(File.Exists(dataPath))
		{
			Data = Serialization.FromFile<SaveData>(dataPath);
		}
		else
		{
			var textAsset = Resources.Load("DefaultData") as TextAsset;
			Data = Serialization.FromString<SaveData>(textAsset.text);
		}
	}

	public void Save()
	{
		var dataPath = Path.Combine(Application.persistentDataPath, "SaveData.xml");
		Serialization.ToFile<SaveData>(Data, dataPath);
	}
#endregion
}
