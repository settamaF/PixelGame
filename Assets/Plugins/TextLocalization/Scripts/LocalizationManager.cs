using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.Globalization;
using UnityEngine.Networking;
using System.Collections;
using System.Xml;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
//******************************************************************************

namespace Localization
{
	[System.Serializable]
	public class LocalizationData
	{
		public List<LocalizationItem>   Items;
	}

	[System.Serializable]
	public class LocalizationItem
	{
		public string   Key;
		public string   Value;

		public LocalizationItem()
		{
		}

		public LocalizationItem(KeyValuePair<string, string> item)
		{
			Key = item.Key;
			Value = item.Value;
		}
	}

	public class LocalizationManager : MonoBehaviour
	{
		#region Enum
		public enum eTypeOfFile
		{
			XML,
			JSON,
		};

		public enum eDownloadMethod
		{
			RESOURCES,
			STREAMINGASSET,
			HTTP
		};
		#endregion

		#region Script Parameters
		public string                       Filename = "MyLocalizationFile";
		public eTypeOfFile                  TypeOfFile = eTypeOfFile.XML;
		public eDownloadMethod              DownloadMethod = eDownloadMethod.RESOURCES;
		public bool                         UseOtherDefaultLanguage = false;
		public string                       OtherDefaultLanguage = "en";
		[Header("Generation")]
		public bool							AutoGeneration = false;
		public bool							GenerateAllType = false;
		#endregion

		#region Static
		public static LocalizationManager Get { get { return mInstance; } }
		private static LocalizationManager  mInstance;
		#endregion

		#region Properties
		public bool IsReady { get; private set; }
		public string CurrentLanguage { get { return mCurrentLanguage; } }
		#endregion

		#region Fields
		// Const -------------------------------------------------------------------
		private const string                LOG_HEADER = "[LocalizationManager]";
		private const string                TEXT_ID_NOT_FOUND = "{0} not found";
		private const string                SEPARATOR = "_";
		// Private -----------------------------------------------------------------
		private List<LocalizedField>        mListLocalizedText = new List<LocalizedField>();
		private Dictionary<string, string>  mDictionaryText = new Dictionary<string, string>();
		private string                      mCurrentLanguage;
		private string                      mCurrentFilename;
		private string mCurrentFilenameWithExtension
		{
			get
			{
				switch (TypeOfFile)
				{
					case eTypeOfFile.JSON:
						return mCurrentFilename + ".json";
					case eTypeOfFile.XML:
						return mCurrentFilename + ".xml";
					default:
						return mCurrentFilename;
				}
			}
		}
		#endregion

		#region Unity Methods
		void Awake()
		{
			if (mInstance != null && mInstance != this)
			{
				Destroy(this);
				return;
			}
			if (mInstance == null)
				mInstance = this;
			if (transform.parent == null)
				DontDestroyOnLoad(gameObject);
			IsReady = false;
			if (UseOtherDefaultLanguage)
				mCurrentLanguage = OtherDefaultLanguage;
			else
				mCurrentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			Init(mCurrentLanguage);
		}

		void Start()
		{
			
		}

#if UNITY_EDITOR
		void OnApplicationQuit()
		{
			if (!AutoGeneration)
				return;
			SaveFile();
		}
#endif
		#endregion

		#region Methods

		public void AssignToManager(LocalizedField text)
		{
			if (!mListLocalizedText.Contains(text))
				mListLocalizedText.Add(text);
		}

		public string GetValue(string key)
		{
			if (mDictionaryText.ContainsKey(key))
				return mDictionaryText[key];
			else
			{
#if UNITY_EDITOR
				if (AutoGeneration)
					mDictionaryText.Add(key, key);
#endif
				return string.Format(TEXT_ID_NOT_FOUND, key);
			}
		}

		public void SwitchLocalization(string newLanguage)
		{
			newLanguage = newLanguage.ToLower();
			if (mCurrentLanguage == newLanguage)
				return;
			mCurrentLanguage = newLanguage;
			Init(newLanguage);
		}
		#endregion

		#region Implementation
		private void Init(string language)
		{
			mCurrentFilename = GetFolder() + Filename + "_" + language;
			Debug.LogFormat("{0}: load file {1}", LOG_HEADER, mCurrentFilename);
			DownloadFile();
		}

		private void UpdateCurrentText()
		{
			for (int i = 0; i < mListLocalizedText.Count; i++)
			{
				mListLocalizedText[i].SetText();
			}
		}

		private void DownloadFile()
		{
			switch (DownloadMethod)
			{
				case eDownloadMethod.RESOURCES:
					DownloadFileToResources();
					break;
				case eDownloadMethod.STREAMINGASSET:
					DownloadFileToStreamingAssset();
					break;
				case eDownloadMethod.HTTP:
					DownloadFileToHTTP();
					break;
				default:
					break;
			}
		}

		private void DownloadFileToResources()
		{
			TextAsset data = Resources.Load(mCurrentFilename) as TextAsset;

			if (data == null)
			{
				Debug.LogError("Not found the file " + mCurrentFilename + " in Resources folder");
				return;
			}
			LocalizationData deserializedData = null;
			switch (TypeOfFile)
			{
				case eTypeOfFile.JSON:
					deserializedData = FromJsonTextAsset<LocalizationData>(data);
					break;
				case eTypeOfFile.XML:
					deserializedData = FromXmlTextAsset<LocalizationData>(data);
					break;
				default:
					break;
			}
			if (deserializedData != null)
				InitDictionnary(deserializedData);
		}

		private void DownloadFileToStreamingAssset()
		{
			string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, mCurrentFilenameWithExtension);

			LocalizationData deserializedData = null;
			switch (TypeOfFile)
			{
				case eTypeOfFile.JSON:
					deserializedData = FromJsonFile<LocalizationData>(filePath);
					break;
				case eTypeOfFile.XML:
					deserializedData = FromXmlFile<LocalizationData>(filePath);
					break;
				default:
					break;
			}
			if (deserializedData != null && deserializedData.Items != null)
				InitDictionnary(deserializedData);
		}

		private void DownloadFileToHTTP()
		{
			//not implemented
		}

		private void InitDictionnary(LocalizationData data)
		{
			mDictionaryText = new Dictionary<string, string>();
			foreach (var item in data.Items)
				mDictionaryText.Add(item.Key, item.Value);
			IsReady = true;
			UpdateCurrentText();
			Debug.LogFormat("{0}: Sucess file loaded {1}", LOG_HEADER, mCurrentFilename);
		}

		IEnumerator FromFileWithUrl(string path, Action<string, bool> callback)
		{
			UnityWebRequest www = UnityWebRequest.Get(path);
#if UNITY_2017_3_OR_NEWER
			yield return www.SendWebRequest();
#else
			yield return www.Send();
#endif
			var result = www.downloadHandler.text;
			if (www.isHttpError || www.isNetworkError)
				callback(result, true);
			else
				callback(result, false);
		}

		private void CallbackDownloadJsonFile(string data, bool error)
		{
			if (error)
			{
				Debug.LogError("Download json file failed: " + data);
				return;
			}
			if (string.IsNullOrEmpty(data))
			{
				Debug.LogError("Not found the file or is empty" + mCurrentFilename);
				return;
			}
			LocalizationData deserializedData = FromJsonString<LocalizationData>(data);
			if (deserializedData != null && deserializedData.Items != null)
				InitDictionnary(deserializedData);
		}

		private void CallbackDownloadXmlFile(string data, bool error)
		{
			if (error)
			{
				Debug.LogError("Download xml file failed: " + data);
				return;
			}
			if (string.IsNullOrEmpty(data))
			{
				Debug.LogError("Not found the file or is empty" + mCurrentFilename);
				return;
			}
			LocalizationData deserializedData = FromXmlString<LocalizationData>(data);
			if (deserializedData != null && deserializedData.Items != null)
				InitDictionnary(deserializedData);
		}

		private string GetFolder()
		{
			switch (TypeOfFile)
			{
				case eTypeOfFile.JSON:
					return "Json/";
				case eTypeOfFile.XML:
					return "Xml/";
				default:
					return "";
			}
		}
		#endregion

		#region Serialization
		private T FromXmlFile<T>(string path)
		{
			try
			{
				if (path.Contains("://"))
				{
					StartCoroutine(FromFileWithUrl(path, CallbackDownloadXmlFile));
					return default(T);
				}
				if (!File.Exists(path))
				{
					Debug.LogError("Not found the file " + mCurrentFilename + " in StreamingAsset folder");
					return default(T);
				}
				XmlSerializer serializer = new XmlSerializer(typeof(T));

				using (FileStream fileStream = new FileStream(path, System.IO.FileMode.Open))
				{
					return (T)serializer.Deserialize(fileStream);
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Serialization.FromXmlFile - exception " + e.ToString());
				return default(T);
			}
		}

		private T FromXmlString<T>(string text)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));

				using (TextReader reader = new StringReader(text))
				{
					return (T)serializer.Deserialize(reader);
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Serialization.FromXmlString - exception " + e.ToString());
				return default(T);
			}
		}

		private T FromXmlTextAsset<T>(TextAsset textAsset)
		{
			try
			{
				T newObject = FromXmlString<T>(textAsset.text);
				return newObject;
			}
			catch (Exception e)
			{
				Debug.LogError("Serialization.FromXmlTextAsset - exception " + e.ToString());
				return default(T);
			}
		}

		private T FromJsonFile<T>(string path)
		{
			try
			{
				if (path.Contains("://"))
				{
					StartCoroutine(FromFileWithUrl(path, CallbackDownloadJsonFile));
					return default(T);
				}
				if (!File.Exists(path))
				{
					Debug.LogError("Not found the file " + mCurrentFilename + " in StreamingAsset folder");
					return default(T);
				}
				string dataAsJson = File.ReadAllText(path, Encoding.UTF8);
				T loadedData = JsonUtility.FromJson<T>(dataAsJson);
				return loadedData;
			}
			catch (Exception e)
			{
				Debug.LogError("Serialization.FromjsonFile - exception " + e.ToString());
				return default(T);
			}
		}

		private T FromJsonString<T>(string text)
		{
			try
			{
				T loadedData = JsonUtility.FromJson<T>(text);
				return loadedData;
			}
			catch (Exception e)
			{
				Debug.LogError("Serialization.FromjsonString - exception " + e.ToString());
				return default(T);
			}
		}

		private T FromJsonTextAsset<T>(TextAsset textAsset)
		{
			try
			{
				T newObject = JsonUtility.FromJson<T>(textAsset.text);
				return newObject;
			}
			catch (Exception e)
			{
				Debug.LogError("Serialization.FromJsonTextAsset - exception " + e.ToString());
				return default(T);
			}
		}

		private void ToJsonTextAsset<T>(T dictionary)
		{
#if UNITY_EDITOR
			TextAsset data = Resources.Load("Json/" + Filename + "_" + mCurrentLanguage) as TextAsset;
			string path;
			if (data == null)
			{
				path = Path.Combine(Application.dataPath + "/Resources", "Json/" + Filename + "_" + mCurrentLanguage + ".json");
			}
			else
			{
				path = AssetDatabase.GetAssetPath(data);
			}
			string text = JsonUtility.ToJson(dictionary);
			WriteTheFile(AddNewLine(text, ",{"), path);
#endif
		}


		private void ToJsonFile<T>(T dictionary)
		{
			string path = Path.Combine(Application.streamingAssetsPath, "Json/" + Filename + "_" + mCurrentLanguage + ".json");
			string text = JsonUtility.ToJson(dictionary);
			WriteTheFile(AddNewLine(text, ",{"), path);
		}

		private void ToXmlTextAsset<T>(T dictionary)
		{
#if UNITY_EDITOR
			TextAsset data = Resources.Load("Xml/" + Filename + "_" + mCurrentLanguage) as TextAsset;
			string path;
			if (data == null)
			{
				path = Path.Combine(Application.dataPath + "/Resources", "Xml/" + Filename + "_" + mCurrentLanguage + ".xml");
			}
			else
			{
				path = AssetDatabase.GetAssetPath(data);
			}
			string text = ObjectXmlToString(dictionary);
			WriteTheFile(AddNewLine(text, "><"), path);
#endif
		}

		private void ToXmlFile<T>(T dictionary)
		{
			string path = Path.Combine(Application.streamingAssetsPath, "Xml/" + Filename + "_" + mCurrentLanguage + ".xml");
			string text = ObjectXmlToString(dictionary);
			WriteTheFile(AddNewLine(text, "><"), path);
		}

		private string AddNewLine(string text, string separator)
		{
			var builder = new StringBuilder();
			char lastChar = ' ';
			foreach (var c in text)
			{
				if (string.Equals(separator, lastChar.ToString() + c.ToString()))
					builder.Append('\n');
				builder.Append(c);
				lastChar = c;
			}
			return builder.ToString();
		}

		private string ObjectXmlToString<T>(T obj)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			StringWriter sw = new StringWriter();

			using (XmlWriter xmlWriter = XmlWriter.Create(sw, settings))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(xmlWriter, obj, ns);
			}
			return sw.ToString();
		}

		private void CreateFolder(string path)
		{
			string fullPath = Path.GetFullPath(path);
			int lastSeparator = fullPath.LastIndexOf('\\');
			fullPath = fullPath.Remove(lastSeparator);
			if (Directory.Exists(fullPath))
				return;
			CreateFolder(fullPath);
			Directory.CreateDirectory(fullPath);
		}

		private void WriteTheFile(string text, string path)
		{
			if (File.Exists(path))
				File.Delete(path);
			else
				CreateFolder(path);
			File.WriteAllText(path, text, Encoding.UTF8);
		}
		#endregion

		#region AutoGeneration
		public void SaveFile()
		{
			var dictionary = GenerateList();

			if (dictionary == null)
			{
				Debug.Log("File not generate: no localized field in the scene");
				return;
			}
			if (GenerateAllType)
			{
				ToJsonTextAsset(dictionary);
				ToJsonFile(dictionary);
				ToXmlTextAsset(dictionary);
				ToXmlFile(dictionary);
			}
			else
			{
				switch (TypeOfFile)
				{
					case eTypeOfFile.JSON:
						switch (DownloadMethod)
						{
							case eDownloadMethod.RESOURCES:
								ToJsonTextAsset(dictionary);
								break;
							case eDownloadMethod.STREAMINGASSET:
								ToJsonFile(dictionary);
								break;
						}
						break;
					case eTypeOfFile.XML:
						switch (DownloadMethod)
						{
							case eDownloadMethod.RESOURCES:
								ToXmlTextAsset(dictionary);
								break;
							case eDownloadMethod.STREAMINGASSET:
								ToXmlFile(dictionary);
								break;
						}
						break;
					default:
						break;
				}
			}
			Debug.Log("Generate localization file completed");
		}

		public LocalizationData GenerateList()
		{
			if (mDictionaryText == null || mDictionaryText.Count == 0)
				return null;
			var ret = new LocalizationData
			{
				Items = new List<LocalizationItem>()
			};
			foreach (var data in mDictionaryText)
			{
				var item = new LocalizationItem(data);
				ret.Items.Add(item);
			}
			return ret;
		}

		private string FindPath(string filename)
		{
			return "";
		}
#endregion
	}
}
