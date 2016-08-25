using UnityEngine;

using System;
using System.Collections.Generic;

using SimpleJSON;

#if HYPER_MESSAGING
using HyperGames.MessageSystem;
using HyperGames.MessageSystem.Messages;
#endif

namespace HyperGames.Localisation {
	
	// A sample definition of languages that are supported
	public enum Languages { 
		None = 0, // Reference: http://www.worldatlas.com/aatlas/ctycodes.htm
		NOR = 1, // Norway
		SWE = 2, // Sweden
		DNK = 3, // Denmark
		DEU = 4, // Germany
		GBR = 5 // Great Britain aka oxford english
	}

	// A sample definiton of different types of localisation files that can be loaded
	public enum TextDataType { 
		None = 0,
		UI = 1
	}

	/// <summary>
	/// Loads JSON files with text data according to a language
	/// </summary>
	public static class TextManager {

		// GameObjects

		// Vars
		public const string EOL = "#EOL#";
		private const string LANG = "[lang]";
		private const string MAIN_PATH = "Localisation/" + LANG + "/";
		public static bool hasLoaded = false;
		private static Action _nullAction = () => { };
		public static Action OnLoadComplete = _nullAction;
        private static Dictionary<int, JSONNode> _datas;

		// ---------------------- GETTERS/SETTERS ----------------------
		// -------------------------------------------------------------
    
		// ---------------------- START FUNCTION -----------------------
		// -------------------------------------------------------------
		
		// ---------------------- UPDATE FUNCTION ----------------------
		// -------------------------------------------------------------
    
		// --------------------- PUBLIC FUNCTIONS ----------------------
		// -------------------------------------------------------------
		/// <summary>
		/// If you want control over when a load occurs and/or, what
		/// default language will be loaded, use this function
		/// If not, this function will be run as soon as a 'GetDataSource'
		/// is called.
		/// </summary>
		/// <param name="currentLang"></param>
		public static void Initialise(Languages currentLang = Languages.GBR) {
			if (hasLoaded) {
				return;
			}
			LoadNewLanguage(currentLang);
		}

		/// <summary>
		/// Loads in a new set of language file(s), and ensures all registered components
		/// sets/updates its text data.
		/// </summary>
		/// <param name="newLang"></param>
		public static void LoadNewLanguage(Languages newLang) {
			_datas = new Dictionary<int, JSONNode>();
			
			// let's fetch the resource path to load from
			// F.ex. "Localisation/GBR/" if language is set to GBR
			string basepath = MAIN_PATH.Replace(LANG, newLang.ToString());
			
			// traverse all entries in 'TextDataType' and load the needed
			// JSON files
			string[] names = System.Enum.GetNames(typeof(TextDataType));
			TextDataType[] values = (TextDataType[])System.Enum.GetValues(typeof(TextDataType));
			int len = names.Length;
			TextDataType value;
			for (int i = 0; i < len; ++i) {
				value = values[i];
				if (value == TextDataType.None) {
					continue;
				}
				
				_datas.Add((int)value, JSON.Parse((Resources.Load<TextAsset>(basepath + value.ToString())).text));
			}
			
			hasLoaded = true;
			
#if HYPER_MESSAGING
			Messenger.Dispatch(new NTextManagerLoadComplete());
#else
			OnLoadComplete();
#endif
		}

		/// <summary>
		/// Fetches the a text data set for the given 'TextDataType'
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static JSONNode GetDataSource(TextDataType type) {
			Initialise();

			if (type == TextDataType.None) {
				Debug.LogError("There are not data source for the given type: " + type);
			}

			return _datas[(int)type];
		}

		/// <summary>
		/// Used to convert end of line codes to correct end of line string
		/// since you can't add eol's in a JSON file
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ConvertEOL(string value) {
			return value.Replace(EOL, "\n");
		}

		// -------------------- PROTECTED FUNCTIONS --------------------
		// -------------------------------------------------------------

		// --------------------- PRIVATE FUNCTIONS ---------------------
		// -------------------------------------------------------------

		// ------------------------- HANDLERS --------------------------
		// -------------------------------------------------------------
	}
}
