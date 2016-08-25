using UnityEngine;
using UnityEngine.UI;

using SimpleJSON;

#if TM_PRO
using TMPro;
#endif

#if HYPER_MESSAGING
using HyperGames.MessageSystem;
using HyperGames.MessageSystem.Messages;
#endif

namespace HyperGames.Localisation {

	/// <summary>
	/// Fetches text data from a 'TextManager' and populates the given 'TextField' 
	/// or 'TextMeshProUGUI' according to the data given.
	/// 
	/// 'TextMeshPro' is not included in this package and must be bought separately. 
	/// If you want to use it go to:
	/// 'Build Settings' -> 'Player Settings' -> 'Scripting Define Symbols'
	/// and add 'TM_PRO' as one of the keywords
	/// 
	/// The same goes for Hyper Games Message System, if you are using it you need
	/// to add 'HYPER_MESSAGING' to 'Scripting Define Symbols'
	/// </summary>
	public class TextDataFromJSON : MonoBehaviour {

		// GameObjects
		private Text _textfield;
#if TM_PRO
		private TextMeshProUGUI _textfieldPro;
#endif

		// Vars
		[Tooltip("An array of names for JSON nodes to specify where the text being populated live in the JSON file.")]
		public string[] jsonNodeIds;
		[Tooltip("If set to true, the script will randomise an JSON array of entries for the given JSON node.")]
		public bool isRandomising = false;

		// ---------------------- GETTERS/SETTERS ----------------------
		// -------------------------------------------------------------

		// ---------------------- START FUNCTION -----------------------
		// -------------------------------------------------------------
		private void Awake() {
			_textfield = GetComponent<Text>();
#if TM_PRO
			_textfieldPro = GetComponent<TextMeshProUGUI>();
#endif

			// Start listening for callbacks for when a language
			// change has occured
#if HYPER_MESSAGING
			Messenger.AddListener<NTextManagerLoadComplete>(OnLoadComplete);
#else
			TextManager.OnLoadComplete += OnLoadComplete;
#endif
			if (TextManager.hasLoaded) {
				Populate();
			}
		}

		// ---------------------- UPDATE FUNCTION ----------------------
		// -------------------------------------------------------------

		// --------------------- PUBLIC FUNCTIONS ----------------------
		// -------------------------------------------------------------
		/// <summary>
		/// This function is used to force an update of the text
		/// F.ex. in a preloader and using 'isRandomising' you can change
		/// the text at a given point in time (if the loading time is high)
		/// </summary>
		public void TriggerPopulate() {
			Populate();
		}
		
		// -------------------- PROTECTED FUNCTIONS --------------------
		// -------------------------------------------------------------

		// --------------------- PRIVATE FUNCTIONS ---------------------
		// -------------------------------------------------------------
		/// <summary>
		/// Gets JSON data from 'TextManager' and tries to populate the given
		/// textfield with text according to the 'jsonNodeIds' information provided
		/// </summary>
		private void Populate() {
			JSONNode node = TextManager.GetDataSource(TextDataType.UI);

			// let's fetch the correct node
			int len = jsonNodeIds.Length;
			string nodePath = "";
			for (int i = 0; i < len; ++i) {
				node = node[jsonNodeIds[i]];
				nodePath += jsonNodeIds[i] + "/";
			}

			// is this a proper node?
			if (node == null) {
				if (_textfield != null) {
					Debug.LogError("Trying to populate a textfield with value that doesn't exist: '" + nodePath + "' " + _textfield, _textfield);
				}
#if TM_PRO
				else if (_textfieldPro != null) {
					Debug.LogError("Trying to populate a textfield with value that doesn't exist: '" + nodePath + "' " + _textfieldPro, _textfieldPro);
				}
#endif
				return;
			}
			
			// convert node reference to string
			string text = node;

			// fetch a randomised entry if needed
			if (isRandomising) {
				int randomIndex = Random.Range(0, node.AsArray.Count);
				text = node[randomIndex];
			}
			
			// populate text data
			if (_textfield != null) {
				_textfield.text = TextManager.ConvertEOL(text);
			}
#if TM_PRO
			else if (_textfieldPro != null) {
				_textfieldPro.text = TextManager.ConvertEOL(text);
			}
#endif
		}

		// ------------------------- HANDLERS --------------------------
		// -------------------------------------------------------------
#if HYPER_MESSAGING
		/// <summary>
		/// If using a messaging system, this will allow the field to populate
		/// automatically when language has changed
		/// </summary>
		/// <param name="msg"></param>
		private void OnLoadComplete(NTextManagerLoadComplete msg) {
			Populate();
		}
#else 
		private void OnLoadComplete() {
			Populate();
		}
#endif
	}
}