//******************************************************************************
// Author: Frédéric SETTAMA
//******************************************************************************

using UnityEngine;
using System.Security.Cryptography;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

//******************************************************************************
public static class Utils
{
	// Encrypt the specified plainText using MD5 hash.
	public static string Encrypt( string plainText )
	{
		// Calculate MD5 hash from input
		MD5.Create();
		MD5 md5 			= MD5.Create();
		byte[] inputBytes 	= Encoding.ASCII.GetBytes( plainText );
		byte[] hash 		= md5.ComputeHash( inputBytes );
		
		// Convert byte array to hex string
		var stringBuilder = new StringBuilder();
		for (int i = 0; i < hash.Length; i++)
		{
			stringBuilder.Append(hash[i].ToString("X2"));
		}
		
		return stringBuilder.ToString();
	}

	public static string TextToKey(string value)
	{
		if (string.IsNullOrEmpty(value))
			return "";
		if (value[0] == '[' && value[value.Length - 1] == ']')
			return value;
		return "[" + value.ToUpper() + "]";
	}

#if UNITY_EDITOR
	/// <summary>
	/// Create new asset from <see cref="ScriptableObject"/> type with unique name at
	/// selected folder in project window. Asset creation can be cancelled by pressing
	/// escape key when asset is initially being named.
	/// </summary>
	/// <typeparam name="T">Type of scriptable object.</typeparam>
	public static T CreateAsset<T>(string pathModel, string fileName) where T : ScriptableObject
	{
		var asset = ScriptableObject.CreateInstance<T>();
		AssetDatabase.CreateAsset(asset, pathModel + "/" + fileName + ".asset");
		return asset;
	}

#endif
}
