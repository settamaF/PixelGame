//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************************************************************

public class TextureManager : MonoBehaviour 
{
	[System.Serializable]
	public class NumberTexture
	{
		public Texture	Normal;
		public Texture	Circle;
	}

	[System.Serializable]
	public class TextureEntry
	{
		public int				Key;
		public NumberTexture	Value;
	}

#region Script Parameters
	[SerializeField]
	private List<TextureEntry> NumberTextures;
	[Header("Cube material")]
	public Material DefaultMat;
	public Material LockMat;
	public Material WrongMat;
#endregion

#region Properties

#endregion

#region Fields
	// Const -------------------------------------------------------------------

	// Static ------------------------------------------------------------------
	private static TextureManager		mInstance;
	public static TextureManager		Get { get { return mInstance; } }
	// Private -----------------------------------------------------------------
	private Dictionary<int, NumberTexture>	mTextures;
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
		mTextures = new Dictionary<int, NumberTexture>();
		foreach(TextureEntry entry in NumberTextures)
		{
			mTextures.Add(entry.Key, entry.Value);
		}
		Debug.Log("TextureManager loaded", this);
	}

#endregion

#region Methods
	public Texture GetTexture(int number, bool circle)
	{
		NumberTexture tex = null;
		if(mTextures.TryGetValue(number, out tex))
		{
			if(circle)
				return tex.Circle;
			return tex.Normal;
		}
		return null;
	}
#endregion

#region Implementation

#endregion
}
