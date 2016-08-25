using UnityEngine;
using System.Collections;
using HyperGames.Localisation;

public class SettingsPanel : MonoBehaviour {

	// GameObjects

    // Vars

    // ---------------------- GETTERS/SETTERS ----------------------
    // -------------------------------------------------------------
    
	// ---------------------- START FUNCTION -----------------------
    // -------------------------------------------------------------
    private void Awake() {
		// Should probably be call from a centralised script
		// and not on a panel like this -- this is just to ensure
		// that a language gets populated to start with
		TextManager.Initialise();
	}

    // ---------------------- UPDATE FUNCTION ----------------------
    // -------------------------------------------------------------
    
    // --------------------- PUBLIC FUNCTIONS ----------------------
    // -------------------------------------------------------------
	
    // -------------------- PROTECTED FUNCTIONS --------------------
    // -------------------------------------------------------------

    // --------------------- PRIVATE FUNCTIONS ---------------------
    // -------------------------------------------------------------

    // ------------------------- HANDLERS --------------------------
    // -------------------------------------------------------------
	public void OnClickGbr() {
		TextManager.LoadNewLanguage(Languages.GBR);
	}

	public void OnClickNor() {
		TextManager.LoadNewLanguage(Languages.NOR);
	}
}