using System.Collections;
using System.Linq;

public static class ExtensionsHashtable
{
	public static bool TryGetString(this Hashtable hash, string key, out string val, string defaultValue = "")
	{
		if (hash.ContainsKey(key))
		{
			val = hash[key].ToString();
			return true;
		}
		val = defaultValue;
		return false;
	}
	public static string GetString(this Hashtable hash, string key, string defaultValue = "")
	{
		if (hash.ContainsKey(key))
			return hash[key].ToString();
		return defaultValue;
	}
	public static bool TryGetInt(this Hashtable hash, string key, out int val, int defaultValue = 0)
	{
		if (hash.ContainsKey(key))
		{
			if (int.TryParse(hash[key].ToString(), out val))
				return true;
			else
			{
				val = defaultValue;
				return false;
			}
		}
		val = defaultValue;
		return false;
	}
	public static int GetInt(this Hashtable hash, string key, int defaultValue = 0)
	{
		if (hash.ContainsKey(key))
		{
			int val;
			if (int.TryParse(hash[key].ToString(), out val))
				return val;
		}
		return defaultValue;
	}
	public static bool TryGetFloat(this Hashtable hash, string key, out float val, float defaultValue = 0)
	{
		if (hash.ContainsKey(key))
		{
			if (float.TryParse(hash[key].ToString(), out val))
				return true;
			else
			{
				val = defaultValue;
				return false;
			}
		}
		val = defaultValue;
		return false;
	}
	public static float GetFloat(this Hashtable hash, string key, float defaultValue = 0)
	{
		if (hash.ContainsKey(key))
		{
			float val;
			if (float.TryParse(hash[key].ToString(), out val))
				return val;
		}
		return defaultValue;
	}
	public static bool TryGetBool(this Hashtable hash, string key, out bool val, bool defaultValue = false)
	{
		if (hash.ContainsKey(key))
		{
			if (bool.TryParse(hash[key].ToString(), out val))
				return true;
			else
			{
				val = defaultValue;
				return false;
			}
		}
		val = defaultValue;
		return false;
	}
	public static bool GetBool(this Hashtable hash, string key, bool defaultValue = false)
	{
		if (hash.ContainsKey(key))
		{
			bool val;
			if (bool.TryParse(hash[key].ToString(), out val))
				return val;
		}
		return defaultValue;
	}
	public static bool TryGetStringEqual(this Hashtable hash, string key, string val)
	{
		string currentValue;
		return hash.TryGetString(key, out currentValue) && currentValue == val;
	}
	public static bool TryGetIntEqual(this Hashtable hash, string key, int val)
	{
		int currentValue;
		return hash.TryGetInt(key, out currentValue) && currentValue == val;
	}
	public static bool TryGetFloatEqual(this Hashtable hash, string key, float val)
	{
		float currentValue;
		return hash.TryGetFloat(key, out currentValue) && currentValue == val;
	}
	public static bool TryGetBoolEqual(this Hashtable hash, string key, bool val)
	{
		bool currentValue;
		return hash.TryGetBool(key, out currentValue) && currentValue == val;
	}
	public static bool TryGetStringArray(this Hashtable hash, string key, out string[] stringArray, params char[] splitCharacter)
	{
		string val;
		if (splitCharacter == null || splitCharacter.Length == 0)
			splitCharacter = new char[] { ',' };
		if (hash.TryGetString(key, out val))
		{
			stringArray = val.Split(splitCharacter, System.StringSplitOptions.RemoveEmptyEntries);
			return true;
		}
		stringArray = new string[0];
		return false;
	}
}
