//******************************************************************************
// Authors: Frederic SETTAMA  
//******************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CubeTexture", menuName = "Scriptable Object/CubeTexture", order = 1)]
public class CubeTexture : ScriptableObject
{
	[System.Serializable]
	public class NumberTexture
	{
		public Texture  Normal;
		public Texture  Circle;
	}

	[System.Serializable]
	public class TextureEntry
	{
		public int				Key;
		public NumberTexture	Value;
	}

	#region Parameters
	public List<TextureEntry> NumberTextures;
	[Header("Cube material")]
	public Material DefaultMat;
	public Material LockMat;
	public Material WrongMat;
	public Material SelectedMat;
	public Material TransparentMat;
	#endregion

	#region Fields
	// Const -------------------------------------------------------------------

	// Private -----------------------------------------------------------------
	private Dictionary<int, NumberTexture>  mTextures = null;
	#endregion

	#region Methods
	public Texture GetTexture(int number, bool circle)
	{
		NumberTexture tex = null;
		if (mTextures == null)
			GenerateDictionnary();
		if (mTextures.TryGetValue(number, out tex))
		{
			if (circle)
				return tex.Circle;
			return tex.Normal;
		}
		return null;
	}
	#endregion

	#region Implementation
	private void GenerateDictionnary()
	{
		mTextures = new Dictionary<int, NumberTexture>();
		foreach (TextureEntry entry in NumberTextures)
		{
			mTextures.Add(entry.Key, entry.Value);
		}
	}
	#endregion
}
